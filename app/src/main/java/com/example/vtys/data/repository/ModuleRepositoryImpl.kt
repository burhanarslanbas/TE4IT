package com.example.vtys.data.repository

import com.example.vtys.common.Resource
import com.example.vtys.data.local.dao.ModuleDao
import com.example.vtys.data.local.entities.ModuleEntity
import com.example.vtys.data.network.ModuleApiService
import com.example.vtys.data.network.dto.ModuleDto
import com.example.vtys.data.network.dto.PagedResult as PagedResultDto
import com.example.vtys.domain.model.Module
import com.example.vtys.domain.model.PagedResult
import com.example.vtys.domain.repository.ModuleRepository
import kotlinx.coroutines.flow.firstOrNull
import retrofit2.HttpException
import java.io.IOException

class ModuleRepositoryImpl(
    private val api: ModuleApiService,
    private val dao: ModuleDao
) : ModuleRepository {

    override suspend fun getModulesByProject(
        projectId: String,
        page: Int,
        pageSize: Int,
        isActive: Boolean?,
        search: String?
    ): Resource<PagedResult<Module>> {
        return try {
            val response = api.getModulesByProject(projectId, page, pageSize, isActive, search)
            if (response.isSuccessful && response.body() != null) {
                val result = response.body()!!.toDomain(projectId)
                
                // Cache only the first page and if no search filter is applied
                if (page == 1 && search.isNullOrEmpty()) {
                    val entities = result.items.map { it.toEntity(projectId) }
                    dao.clearModulesByProject(projectId)
                    dao.insertModules(entities)
                }
                
                Resource.Success(result)
            } else {
                getModulesFromLocal(projectId, page, pageSize)
            }
        } catch (e: Exception) {
            getModulesFromLocal(projectId, page, pageSize)
        }
    }

    private suspend fun getModulesFromLocal(projectId: String, page: Int, pageSize: Int): Resource<PagedResult<Module>> {
        val localModules = dao.getModulesByProject(projectId).firstOrNull() ?: emptyList()
        if (localModules.isNotEmpty()) {
            val domainModules = localModules.map { it.toDomain() }
            val fromIndex = (page - 1) * pageSize
            val toIndex = minOf(fromIndex + pageSize, domainModules.size)
            
            if (fromIndex < domainModules.size) {
                val pagedItems = domainModules.subList(fromIndex, toIndex)
                return Resource.Success(
                    PagedResult(
                        items = pagedItems,
                        pageNumber = page,
                        pageSize = pageSize,
                        totalCount = domainModules.size,
                        totalPages = (domainModules.size + pageSize - 1) / pageSize,
                        hasPreviousPage = page > 1,
                        hasNextPage = toIndex < domainModules.size
                    )
                )
            }
        }
        return Resource.Error("No internet connection and no cached data available.")
    }

    override suspend fun getModuleById(moduleId: String): Resource<Module> {
        return try {
            val response = api.getModuleById(moduleId)
            if (response.isSuccessful && response.body() != null) {
                Resource.Success(response.body()!!.toDomain())
            } else {
                val localModule = dao.getModuleById(moduleId)
                if (localModule != null) {
                    Resource.Success(localModule.toDomain())
                } else {
                    Resource.Error(response.message() ?: "An error occurred")
                }
            }
        } catch (e: Exception) {
            val localModule = dao.getModuleById(moduleId)
            if (localModule != null) {
                Resource.Success(localModule.toDomain())
            } else {
                Resource.Error(e.message ?: "An error occurred")
            }
        }
    }

    private fun PagedResultDto<ModuleDto>.toDomain(projectId: String): PagedResult<Module> {
        return PagedResult(
            items = items.map { it.toDomain(projectId) },
            pageNumber = pageNumber,
            pageSize = pageSize,
            totalCount = totalCount,
            totalPages = totalPages,
            hasPreviousPage = hasPreviousPage,
            hasNextPage = hasNextPage
        )
    }

    private fun ModuleDto.toDomain(projectId: String? = null): Module {
        return Module(
            id = id,
            projectId = this.projectId ?: projectId ?: "", // Fallback to empty string or handle error if both null
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate,
            useCaseCount = useCaseCount
        )
    }
    
    // Mappers for Entity
    private fun Module.toEntity(projectId: String): ModuleEntity {
        return ModuleEntity(
            id = id,
            projectId = projectId,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate,
            useCaseCount = useCaseCount
        )
    }
    
    private fun ModuleEntity.toDomain(): Module {
        return Module(
            id = id,
            projectId = projectId,
            title = title,
            description = description,
            isActive = isActive,
            startedDate = startedDate,
            useCaseCount = useCaseCount
        )
    }
}
