package com.example.vtys.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.vtys.common.Resource
import com.example.vtys.domain.model.Module
import com.example.vtys.domain.model.ProjectDetail
import com.example.vtys.domain.model.ProjectMember
import com.example.vtys.domain.repository.ModuleRepository
import com.example.vtys.domain.repository.ProjectRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class ProjectDetailState(
    val isLoading: Boolean = false,
    val project: ProjectDetail? = null,
    val modules: List<Module> = emptyList(),
    val members: List<ProjectMember> = emptyList(),
    val error: String? = null
)

class ProjectDetailViewModel(
    private val projectRepository: ProjectRepository,
    private val moduleRepository: ModuleRepository
) : ViewModel() {

    private val _state = MutableStateFlow(ProjectDetailState())
    val state: StateFlow<ProjectDetailState> = _state.asStateFlow()

    fun loadProjectDetails(projectId: String) {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)

            // Load Project Detail
            val projectResult = projectRepository.getProjectById(projectId)
            if (projectResult is Resource.Error) {
                _state.value = _state.value.copy(isLoading = false, error = projectResult.message)
                return@launch
            }

            // Load Modules
            val modulesResult = moduleRepository.getModulesByProject(projectId, 1, 20, null, null)
            
            // Load Members
            val membersResult = projectRepository.getProjectMembers(projectId)

            _state.value = _state.value.copy(
                isLoading = false,
                project = projectResult.data,
                modules = modulesResult.data?.items ?: emptyList(),
                members = membersResult.data ?: emptyList()
            )
        }
    }
}
