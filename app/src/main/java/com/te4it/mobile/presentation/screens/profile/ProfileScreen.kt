package com.te4it.mobile.presentation.screens.profile

import android.content.Intent
import android.net.Uri
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavController
import com.te4it.mobile.data.local.TokenManager
import com.te4it.mobile.data.network.AuthApiService
import com.te4it.mobile.data.network.AuthInterceptor
import com.te4it.mobile.data.repository.AuthRepositoryImpl
import com.te4it.mobile.di.GsonModule
import com.te4it.mobile.di.NetworkModule
import com.te4it.mobile.presentation.viewmodel.ProfileViewModel
import com.te4it.mobile.presentation.viewmodel.ProfileViewModelFactory
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ProfileScreen(
    viewModelFactory: ViewModelFactory? = null, // Kept for compatibility if needed, but unused for now
    onNavigateToSettings: () -> Unit,
    onNavigateToEditProfile: () -> Unit,
    onNavigateToChangePassword: () -> Unit,
    onNavigateToAbout: () -> Unit,
    onLogout: () -> Unit
) {
    // Manual wiring (Service Locator style) kept local to screen scope
    val context = LocalContext.current
    val logging = remember { NetworkModule.provideLoggingInterceptor() }
    val tokenManager = remember(context) { TokenManager(context) }
    val authInterceptor = remember(tokenManager) { NetworkModule.provideAuthInterceptor(tokenManager) }
    
    // Circular dependency handling for TokenAuthenticator
    val authApiHolder = remember { 
        object {
            var api: AuthApiService? = null
        }
    }
    val authApiProvider: () -> AuthApiService = remember { { authApiHolder.api!! } }
    
    val tokenAuthenticator = remember(tokenManager) { NetworkModule.provideTokenAuthenticator(tokenManager, authApiProvider) }
    val okHttp = remember(logging, authInterceptor, tokenAuthenticator) { NetworkModule.provideOkHttpClient(logging, authInterceptor, tokenAuthenticator) }
    val retrofit = remember { NetworkModule.provideRetrofit(okHttp) }
    val api = remember { NetworkModule.provideAuthApiService(retrofit) }
    
    LaunchedEffect(api) {
        authApiHolder.api = api
    }
    
    val gson = remember { GsonModule.provideGson() }
    val repo = remember { AuthRepositoryImpl(api, tokenManager, gson) }
    val viewModel: ProfileViewModel = viewModel(factory = ProfileViewModelFactory(repo))

    val logoutState by viewModel.logoutState.collectAsState()
    
    // Kullanıcı bilgileri için state'ler
    var userEmail by remember { mutableStateOf<String?>("kullanici@example.com") }
    var userName by remember { mutableStateOf<String?>("Kullanıcı") }
    
    // Kullanıcı bilgilerini yükle
    LaunchedEffect(Unit) {
        tokenManager.getUserEmail().collect { email ->
            userEmail = email ?: "kullanici@example.com"
            // Mail adresinden kullanıcı adı çıkar
            userName = email?.substringBefore("@") ?: "Kullanıcı"
        }
    }
    
    // Handle logout success
    LaunchedEffect(logoutState.isSuccess) {
        if (logoutState.isSuccess) {
            viewModel.resetLogoutState()
            onLogout()
        }
    }

    // Show error snackbar
    val snackbarHostState = remember { SnackbarHostState() }
    LaunchedEffect(logoutState.errorMessage) {
        logoutState.errorMessage?.let {
            snackbarHostState.showSnackbar(
                message = it,
                duration = SnackbarDuration.Short
            )
            viewModel.resetLogoutState()
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Hesabım") },
                // Back button removed as it is a main tab
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = Color.White,
                    navigationIconContentColor = Color.White
                )
            )
        },
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { paddingValues ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .verticalScroll(rememberScrollState())
        ) {
            // Profile Header
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                // Profile Image
                Box(
                    modifier = Modifier
                        .size(128.dp)
                        .clip(CircleShape)
                        .background(MaterialTheme.colorScheme.primaryContainer),
                    contentAlignment = Alignment.Center
                ) {
                    Icon(
                        imageVector = Icons.Default.Person,
                        contentDescription = "Profil Resmi",
                        modifier = Modifier.size(64.dp),
                        tint = MaterialTheme.colorScheme.onPrimaryContainer
                    )
                }

                Spacer(modifier = Modifier.height(16.dp))

                // Dinamik kullanıcı adı
                Text(
                    text = userName ?: "Kullanıcı",
                    fontSize = 24.sp,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.onSurface
                )

                // Dinamik mail adresi
                Text(
                    text = userEmail ?: "kullanici@example.com",
                    fontSize = 16.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }

            // HESAP Section
            SectionHeader("HESAP")
            AccountSection(
                onProfileEditClick = onNavigateToEditProfile,
                onChangePasswordClick = onNavigateToChangePassword,
                onForgotPasswordClick = {
                    val intent = Intent(Intent.ACTION_VIEW, Uri.parse("https://te4it-frontend.azurestaticapps.net/forgot-password"))
                    context.startActivity(intent)
                }
            )

            // UYGULAMA AYARLARI Section
            SectionHeader("UYGULAMA AYARLARI")
            SettingsSection(
                onSettingsClick = onNavigateToSettings
            )

            // DESTEK Section
            SectionHeader("DESTEK")
            SupportSection(
                onAboutClick = onNavigateToAbout
            )

            Spacer(modifier = Modifier.weight(1f))

            // Logout Button
            OutlinedButton(
                onClick = { viewModel.logout() },
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp)
                    .height(50.dp),
                colors = ButtonDefaults.outlinedButtonColors(
                    contentColor = MaterialTheme.colorScheme.error
                )
            ) {
                Text(
                    text = "Çıkış Yap",
                    fontWeight = FontWeight.Bold
                )
            }

            // Version Text
            Text(
                text = "Uygulama Sürümü v1.0.0",
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                fontSize = 12.sp,
                color = MaterialTheme.colorScheme.onSurfaceVariant,
                textAlign = TextAlign.Center
            )
        }
    }
}

@Composable
fun SectionHeader(title: String) {
    Text(
        text = title,
        modifier = Modifier
            .fillMaxWidth()
            .padding(start = 16.dp, top = 24.dp, bottom = 8.dp),
        fontSize = 14.sp,
        fontWeight = FontWeight.Bold,
        color = MaterialTheme.colorScheme.onSurfaceVariant,
        letterSpacing = 0.5.sp
    )
}

@Composable
fun AccountSection(
    onProfileEditClick: () -> Unit,
    onChangePasswordClick: () -> Unit,
    onForgotPasswordClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        ),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column {
            ProfileListItem(
                icon = Icons.Default.Person,
                text = "Profili Düzenle",
                onClick = onProfileEditClick
            )

            Divider(
                modifier = Modifier.padding(start = 64.dp),
                color = MaterialTheme.colorScheme.outlineVariant
            )

            ProfileListItem(
                icon = Icons.Default.Lock,
                text = "Şifre Değiştir",
                onClick = onChangePasswordClick
            )
            
            Divider(
                modifier = Modifier.padding(start = 64.dp),
                color = MaterialTheme.colorScheme.outlineVariant
            )
            
            ProfileListItem(
                icon = Icons.Default.Key,
                text = "Şifremi Unuttum",
                onClick = onForgotPasswordClick
            )
        }
    }
}

@Composable
fun SettingsSection(
    onSettingsClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        ),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column {
            ProfileListItem(
                icon = Icons.Default.Settings,
                text = "Ayarlar (Görünüm & Bildirimler)",
                onClick = onSettingsClick
            )
        }
    }
}

@Composable
fun SupportSection(
    onAboutClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        ),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column {
            ProfileListItem(
                icon = Icons.Default.Info,
                text = "Hakkında",
                onClick = onAboutClick
            )
        }
    }
}

@Composable
fun ProfileListItem(
    icon: ImageVector,
    text: String,
    onClick: () -> Unit
) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .clickable { onClick() }
            .padding(16.dp),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Box(
            modifier = Modifier
                .size(40.dp)
                .clip(CircleShape)
                .background(MaterialTheme.colorScheme.primary.copy(alpha = 0.2f)),
            contentAlignment = Alignment.Center
        ) {
            Icon(
                imageVector = icon,
                contentDescription = null,
                tint = MaterialTheme.colorScheme.primary
            )
        }

        Spacer(modifier = Modifier.width(16.dp))

        Text(
            text = text,
            modifier = Modifier.weight(1f),
            fontSize = 16.sp,
            color = MaterialTheme.colorScheme.onSurface
        )

        Icon(
            imageVector = Icons.Default.ChevronRight,
            contentDescription = null,
            tint = MaterialTheme.colorScheme.onSurfaceVariant
        )
    }
}

fun showFeatureNotAvailableMessage(snackbarHostState: SnackbarHostState) {
    // Show a snackbar message indicating the feature is not available yet
    // In a real app, this would show a proper message
    CoroutineScope(Dispatchers.Main).launch {
        try {
            snackbarHostState.showSnackbar(
                message = "Bu özellik (Adım 3) geliştirme aşamasındadır.",
                duration = SnackbarDuration.Short
            )
        } catch (e: Exception) {
            // Ignore any exceptions that might occur when showing the snackbar
        }
    }
}