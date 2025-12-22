package com.te4it.mobile.presentation.screens

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.te4it.mobile.data.network.dto.TaskState
import com.te4it.mobile.domain.model.Task
import com.te4it.mobile.presentation.viewmodel.UseCaseDetailViewModel
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun UseCaseDetailScreen(
    useCaseId: String,
    viewModelFactory: ViewModelFactory,
    onNavigateBack: () -> Unit,
    onNavigateToTaskDetail: (String) -> Unit
) {
    val viewModel: UseCaseDetailViewModel = viewModel(factory = viewModelFactory)
    val state by viewModel.state.collectAsState()

    LaunchedEffect(useCaseId) {
        viewModel.loadUseCaseDetails(useCaseId)
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(state.useCase?.title ?: "Use Case Detay") },
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
            } else if (state.error != null) {
                Text(
                    text = state.error!!,
                    color = MaterialTheme.colorScheme.error,
                    modifier = Modifier.align(Alignment.Center)
                )
            } else {
                state.useCase?.let { useCase ->
                    LazyColumn(
                        contentPadding = PaddingValues(16.dp),
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        item {
                            Text(text = useCase.description ?: "Açıklama yok", style = MaterialTheme.typography.bodyLarge)
                            if (!useCase.importantNotes.isNullOrBlank()) {
                                Spacer(modifier = Modifier.height(8.dp))
                                Card(
                                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.tertiaryContainer)
                                ) {
                                    Column(modifier = Modifier.padding(16.dp)) {
                                        Text("Önemli Notlar", fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.onTertiaryContainer)
                                        Text(useCase.importantNotes, color = MaterialTheme.colorScheme.onTertiaryContainer)
                                    }
                                }
                            }
                        }

                        item {
                            Text("Tasks", style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold)
                        }

                        items(state.tasks) { task ->
                            TaskCard(task = task, onClick = { onNavigateToTaskDetail(task.id) })
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun TaskCard(task: Task, onClick: () -> Unit) {
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
                    text = task.title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.weight(1f)
                )
                TaskStateChip(state = task.taskState)
            }
            Spacer(modifier = Modifier.height(4.dp))
            Text(
                text = task.description ?: "Açıklama yok",
                style = MaterialTheme.typography.bodyMedium,
                maxLines = 2
            )
            Spacer(modifier = Modifier.height(8.dp))
            Row(horizontalArrangement = Arrangement.SpaceBetween, modifier = Modifier.fillMaxWidth()) {
                Text(task.assigneeName ?: "Atanmamış", style = MaterialTheme.typography.bodySmall)
                Text(task.taskType?.name ?: "Diğer", style = MaterialTheme.typography.bodySmall)
            }
        }
    }
}

@Composable
fun TaskStateChip(state: TaskState?) {
    val (color, text) = when (state) {
        TaskState.NOT_STARTED -> Color.Gray to "Başlamadı"
        TaskState.IN_PROGRESS -> Color.Blue to "Devam Ediyor"
        TaskState.COMPLETED -> Color(0xFF2E7D32) to "Tamamlandı"
        TaskState.CANCELLED -> Color.Red to "İptal Edildi"
        null -> Color.Gray to "Bilinmiyor"
    }

    Surface(
        color = color.copy(alpha = 0.1f),
        shape = MaterialTheme.shapes.small
    ) {
        Text(
            text = text,
            color = color,
            style = MaterialTheme.typography.labelSmall,
            modifier = Modifier.padding(horizontal = 8.dp, vertical = 4.dp)
        )
    }
}
