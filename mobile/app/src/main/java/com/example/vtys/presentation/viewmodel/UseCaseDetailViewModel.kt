package com.example.vtys.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.vtys.common.Resource
import com.example.vtys.domain.model.Task
import com.example.vtys.domain.model.UseCase
import com.example.vtys.domain.repository.TaskRepository
import com.example.vtys.domain.repository.UseCaseRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class UseCaseDetailState(
    val isLoading: Boolean = false,
    val useCase: UseCase? = null,
    val tasks: List<Task> = emptyList(),
    val error: String? = null
)

class UseCaseDetailViewModel(
    private val useCaseRepository: UseCaseRepository,
    private val taskRepository: TaskRepository
) : ViewModel() {

    private val _state = MutableStateFlow(UseCaseDetailState())
    val state: StateFlow<UseCaseDetailState> = _state.asStateFlow()

    fun loadUseCaseDetails(useCaseId: String) {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)

            val useCaseResult = useCaseRepository.getUseCaseById(useCaseId)
            if (useCaseResult is Resource.Error) {
                _state.value = _state.value.copy(isLoading = false, error = useCaseResult.message)
                return@launch
            }

            val tasksResult = taskRepository.getTasksByUseCase(useCaseId, 1, 20, null, null, null, null, null, null)

            _state.value = _state.value.copy(
                isLoading = false,
                useCase = useCaseResult.data,
                tasks = tasksResult.data?.items ?: emptyList()
            )
        }
    }
}
