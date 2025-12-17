package com.example.vtys.presentation.screens.profile

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.KeyboardActions
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Visibility
import androidx.compose.material.icons.filled.VisibilityOff
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.focus.FocusDirection
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalFocusManager
import androidx.compose.ui.text.input.ImeAction
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavController
import com.example.vtys.data.local.TokenManager
import com.example.vtys.data.network.AuthApiService
import com.example.vtys.data.network.AuthInterceptor
import com.example.vtys.data.repository.AuthRepositoryImpl
import com.example.vtys.di.GsonModule
import com.example.vtys.di.NetworkModule
import com.example.vtys.presentation.viewmodel.ProfileViewModel
import com.example.vtys.presentation.viewmodel.ProfileViewModelFactory

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ChangePasswordScreen(
    navController: NavController
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
    val retrofit = remember(okHttp) { NetworkModule.provideRetrofit(okHttp) }
    val api = remember(retrofit) { NetworkModule.provideAuthApiService(retrofit) }
    
    LaunchedEffect(api) {
        authApiHolder.api = api
    }
    
    val gson = remember { GsonModule.provideGson() }
    val repo = remember(api, tokenManager, gson) { AuthRepositoryImpl(api, tokenManager, gson) }
    val viewModel: ProfileViewModel = viewModel(factory = ProfileViewModelFactory(repo))

    var currentPassword by remember { mutableStateOf("") }
    var newPassword by remember { mutableStateOf("") }
    var confirmNewPassword by remember { mutableStateOf("") }
    var currentPasswordVisible by remember { mutableStateOf(false) }
    var newPasswordVisible by remember { mutableStateOf(false) }
    var confirmNewPasswordVisible by remember { mutableStateOf(false) }
    var passwordsMatch by remember { mutableStateOf(true) }
    var passwordValidationError by remember { mutableStateOf<String?>(null) }

    val changePasswordState by viewModel.changePasswordState.collectAsState()
    val focusManager = LocalFocusManager.current

    // Show error snackbar
    val snackbarHostState = remember { SnackbarHostState() }
    
    // Handle success navigation
    LaunchedEffect(changePasswordState.isSuccess) {
        if (changePasswordState.isSuccess) {
            // Show success message
            snackbarHostState.showSnackbar(
                message = "Şifreniz başarıyla değiştirildi",
                duration = SnackbarDuration.Short
            )
            
            // Navigate to home screen after a short delay
            kotlinx.coroutines.delay(1000)
            navController.navigate("home") {
                popUpTo("profile") { inclusive = true }
            }
            viewModel.resetChangePasswordState()
        }
    }

    LaunchedEffect(changePasswordState.errorMessage) {
        changePasswordState.errorMessage?.let {
            snackbarHostState.showSnackbar(
                message = it,
                duration = SnackbarDuration.Short
            )
            viewModel.resetChangePasswordState()
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Şifre Değiştir") },
                navigationIcon = {
                    IconButton(onClick = { navController.popBackStack() }) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Geri")
                    }
                }
            )
        },
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { paddingValues ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .verticalScroll(rememberScrollState())
                .padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = "Yeni şifreniz en az 6 karakter uzunluğunda olmalıdır.",
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 24.dp)
            )

            // Current Password Field
            OutlinedTextField(
                value = currentPassword,
                onValueChange = { currentPassword = it },
                label = { Text("Mevcut Şifre") },
                leadingIcon = {
                    Icon(Icons.Default.Lock, contentDescription = null)
                },
                trailingIcon = {
                    IconButton(onClick = { currentPasswordVisible = !currentPasswordVisible }) {
                        Icon(
                            imageVector = if (currentPasswordVisible) Icons.Default.Visibility
                            else Icons.Default.VisibilityOff,
                            contentDescription = if (currentPasswordVisible) "Şifreyi gizle"
                            else "Şifreyi göster"
                        )
                    }
                },
                visualTransformation = if (currentPasswordVisible) VisualTransformation.None
                else PasswordVisualTransformation(),
                keyboardOptions = KeyboardOptions(
                    keyboardType = KeyboardType.Password,
                    imeAction = ImeAction.Next
                ),
                keyboardActions = KeyboardActions(
                    onNext = { focusManager.moveFocus(FocusDirection.Down) }
                ),
                singleLine = true,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                enabled = !changePasswordState.isLoading
            )

            // New Password Field
            OutlinedTextField(
                value = newPassword,
                onValueChange = { 
                    newPassword = it
                    // Şifre validasyonu
                    validatePassword(it) { isValid, message ->
                        passwordValidationError = if (isValid) null else message
                    }
                },
                label = { Text("Yeni Şifre") },
                leadingIcon = {
                    Icon(Icons.Default.Lock, contentDescription = null)
                },
                trailingIcon = {
                    IconButton(onClick = { newPasswordVisible = !newPasswordVisible }) {
                        Icon(
                            imageVector = if (newPasswordVisible) Icons.Default.Visibility
                            else Icons.Default.VisibilityOff,
                            contentDescription = if (newPasswordVisible) "Şifreyi gizle"
                            else "Şifreyi göster"
                        )
                    }
                },
                visualTransformation = if (newPasswordVisible) VisualTransformation.None
                else PasswordVisualTransformation(),
                keyboardOptions = KeyboardOptions(
                    keyboardType = KeyboardType.Password,
                    imeAction = ImeAction.Next
                ),
                keyboardActions = KeyboardActions(
                    onNext = { focusManager.moveFocus(FocusDirection.Down) }
                ),
                singleLine = true,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                enabled = !changePasswordState.isLoading,
                isError = passwordValidationError != null
            )

            // Şifre validasyonu hatası göster
            if (passwordValidationError != null) {
                Text(
                    text = passwordValidationError!!,
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodySmall,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp)
                )
            }

            // Confirm New Password Field
            OutlinedTextField(
                value = confirmNewPassword,
                onValueChange = { 
                    confirmNewPassword = it
                    // Şifreler eşleşmiyorsa uyarı göster
                    passwordsMatch = it == newPassword || it.isEmpty()
                },
                label = { Text("Yeni Şifre Tekrar") },
                leadingIcon = {
                    Icon(Icons.Default.Lock, contentDescription = null)
                },
                trailingIcon = {
                    IconButton(onClick = { confirmNewPasswordVisible = !confirmNewPasswordVisible }) {
                        Icon(
                            imageVector = if (confirmNewPasswordVisible) Icons.Default.Visibility
                            else Icons.Default.VisibilityOff,
                            contentDescription = if (confirmNewPasswordVisible) "Şifreyi gizle"
                            else "Şifreyi göster"
                        )
                    }
                },
                visualTransformation = if (confirmNewPasswordVisible) VisualTransformation.None
                else PasswordVisualTransformation(),
                keyboardOptions = KeyboardOptions(
                    keyboardType = KeyboardType.Password,
                    imeAction = ImeAction.Done
                ),
                keyboardActions = KeyboardActions(
                    onDone = {
                        focusManager.clearFocus()
                        if (currentPassword.isNotBlank() && newPassword.isNotBlank() && confirmNewPassword.isNotBlank() && newPassword == confirmNewPassword) {
                            viewModel.changePassword(currentPassword, newPassword)
                        }
                    }
                ),
                singleLine = true,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 8.dp),
                enabled = !changePasswordState.isLoading,
                isError = !passwordsMatch
            )

            // Şifreler eşleşmiyorsa hata mesajı göster
            if (!passwordsMatch) {
                Text(
                    text = "Şifreler eşleşmiyor",
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodySmall,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp)
                )
            }

            // Change Password Button
            Button(
                onClick = {
                    if (currentPassword.isNotBlank() && newPassword.isNotBlank() && confirmNewPassword.isNotBlank() && newPassword == confirmNewPassword) {
                        viewModel.changePassword(currentPassword, newPassword)
                    }
                },
                modifier = Modifier
                    .fillMaxWidth()
                    .height(50.dp),
                enabled = !changePasswordState.isLoading && 
                         currentPassword.isNotBlank() && 
                         newPassword.isNotBlank() && 
                         confirmNewPassword.isNotBlank() &&
                         newPassword == confirmNewPassword &&
                         passwordValidationError == null
            ) {
                if (changePasswordState.isLoading) {
                    CircularProgressIndicator(
                        modifier = Modifier.size(24.dp),
                        color = MaterialTheme.colorScheme.onPrimary
                    )
                } else {
                    Text("Şifreyi Değiştir")
                }
            }
        }
    }
}

// Şifre validasyonu fonksiyonu
fun validatePassword(password: String, onResult: (Boolean, String) -> Unit) {
    // Şifre uzunluğu kontrolü
    if (password.length < 6) {
        onResult(false, "Şifre en az 6 karakter uzunluğunda olmalıdır")
        return
    }
    
    // Küçük harf kontrolü
    if (!password.any { it.isLowerCase() }) {
        onResult(false, "Şifre en az bir küçük harf içermelidir")
        return
    }
    
    // Büyük harf kontrolü
    if (!password.any { it.isUpperCase() }) {
        onResult(false, "Şifre en az bir büyük harf içermelidir")
        return
    }
    
    // Rakam kontrolü
    if (!password.any { it.isDigit() }) {
        onResult(false, "Şifre en az bir rakam içermelidir")
        return
    }
    
    // Özel karakter kontrolü
    val specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?"
    if (!password.any { it in specialChars }) {
        onResult(false, "Şifre en az bir özel karakter içermelidir (!@#$%^&*()_+-=[]{}|;':\",./<>?)")
        return
    }
    
    // Tüm kontrollerden geçti
    onResult(true, "")
}
