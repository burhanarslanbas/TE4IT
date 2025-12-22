package com.te4it.mobile.presentation.screens.profile

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.te4it.mobile.data.local.TokenManager
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SettingsScreen(
    navController: NavController
) {
    val context = LocalContext.current
    val tokenManager = remember { TokenManager(context) }
    val scope = rememberCoroutineScope()
    
    // State for settings (mocked for now, in real app would come from DataStore/ViewModel)
    var isDarkMode by remember { mutableStateOf(false) }
    var areNotificationsEnabled by remember { mutableStateOf(true) }
    var selectedLanguage by remember { mutableStateOf("Türkçe") }
    
    val snackbarHostState = remember { SnackbarHostState() }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Uygulama Ayarları") },
                navigationIcon = {
                    IconButton(onClick = { navController.popBackStack() }) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Geri")
                    }
                },
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
                .padding(16.dp)
        ) {
            // Görünüm Ayarları
            Text(
                text = "GÖRÜNÜM",
                style = MaterialTheme.typography.labelMedium,
                color = MaterialTheme.colorScheme.primary,
                fontWeight = FontWeight.Bold,
                modifier = Modifier.padding(vertical = 8.dp)
            )
            
            ListItem(
                headlineContent = { Text("Karanlık Mod") },
                supportingContent = { Text("Uygulamayı karanlık temada kullan") },
                trailingContent = {
                    Switch(
                        checked = isDarkMode,
                        onCheckedChange = { 
                            isDarkMode = it
                            scope.launch {
                                snackbarHostState.showSnackbar("Tema ayarı değiştirildi (Demo)")
                            }
                        }
                    )
                }
            )
            
            HorizontalDivider()
            
            // Bildirim Ayarları
            Text(
                text = "BİLDİRİMLER",
                style = MaterialTheme.typography.labelMedium,
                color = MaterialTheme.colorScheme.primary,
                fontWeight = FontWeight.Bold,
                modifier = Modifier.padding(top = 24.dp, bottom = 8.dp)
            )
            
            ListItem(
                headlineContent = { Text("Push Bildirimleri") },
                supportingContent = { Text("Önemli güncellemelerden haberdar ol") },
                trailingContent = {
                    Switch(
                        checked = areNotificationsEnabled,
                        onCheckedChange = { 
                            areNotificationsEnabled = it
                            scope.launch {
                                snackbarHostState.showSnackbar("Bildirim ayarı değiştirildi (Demo)")
                            }
                        }
                    )
                }
            )
            
            HorizontalDivider()
            
            // Genel Ayarlar
            Text(
                text = "GENEL",
                style = MaterialTheme.typography.labelMedium,
                color = MaterialTheme.colorScheme.primary,
                fontWeight = FontWeight.Bold,
                modifier = Modifier.padding(top = 24.dp, bottom = 8.dp)
            )
            
            ListItem(
                headlineContent = { Text("Dil") },
                supportingContent = { Text(selectedLanguage) },
                trailingContent = {
                    TextButton(onClick = { 
                        scope.launch {
                            snackbarHostState.showSnackbar("Dil seçimi yakında eklenecek")
                        }
                    }) {
                        Text("Değiştir")
                    }
                }
            )
        }
    }
}
