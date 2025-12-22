package com.te4it.mobile.presentation.screens

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.te4it.mobile.presentation.viewmodel.TaskListViewModel
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TaskListScreen(
    viewModelFactory: ViewModelFactory,
    onNavigateBack: () -> Unit,
    onNavigateToTaskDetail: (String) -> Unit
) {
    val viewModel: TaskListViewModel = viewModel(factory = viewModelFactory)
    val state by viewModel.state.collectAsState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Görevlerim") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                }
            )
        }
    ) { paddingValues ->
        Column(modifier = Modifier.padding(paddingValues)) {
            // Search Bar
            OutlinedTextField(
                value = state.searchQuery,
                onValueChange = { viewModel.onSearchQueryChange(it) },
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                placeholder = { Text("Görev Ara...") },
                leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                singleLine = true
            )

            // Filter Chips (Simplified)
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(horizontal = 16.dp),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                FilterChip(
                    selected = state.isCompletedFilter == null,
                    onClick = { viewModel.onFilterChange(null) },
                    label = { Text("Tümü") }
                )
                FilterChip(
                    selected = state.isCompletedFilter == true,
                    onClick = { viewModel.onFilterChange(true) },
                    label = { Text("Tamamlanan") }
                )
                FilterChip(
                    selected = state.isCompletedFilter == false,
                    onClick = { viewModel.onFilterChange(false) },
                    label = { Text("Devam Eden") }
                )
            }

            Spacer(modifier = Modifier.height(8.dp))

            if (state.isLoading) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            } else if (state.error != null) {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text(text = state.error!!, color = MaterialTheme.colorScheme.error)
                }
            } else {
                LazyColumn(
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    items(state.tasks) { task ->
                        // Reusing DashboardTaskCard or creating a new one. 
                        // Since DashboardTaskCard is private in DashboardScreen, we should probably make it public or duplicate/refactor.
                        // For now, let's assume we can use a generic TaskCard if available, or I'll define a local one to avoid dependency issues.
                        // Actually, TaskDetailScreen has a TaskCard, but it might be private too.
                        // Let's define a simple TaskListItem here.
                        TaskListItem(task = task, onClick = { onNavigateToTaskDetail(task.id) })
                    }
                }
            }
        }
    }
}

@Composable
fun TaskListItem(
    task: com.te4it.mobile.domain.model.Task,
    onClick: () -> Unit
) {
    Card(
        onClick = onClick,
        modifier = Modifier.fillMaxWidth(),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(
            modifier = Modifier
                .padding(16.dp)
                .fillMaxWidth()
        ) {
            Text(
                text = task.title,
                style = MaterialTheme.typography.titleMedium
            )
            Spacer(modifier = Modifier.height(4.dp))
            Text(
                text = task.description ?: "",
                style = MaterialTheme.typography.bodyMedium,
                maxLines = 2
            )
            Spacer(modifier = Modifier.height(8.dp))
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text(
                    text = task.taskState?.name ?: "Bilinmiyor", // Should localize this later
                    style = MaterialTheme.typography.labelSmall,
                    color = MaterialTheme.colorScheme.primary
                )
                if (task.dueDate != null) {
                    Text(
                        text = task.dueDate ?: "",
                        style = MaterialTheme.typography.labelSmall,
                        color = MaterialTheme.colorScheme.secondary
                    )
                }
            }
        }
    }
}
