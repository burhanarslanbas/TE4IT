package com.te4it.mobile.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.te4it.mobile.common.Resource
import com.te4it.mobile.domain.model.Project
import com.te4it.mobile.domain.repository.ProjectRepository
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class ProjectListState(
    val isLoading: Boolean = false,
    val projects: List<Project> = emptyList(),
    val error: String? = null,
    val searchQuery: String = "",
    val isActiveFilter: Boolean? = null
)

class ProjectListViewModel(
    private val projectRepository: ProjectRepository
) : ViewModel() {

    private val _state = MutableStateFlow(ProjectListState())
    val state: StateFlow<ProjectListState> = _state.asStateFlow()

    private var searchJob: Job? = null

    init {
        loadProjects()
    }

    fun loadProjects() {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)
            val result = projectRepository.getProjects(
                page = 1,
                pageSize = 20,
                search = _state.value.searchQuery.takeIf { it.isNotBlank() },
                isActive = _state.value.isActiveFilter
            )

            when (result) {
                is Resource.Success -> {
                    _state.value = _state.value.copy(
                        isLoading = false,
                        projects = result.data?.items ?: emptyList()
                    )
                }
                is Resource.Error -> {
                    _state.value = _state.value.copy(
                        isLoading = false,
                        error = result.message
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
            loadProjects()
        }
    }

    fun onFilterChange(isActive: Boolean?) {
        _state.value = _state.value.copy(isActiveFilter = isActive)
        loadProjects()
    }
}
