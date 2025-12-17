package com.example.vtys.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.vtys.common.Resource
import com.example.vtys.domain.model.Module
import com.example.vtys.domain.model.UseCase
import com.example.vtys.domain.repository.ModuleRepository
import com.example.vtys.domain.repository.UseCaseRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class ModuleDetailState(
    val isLoading: Boolean = false,
    val module: Module? = null,
    val useCases: List<UseCase> = emptyList(),
    val error: String? = null
)

class ModuleDetailViewModel(
    private val moduleRepository: ModuleRepository,
    private val useCaseRepository: UseCaseRepository
) : ViewModel() {

    private val _state = MutableStateFlow(ModuleDetailState())
    val state: StateFlow<ModuleDetailState> = _state.asStateFlow()

    fun loadModuleDetails(moduleId: String) {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)

            val moduleResult = moduleRepository.getModuleById(moduleId)
            if (moduleResult is Resource.Error) {
                _state.value = _state.value.copy(isLoading = false, error = moduleResult.message)
                return@launch
            }

            val useCasesResult = useCaseRepository.getUseCasesByModule(moduleId, 1, 20, null, null)

            _state.value = _state.value.copy(
                isLoading = false,
                module = moduleResult.data,
                useCases = useCasesResult.data?.items ?: emptyList()
            )
        }
    }
}
