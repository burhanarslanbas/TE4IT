package com.example.vtys.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.vtys.common.Resource
import com.example.vtys.domain.model.Project
import com.example.vtys.domain.model.Task
import com.example.vtys.domain.repository.ProjectRepository
import com.example.vtys.domain.repository.TaskRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class DashboardState(
    val isLoading: Boolean = false,
    val projects: List<Project> = emptyList(),
    val tasks: List<Task> = emptyList(),
    val error: String? = null
)

class DashboardViewModel(
    private val projectRepository: ProjectRepository,
    private val taskRepository: TaskRepository,
    private val tokenManager: com.example.vtys.data.local.TokenManager
) : ViewModel() {

    private val _state = MutableStateFlow(DashboardState())
    val state: StateFlow<DashboardState> = _state.asStateFlow()

    init {
        loadDashboardData()
    }

    fun loadDashboardData() {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)

            // Fetch projects
            val projectsResult = projectRepository.getProjects(1, 5, null, null)
            
            // Fetch tasks assigned to user
            val userId = tokenManager.getUserIdSync() // Assuming synchronous access or collect flow
            val tasksResult = if (userId != null) {
                taskRepository.getTasksByAssignee(userId)
            } else {
                Resource.Success(emptyList())
            }
            
            val currentProjects = if (projectsResult is Resource.Success) projectsResult.data?.items ?: emptyList() else emptyList()
            val currentTasks = if (tasksResult is Resource.Success) tasksResult.data ?: emptyList() else emptyList()
            val errorMsg = if (projectsResult is Resource.Error) projectsResult.message else if (tasksResult is Resource.Error) tasksResult.message else null

            _state.value = _state.value.copy(
                isLoading = false,
                projects = currentProjects,
                tasks = currentTasks,
                error = errorMsg
            )
        }
    }
}
