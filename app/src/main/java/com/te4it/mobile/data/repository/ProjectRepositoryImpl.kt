package com.te4it.mobile.data.repository

import com.te4it.mobile.common.Resource
import com.te4it.mobile.data.local.dao.ProjectDao
import com.te4it.mobile.data.local.entities.ProjectEntity
import com.te4it.mobile.data.mock.MockData
import com.te4it.mobile.data.network.ProjectApiService
import com.te4it.mobile.data.network.dto.PagedResult as PagedResultDto
import com.te4it.mobile.data.network.dto.ProjectDetailDto
import com.te4it.mobile.data.network.dto.ProjectDto
import com.te4it.mobile.data.network.dto.ProjectMemberDto
import com.te4it.mobile.domain.model.PagedResult
import com.te4it.mobile.domain.model.Project
import com.te4it.mobile.domain.model.ProjectDetail
import com.te4it.mobile.domain.model.ProjectMember
import com.te4it.mobile.domain.repository.ProjectRepository
import kotlinx.coroutines.flow.firstOrNull
import retrofit2.HttpException
import java.io.IOException

class ProjectRepositoryImpl(
    private val api: ProjectApiService,
    private val dao: ProjectDao
) : ProjectRepository {

    override suspend fun getProjects(
        page: Int,
        pageSize: Int,
        search: String?,
        isActive: Boolean?
    ): Resource<PagedResult<Project>> {
        return try {
            // Try Network
            val response = api.getProjects(page, pageSize, search, isActive)
            if (response.isSuccessful && response.body() != null) {
                val result = response.body()!!.toDomain()
                
                // Cache only the first page and if no search filter is applied
                if (page == 1 && search.isNullOrEmpty()) {
                    val entities = result.items.map { it.toEntity() }
                    dao.clearProjects()
                    dao.insertProjects(entities)
                }
                
                Resource.Success(result)
            } else {
                // Fallback to Local
                getProjectsFromLocal(page, pageSize)
            }
        } catch (e: Exception) { // Catch all exceptions for fallback
            // Fallback to Local
            getProjectsFromLocal(page, pageSize)
        }
    }

    private suspend fun getProjectsFromLocal(page: Int, pageSize: Int): Resource<PagedResult<Project>> {
        val localProjects = dao.getAllProjects().firstOrNull() ?: emptyList()
        
        // If local is empty, use Mock Data for testing purposes
        val projectsToUse = if (localProjects.isNotEmpty()) {
            localProjects.map { it.toDomain() }
        } else {
            MockData.projects
        }

        if (projectsToUse.isNotEmpty()) {
            // Simple pagination for local/mock data
            val fromIndex = (page - 1) * pageSize
            val toIndex = minOf(fromIndex + pageSize, projectsToUse.size)
            
            if (fromIndex < projectsToUse.size) {
                val pagedItems = projectsToUse.subList(fromIndex, toIndex)
                return Resource.Success(
                    PagedResult(
                        items = pagedItems,
                        pageNumber = page,
                        pageSize = pageSize,
                        totalCount = projectsToUse.size,
                        totalPages = (projectsToUse.size + pageSize - 1) / pageSize,
                        hasPreviousPage = page > 1,
                        hasNextPage = toIndex < projectsToUse.size
                    )
                )
            }
        }
        return Resource.Error("No internet connection and no cached data available.")
    }

    override suspend fun getProjectById(projectId: String): Resource<ProjectDetail> {
        return try {
            val response = api.getProjectById(projectId)
            if (response.isSuccessful && response.body() != null) {
                Resource.Success(response.body()!!.toDomain())
            } else {
                // Try to get basic info from local if detail fails
                val localProject = dao.getProjectById(projectId)
                if (localProject != null) {
                    Resource.Success(localProject.toDomainDetail())
                } else {
                    Resource.Error(response.message() ?: "An error occurred")
                }
            }
        } catch (e: Exception) {
            val localProject = dao.getProjectById(projectId)
            if (localProject != null) {
                Resource.Success(localProject.toDomainDetail())
            } else {
                Resource.Error(e.message ?: "An error occurred")
            }
        }
    }

    override suspend fun getProjectMembers(projectId: String): Resource<List<ProjectMember>> {
        return try {
            val response = api.getProjectMembers(projectId)
            if (response.isSuccessful && response.body() != null) {
                Resource.Success(response.body()!!.map { it.toDomain() })
            } else {
                Resource.Error(response.message() ?: "An error occurred")
            }
        } catch (e: HttpException) {
            Resource.Error(e.message ?: "An error occurred")
        } catch (e: IOException) {
            Resource.Error("Couldn't reach server. Check your internet connection.")
        }
    }

    private fun PagedResultDto<ProjectDto>.toDomain(): PagedResult<Project> {
        return PagedResult(
            items = items.map { it.toDomain() },
            pageNumber = pageNumber,
            pageSize = pageSize,
            totalCount = totalCount,
            totalPages = totalPages,
            hasPreviousPage = hasPreviousPage,
            hasNextPage = hasNextPage
        )
    }

    private fun ProjectDto.toDomain(): Project {
        return Project(
            id = id,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate,
            memberCount = memberCount,
            moduleCount = moduleCount
        )
    }

    private fun ProjectDetailDto.toDomain(): ProjectDetail {
        return ProjectDetail(
            id = id,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate
        )
    }

    private fun ProjectMemberDto.toDomain(): ProjectMember {
        return ProjectMember(
            userId = userId,
            email = email,
            fullName = userName, // API returns userName, mapping to fullName
            role = role,
            roleName = roleName ?: getRoleName(role), // Fallback if null
            addedDate = joinedDate // API returns joinedDate
        )
    }

    private fun getRoleName(role: Int): String {
        return when (role) {
            1 -> "Owner"
            2 -> "Admin"
            3 -> "Member"
            4 -> "Viewer"
            else -> "Unknown"
        }
    }
    
    // Mappers for Entity
    private fun Project.toEntity(): ProjectEntity {
        return ProjectEntity(
            id = id,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate,
            memberCount = memberCount,
            moduleCount = moduleCount
        )
    }
    
    private fun ProjectEntity.toDomain(): Project {
        return Project(
            id = id,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate,
            memberCount = memberCount,
            moduleCount = moduleCount
        )
    }
    
    private fun ProjectEntity.toDomainDetail(): ProjectDetail {
        return ProjectDetail(
            id = id,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate
        )
    }
}
