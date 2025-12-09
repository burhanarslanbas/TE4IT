package com.example.vtys.presentation.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import java.text.SimpleDateFormat
import java.util.*
import kotlinx.coroutines.launch
import androidx.compose.foundation.shape.CircleShape

enum class NotificationType {
    TASK_ASSIGNMENT,
    PROJECT_UPDATE,
    TASK_COMPLETED,
    SYSTEM_ALERT,
    INFO
}

data class NotificationItem(
    val id: String,
    val type: NotificationType,
    val title: String,
    val message: String,
    val timestamp: Long,
    var isRead: Boolean
)

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun NotificationScreen(
    onNavigateBack: () -> Unit
) {
    // Mock Data State
    val notifications = remember {
        mutableStateListOf(
            NotificationItem(
                id = "1",
                type = NotificationType.TASK_ASSIGNMENT,
                title = "Yeni Görev Atandı",
                message = "Login Ekranı Tasarımı görevi size atandı.",
                timestamp = System.currentTimeMillis() - 1000 * 60 * 30, // 30 dk önce
                isRead = false
            ),
            NotificationItem(
                id = "2",
                type = NotificationType.PROJECT_UPDATE,
                title = "Proje Güncellemesi",
                message = "E-Ticaret Platformu projesine yeni bir modül eklendi.",
                timestamp = System.currentTimeMillis() - 1000 * 60 * 60 * 2, // 2 saat önce
                isRead = true
            ),
            NotificationItem(
                id = "3",
                type = NotificationType.TASK_COMPLETED,
                title = "Görev Tamamlandı",
                message = "JWT Entegrasyonu görevi tamamlandı olarak işaretlendi.",
                timestamp = System.currentTimeMillis() - 1000 * 60 * 60 * 24, // 1 gün önce
                isRead = true
            ),
            NotificationItem(
                id = "4",
                type = NotificationType.SYSTEM_ALERT,
                title = "Sistem Bakımı",
                message = "Yarın gece 02:00 - 04:00 arası sistem bakımı yapılacaktır.",
                timestamp = System.currentTimeMillis() - 1000 * 60 * 60 * 48, // 2 gün önce
                isRead = false
            ),
             NotificationItem(
                id = "5",
                type = NotificationType.INFO,
                title = "Hoşgeldiniz",
                message = "Uygulamamıza hoşgeldiniz! Profilinizi tamamlamayı unutmayın.",
                timestamp = System.currentTimeMillis() - 1000 * 60 * 60 * 120, // 5 gün önce
                isRead = true
            )
        )
    }

    var selectedTabIndex by remember { mutableIntStateOf(0) }
    val tabs = listOf("Tümü", "Okunmamış")
    val snackbarHostState = remember { SnackbarHostState() }
    val scope = rememberCoroutineScope()

    // Filtered List
    val filteredNotifications = when (selectedTabIndex) {
        0 -> notifications
        1 -> notifications.filter { !it.isRead }
        else -> notifications
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Bildirimler") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Geri")
                    }
                },
                actions = {
                    if (notifications.any { !it.isRead }) {
                        IconButton(onClick = {
                            notifications.forEach { it.isRead = true }
                            // Force recomposition hack if needed, but mutableStateListOf elements update should trigger
                            // Actually for object properties inside list, we might need to replace elements or use a class with MutableState properties
                            // For simplicity in this mock, let's just replace the list content
                            val updated = notifications.map { it.copy(isRead = true) }
                            notifications.clear()
                            notifications.addAll(updated)
                            
                            scope.launch {
                                snackbarHostState.showSnackbar("Tüm bildirimler okundu olarak işaretlendi")
                            }
                        }) {
                            Icon(Icons.Default.DoneAll, contentDescription = "Tümünü Okundu İşaretle")
                        }
                    }
                    IconButton(onClick = {
                        notifications.clear()
                        scope.launch {
                            snackbarHostState.showSnackbar("Tüm bildirimler silindi")
                        }
                    }) {
                        Icon(Icons.Default.DeleteSweep, contentDescription = "Tümünü Sil")
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
        ) {
            TabRow(selectedTabIndex = selectedTabIndex) {
                tabs.forEachIndexed { index, title ->
                    Tab(
                        selected = selectedTabIndex == index,
                        onClick = { selectedTabIndex = index },
                        text = { 
                            Row(verticalAlignment = Alignment.CenterVertically) {
                                Text(title)
                                if (index == 1) {
                                    val unreadCount = notifications.count { !it.isRead }
                                    if (unreadCount > 0) {
                                        Spacer(modifier = Modifier.width(4.dp))
                                        Badge { Text(unreadCount.toString()) }
                                    }
                                }
                            }
                        }
                    )
                }
            }

            if (filteredNotifications.isEmpty()) {
                Box(
                    modifier = Modifier.fillMaxSize(),
                    contentAlignment = Alignment.Center
                ) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Icon(
                            Icons.Default.NotificationsOff,
                            contentDescription = null,
                            modifier = Modifier.size(64.dp),
                            tint = MaterialTheme.colorScheme.outline
                        )
                        Spacer(modifier = Modifier.height(16.dp))
                        Text(
                            text = "Bildirim Yok",
                            style = MaterialTheme.typography.titleMedium,
                            color = MaterialTheme.colorScheme.outline
                        )
                    }
                }
            } else {
                LazyColumn(
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    items(filteredNotifications, key = { it.id }) { notification ->
                        NotificationCard(
                            notification = notification,
                            onDelete = {
                                notifications.remove(notification)
                            },
                            onMarkAsRead = {
                                val index = notifications.indexOf(notification)
                                if (index != -1) {
                                    notifications[index] = notification.copy(isRead = true)
                                }
                            }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun NotificationCard(
    notification: NotificationItem,
    onDelete: () -> Unit,
    onMarkAsRead: () -> Unit
) {
    val icon = when (notification.type) {
        NotificationType.TASK_ASSIGNMENT -> Icons.Default.AssignmentInd
        NotificationType.PROJECT_UPDATE -> Icons.Default.Update
        NotificationType.TASK_COMPLETED -> Icons.Default.TaskAlt
        NotificationType.SYSTEM_ALERT -> Icons.Default.Warning
        NotificationType.INFO -> Icons.Default.Info
    }

    val iconColor = when (notification.type) {
        NotificationType.TASK_ASSIGNMENT -> Color(0xFF1E88E5)
        NotificationType.PROJECT_UPDATE -> Color(0xFF43A047)
        NotificationType.TASK_COMPLETED -> Color(0xFF00ACC1)
        NotificationType.SYSTEM_ALERT -> Color(0xFFE53935)
        NotificationType.INFO -> Color(0xFFFB8C00)
    }

    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable { if (!notification.isRead) onMarkAsRead() },
        colors = CardDefaults.cardColors(
            containerColor = if (notification.isRead) MaterialTheme.colorScheme.surface else MaterialTheme.colorScheme.primaryContainer.copy(alpha = 0.1f)
        ),
        elevation = CardDefaults.cardElevation(defaultElevation = if (notification.isRead) 1.dp else 4.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            verticalAlignment = Alignment.Top
        ) {
            Box(
                modifier = Modifier
                    .size(40.dp)
                    .background(iconColor.copy(alpha = 0.1f), CircleShape),
                contentAlignment = Alignment.Center
            ) {
                Icon(
                    imageVector = icon,
                    contentDescription = null,
                    tint = iconColor,
                    modifier = Modifier.size(24.dp)
                )
            }
            
            Spacer(modifier = Modifier.width(16.dp))
            
            Column(modifier = Modifier.weight(1f)) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = notification.title,
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = if (notification.isRead) FontWeight.Normal else FontWeight.Bold,
                        maxLines = 1,
                        overflow = TextOverflow.Ellipsis
                    )
                    if (!notification.isRead) {
                        Box(
                            modifier = Modifier
                                .size(8.dp)
                                .background(MaterialTheme.colorScheme.primary, CircleShape)
                        )
                    }
                }
                
                Spacer(modifier = Modifier.height(4.dp))
                
                Text(
                    text = notification.message,
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.8f),
                    maxLines = 3,
                    overflow = TextOverflow.Ellipsis
                )
                
                Spacer(modifier = Modifier.height(8.dp))
                
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = formatTimestamp(notification.timestamp),
                        style = MaterialTheme.typography.labelSmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                    
                    IconButton(
                        onClick = onDelete,
                        modifier = Modifier.size(24.dp)
                    ) {
                        Icon(
                            Icons.Default.Close, 
                            contentDescription = "Sil",
                            tint = MaterialTheme.colorScheme.onSurfaceVariant,
                            modifier = Modifier.size(16.dp)
                        )
                    }
                }
            }
        }
    }
}

fun formatTimestamp(timestamp: Long): String {
    val sdf = SimpleDateFormat("dd MMM HH:mm", Locale("tr", "TR"))
    return sdf.format(Date(timestamp))
}
