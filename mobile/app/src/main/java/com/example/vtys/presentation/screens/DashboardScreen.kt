package com.example.vtys.presentation.screens

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Notifications
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.vtys.domain.model.Project
import com.example.vtys.presentation.viewmodel.DashboardViewModel
import com.example.vtys.presentation.viewmodel.ViewModelFactory

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DashboardScreen(
    viewModelFactory: ViewModelFactory,
    onNavigateToProjectList: () -> Unit,
    onNavigateToProjectDetail: (String) -> Unit,
    onNavigateToTaskDetail: (String) -> Unit,
    onNavigateToProfile: () -> Unit,
    onNavigateToNotifications: () -> Unit
) {
    val viewModel: DashboardViewModel = viewModel(factory = viewModelFactory)
    val state by viewModel.state.collectAsState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Dashboard") },
                actions = {
                    IconButton(onClick = onNavigateToNotifications) {
                        Icon(Icons.Default.Notifications, contentDescription = "Notifications")
                    }
                    IconButton(onClick = onNavigateToProfile) {
                        Icon(Icons.Default.Person, contentDescription = "Profile")
                    }
                }
            )
        }
    ) { paddingValues ->
        Box(modifier = Modifier.padding(paddingValues)) {
            if (state.isLoading) {
                CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
            } else if (state.error != null) {
                Text(
                    text = state.error!!,
                    color = MaterialTheme.colorScheme.error,
                    modifier = Modifier.align(Alignment.Center)
                )
            } else {
                LazyColumn(
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    item {
                        SectionHeader(title = "Projelerim", onSeeAllClick = onNavigateToProjectList)
                    }

                    items(state.projects) { project ->
                        ProjectCard(project = project, onClick = { onNavigateToProjectDetail(project.id) })
                    }

                    item {
                        SectionHeader(title = "Görevlerim", onSeeAllClick = { /* TODO */ })
                    }
                    
                    if (state.tasks.isEmpty()) {
                        item {
                            Text("Henüz atanmış görev yok.", style = MaterialTheme.typography.bodyMedium)
                        }
                    } else {
                        items(state.tasks) { task ->
                            DashboardTaskCard(task = task, onClick = { onNavigateToTaskDetail(task.id) })
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun SectionHeader(title: String, onSeeAllClick: () -> Unit) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        Text(text = title, style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold)
        TextButton(onClick = onSeeAllClick) {
            Text("Tümü →")
        }
    }
}

@Composable
fun ProjectCard(project: Project, onClick: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable(onClick = onClick),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text(
                    text = project.title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold
                )
                StatusChip(isActive = project.isActive)
            }
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = project.description ?: "Açıklama yok",
                style = MaterialTheme.typography.bodyMedium,
                maxLines = 2
            )
            Spacer(modifier = Modifier.height(8.dp))
            Row(horizontalArrangement = Arrangement.spacedBy(16.dp)) {
                Text("${project.moduleCount} Modül", style = MaterialTheme.typography.bodySmall)
                Text("${project.memberCount} Üye", style = MaterialTheme.typography.bodySmall)
            }
        }
    }
}

@Composable
private fun DashboardTaskCard(task: com.example.vtys.domain.model.Task, onClick: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable(onClick = onClick),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f)
        )
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text(
                    text = task.title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.SemiBold
                )
                TaskStatusChip(state = task.taskState)
            }
            Spacer(modifier = Modifier.height(4.dp))
            Text(
                text = "Bitiş: ${task.dueDate?.take(10) ?: "Belirsiz"}",
                style = MaterialTheme.typography.bodySmall,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )
        }
    }
}

@Composable
fun StatusChip(isActive: Boolean) {
    Surface(
        color = if (isActive) Color(0xFFE8F5E9) else Color(0xFFFFEBEE),
        shape = MaterialTheme.shapes.small
    ) {
        Text(
            text = if (isActive) "Aktif" else "Pasif",
            color = if (isActive) Color(0xFF2E7D32) else Color(0xFFC62828),
            style = MaterialTheme.typography.labelSmall,
            modifier = Modifier.padding(horizontal = 8.dp, vertical = 4.dp)
        )
    }
}

@Composable
fun TaskStatusChip(state: com.example.vtys.data.network.dto.TaskState?) {
    val (color, text) = when (state) {
        com.example.vtys.data.network.dto.TaskState.COMPLETED -> Color(0xFFE8F5E9) to "Tamamlandı"
        com.example.vtys.data.network.dto.TaskState.IN_PROGRESS -> Color(0xFFE3F2FD) to "Devam Ediyor"
        com.example.vtys.data.network.dto.TaskState.NOT_STARTED -> Color(0xFFF5F5F5) to "Başlamadı"
        else -> Color.LightGray to "Bilinmiyor"
    }
    
    Surface(
        color = color,
        shape = MaterialTheme.shapes.small
    ) {
        Text(
            text = text,
            color = Color.Black.copy(alpha = 0.7f),
            style = MaterialTheme.typography.labelSmall,
            modifier = Modifier.padding(horizontal = 8.dp, vertical = 4.dp)
        )
    }
}
