package com.te4it.mobile.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.te4it.mobile.common.Resource
import com.te4it.mobile.data.network.dto.TaskState
import com.te4it.mobile.domain.model.Task
import com.te4it.mobile.domain.model.TaskRelation
import com.te4it.mobile.domain.repository.TaskRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class TaskDetailState(
    val isLoading: Boolean = false,
    val task: Task? = null,
    val relations: List<TaskRelation> = emptyList(),
    val error: String? = null,
    val isUpdating: Boolean = false
)

class TaskDetailViewModel(
    private val taskRepository: TaskRepository
) : ViewModel() {

    private val _state = MutableStateFlow(TaskDetailState())
    val state: StateFlow<TaskDetailState> = _state.asStateFlow()

    fun loadTaskDetails(taskId: String) {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)

            val taskResult = taskRepository.getTaskById(taskId)
            if (taskResult is Resource.Error) {
                _state.value = _state.value.copy(isLoading = false, error = taskResult.message)
                return@launch
            }

            val relationsResult = taskRepository.getTaskRelations(taskId)

            _state.value = _state.value.copy(
                isLoading = false,
                task = taskResult.data,
                relations = relationsResult.data ?: emptyList()
            )
        }
    }

    fun updateTaskState(newState: TaskState) {
        val currentTask = _state.value.task ?: return
        viewModelScope.launch {
            _state.value = _state.value.copy(isUpdating = true)
            val result = taskRepository.changeTaskState(currentTask.id, newState.value)
            
            when (result) {
                is Resource.Success -> {
                    // Update local state
                    _state.value = _state.value.copy(
                        isUpdating = false,
                        task = currentTask.copy(taskState = newState)
                    )
                }
                is Resource.Error -> {
                    _state.value = _state.value.copy(
                        isUpdating = false,
                        error = result.message
                    )
                }
                is Resource.Loading -> {}
            }
        }
    }
}
