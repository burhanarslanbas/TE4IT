package com.te4it.mobile.presentation.screens

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Check
import androidx.compose.material.icons.filled.Link
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.te4it.mobile.data.network.dto.TaskState
import com.te4it.mobile.presentation.viewmodel.TaskDetailViewModel
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory

import androidx.compose.foundation.clickable
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskDetailScreen(
    taskId: String,
    viewModelFactory: ViewModelFactory,
    onNavigateBack: () -> Unit
) {
    val viewModel: TaskDetailViewModel = viewModel(factory = viewModelFactory)
    val state by viewModel.state.collectAsState()
    var showStatusSheet by remember { mutableStateOf(false) }
    val sheetState = rememberModalBottomSheetState()
    val scope = rememberCoroutineScope()
    val context = androidx.compose.ui.platform.LocalContext.current

    LaunchedEffect(taskId) {
        viewModel.loadTaskDetails(taskId)
    }

    LaunchedEffect(state.error) {
        state.error?.let { error ->
            android.widget.Toast.makeText(context, error, android.widget.Toast.LENGTH_LONG).show()
        }
    }

    if (showStatusSheet) {
        ModalBottomSheet(
            onDismissRequest = { showStatusSheet = false },
            sheetState = sheetState
        ) {
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text(
                    "Durum Değiştir",
                    style = MaterialTheme.typography.titleLarge,
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.padding(bottom = 16.dp)
                )
                
                    TaskState.entries.forEach { taskState ->
                    val isSelected = state.task?.taskState == taskState
                    Card(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(vertical = 4.dp)
                            .clickable {
                                viewModel.updateTaskState(taskState)
                                scope.launch { sheetState.hide() }.invokeOnCompletion {
                                    if (!sheetState.isVisible) {
                                        showStatusSheet = false
                                    }
                                }
                            },
                        colors = CardDefaults.cardColors(
                            containerColor = if (isSelected) MaterialTheme.colorScheme.primaryContainer else MaterialTheme.colorScheme.surface
                        ),
                        border = if (isSelected) null else androidx.compose.foundation.BorderStroke(1.dp, MaterialTheme.colorScheme.outlineVariant)
                    ) {
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(16.dp),
                            verticalAlignment = Alignment.CenterVertically,
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text(
                                text = getTaskStateLabel(taskState),
                                style = MaterialTheme.typography.bodyLarge,
                                fontWeight = if (isSelected) FontWeight.Bold else FontWeight.Normal
                            )
                            if (isSelected) {
                                Icon(Icons.Default.Check, contentDescription = "Selected")
                            }
                        }
                    }
                }
                Spacer(modifier = Modifier.height(32.dp))
            }
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Task Detay") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                }
            )
        }
    ) { paddingValues ->
        Box(modifier = Modifier.padding(paddingValues)) {
            if (state.isLoading) {
                CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
            } else {
                state.task?.let { task ->
                    LazyColumn(
                        contentPadding = PaddingValues(16.dp),
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        item {
                            Text(text = task.title, style = MaterialTheme.typography.headlineSmall, fontWeight = FontWeight.Bold)
                            Spacer(modifier = Modifier.height(8.dp))
                            Row(verticalAlignment = Alignment.CenterVertically) {
                                TaskStateChip(state = task.taskState)
                            }
                        }

                        item {
                            Button(
                                onClick = { showStatusSheet = true },
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                Text("Durumu Güncelle")
                            }
                        }

                        item {
                            Text("Açıklama", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
                            Text(text = task.description ?: "Açıklama yok", style = MaterialTheme.typography.bodyLarge)
                        }

                        if (!task.importantNotes.isNullOrBlank()) {
                            item {
                                Card(
                                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.tertiaryContainer)
                                ) {
                                    Column(modifier = Modifier.padding(16.dp)) {
                                        Text("Önemli Notlar", fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.onTertiaryContainer)
                                        Text(task.importantNotes, color = MaterialTheme.colorScheme.onTertiaryContainer)
                                    }
                                }
                            }
                        }

                        item {
                            Card(
                                modifier = Modifier.fillMaxWidth(),
                                colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.3f))
                            ) {
                                Column(modifier = Modifier.padding(16.dp)) {
                                    DetailRow("Atanan", task.assigneeName ?: "Atanmamış")
                                    DetailRow("Tip", task.taskType?.name ?: "Bilinmiyor")
                                    DetailRow("Başlangıç", task.startedDate?.take(10) ?: "-")
                                    DetailRow("Bitiş", task.dueDate?.take(10) ?: "-")
                                }
                            }
                        }

                        if (state.relations.isNotEmpty()) {
                            item {
                                Text("İlişkiler", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
                            }
                            items(state.relations) { relation ->
                                ListItem(
                                    headlineContent = { Text(relation.targetTaskTitle) },
                                    supportingContent = { Text(relation.relationType?.name ?: "İlişkili") },
                                    leadingContent = { Icon(Icons.Default.Link, contentDescription = null) }
                                )
                            }
                        }
                    }
                }
            }
            
            if (state.isUpdating) {
                CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
            }
        }
    }
}

@Composable
fun DetailRow(label: String, value: String) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp),
        horizontalArrangement = Arrangement.SpaceBetween
    ) {
        Text(label, style = MaterialTheme.typography.bodyMedium, color = MaterialTheme.colorScheme.onSurfaceVariant)
        Text(value, style = MaterialTheme.typography.bodyMedium, fontWeight = FontWeight.Medium)
    }
}

fun getTaskStateLabel(state: TaskState): String {
    return when (state) {
        TaskState.NOT_STARTED -> "Başlamadı"
        TaskState.IN_PROGRESS -> "Devam Ediyor"
        TaskState.COMPLETED -> "Tamamlandı"
        TaskState.CANCELLED -> "İptal Edildi"
    }
}
