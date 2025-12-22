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
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.te4it.mobile.domain.model.UseCase
import com.te4it.mobile.presentation.viewmodel.ModuleDetailViewModel
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ModuleDetailScreen(
    moduleId: String,
    viewModelFactory: ViewModelFactory,
    onNavigateBack: () -> Unit,
    onNavigateToUseCaseDetail: (String) -> Unit
) {
    val viewModel: ModuleDetailViewModel = viewModel(factory = viewModelFactory)
    val state by viewModel.state.collectAsState()

    LaunchedEffect(moduleId) {
        viewModel.loadModuleDetails(moduleId)
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(state.module?.title ?: "Modül Detay") },
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
                state.module?.let { module ->
                    LazyColumn(
                        contentPadding = PaddingValues(16.dp),
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        item {
                            Text(text = module.description ?: "Açıklama yok", style = MaterialTheme.typography.bodyLarge)
                        }

                        item {
                            Text("Use Cases", style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold)
                        }

                        items(state.useCases) { useCase ->
                            UseCaseCard(useCase = useCase, onClick = { onNavigateToUseCaseDetail(useCase.id) })
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun UseCaseCard(useCase: UseCase, onClick: () -> Unit) {
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
                    text = useCase.title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold
                )
                if (!useCase.isActive) {
                    Text("Pasif", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.labelSmall)
                }
            }
            Spacer(modifier = Modifier.height(4.dp))
            Text(
                text = useCase.description ?: "Açıklama yok",
                style = MaterialTheme.typography.bodyMedium,
                maxLines = 2
            )
            Spacer(modifier = Modifier.height(8.dp))
            Text("${useCase.taskCount} Task", style = MaterialTheme.typography.bodySmall)
        }
    }
}
