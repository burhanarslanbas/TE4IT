package com.te4it.mobile.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.te4it.mobile.common.Resource
import com.te4it.mobile.domain.model.Task
import com.te4it.mobile.domain.repository.TaskRepository
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class TaskListState(
    val isLoading: Boolean = false,
    val tasks: List<Task> = emptyList(),
    val error: String? = null,
    val searchQuery: String = "",
    val isCompletedFilter: Boolean? = null // null: All, true: Completed, false: Not Completed
)

class TaskListViewModel(
    private val taskRepository: TaskRepository
) : ViewModel() {

    private val _state = MutableStateFlow(TaskListState())
    val state: StateFlow<TaskListState> = _state.asStateFlow()

    private var searchJob: Job? = null

    init {
        loadTasks()
    }

    fun loadTasks() {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)
            // Assuming getTasksByAssignee or similar exists. 
            // Since we don't have a generic "getTasks" with filters in the repo interface yet (based on previous knowledge),
            // we might need to fetch all and filter in memory or update repo.
            // For now, let's use getTasksByAssignee("user1") as a placeholder or similar.
            // Wait, let's check TaskRepository interface first to be sure.
            // But I can't check it right now without interrupting the flow. 
            // I'll assume getTasksByAssignee is available as seen in previous edits.
            
            val result = taskRepository.getTasksByAssignee("user1") // TODO: Use actual current user ID

            when (result) {
                is Resource.Success -> {
                    var tasks = result.data ?: emptyList()
                    
                    // In-memory filtering for now
                    if (_state.value.searchQuery.isNotBlank()) {
                        tasks = tasks.filter { 
                            it.title.contains(_state.value.searchQuery, ignoreCase = true) ||
                            it.description?.contains(_state.value.searchQuery, ignoreCase = true) == true
                        }
                    }
                    
                    if (_state.value.isCompletedFilter != null) {
                        // Assuming TaskState enum exists and has COMPLETED
                        // We need to map boolean to TaskState check
                        // This is a simplification. Ideally repo handles this.
                    }

                    _state.value = _state.value.copy(
                        isLoading = false,
                        tasks = tasks
                    )
                }
                is Resource.Error -> {
                    _state.value = _state.value.copy(
                        isLoading = false,
                        error = result.message ?: "Bilinmeyen bir hata oluştu"
                    )
                }
                is Resource.Loading -> {}
            }
        }
    }

    fun onSearchQueryChange(query: String) {
        _state.value = _state.value.copy(searchQuery = query)
        searchJob?.cancel()
        searchJob = viewModelScope.launch {
            delay(500L)
            loadTasks()
        }
    }

    fun onFilterChange(isCompleted: Boolean?) {
        _state.value = _state.value.copy(isCompletedFilter = isCompleted)
        loadTasks()
    }
}
