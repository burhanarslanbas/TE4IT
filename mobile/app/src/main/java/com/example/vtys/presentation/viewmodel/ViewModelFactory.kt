package com.example.vtys.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.vtys.domain.repository.ModuleRepository
import com.example.vtys.domain.repository.ProjectRepository
import com.example.vtys.domain.repository.TaskRepository
import com.example.vtys.domain.repository.UseCaseRepository

import com.example.vtys.data.local.TokenManager

class ViewModelFactory(
    private val projectRepository: ProjectRepository? = null,
    private val moduleRepository: ModuleRepository? = null,
    private val useCaseRepository: UseCaseRepository? = null,
    private val taskRepository: TaskRepository? = null,
    private val tokenManager: TokenManager? = null
) : ViewModelProvider.Factory {

    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        return when {
            modelClass.isAssignableFrom(DashboardViewModel::class.java) -> {
                DashboardViewModel(projectRepository!!, taskRepository!!, tokenManager!!) as T
            }
            modelClass.isAssignableFrom(ProjectListViewModel::class.java) -> {
                ProjectListViewModel(projectRepository!!) as T
            }
            modelClass.isAssignableFrom(ProjectDetailViewModel::class.java) -> {
                ProjectDetailViewModel(projectRepository!!, moduleRepository!!) as T
            }
            modelClass.isAssignableFrom(ModuleDetailViewModel::class.java) -> {
                ModuleDetailViewModel(moduleRepository!!, useCaseRepository!!) as T
            }
            modelClass.isAssignableFrom(UseCaseDetailViewModel::class.java) -> {
                UseCaseDetailViewModel(useCaseRepository!!, taskRepository!!) as T
            }
            modelClass.isAssignableFrom(TaskDetailViewModel::class.java) -> {
                TaskDetailViewModel(taskRepository!!) as T
            }
            modelClass.isAssignableFrom(TaskListViewModel::class.java) -> {
                TaskListViewModel(taskRepository!!) as T
            }
            else -> throw IllegalArgumentException("Unknown ViewModel class")
        }
    }
}
