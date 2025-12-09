package com.example.vtys.data.repository

import com.example.vtys.common.Resource
import com.example.vtys.data.local.dao.UseCaseDao
import com.example.vtys.data.local.entities.UseCaseEntity
import com.example.vtys.data.network.UseCaseApiService
import com.example.vtys.data.network.dto.PagedResult as PagedResultDto
import com.example.vtys.data.network.dto.UseCaseDto
import com.example.vtys.domain.model.PagedResult
import com.example.vtys.domain.model.UseCase
import com.example.vtys.domain.repository.UseCaseRepository
import kotlinx.coroutines.flow.firstOrNull
import retrofit2.HttpException
import java.io.IOException

class UseCaseRepositoryImpl(
    private val api: UseCaseApiService,
    private val dao: UseCaseDao
) : UseCaseRepository {

    override suspend fun getUseCasesByModule(
        moduleId: String,
        page: Int,
        pageSize: Int,
        isActive: Boolean?,
        search: String?
    ): Resource<PagedResult<UseCase>> {
        return try {
            val response = api.getUseCasesByModule(moduleId, page, pageSize, isActive, search)
            if (response.isSuccessful && response.body() != null) {
                val result = response.body()!!.toDomain()
                
                // Cache only the first page and if no search filter is applied
                if (page == 1 && search.isNullOrEmpty()) {
                    val entities = result.items.map { it.toEntity() }
                    dao.clearUseCasesByModule(moduleId)
                    dao.insertUseCases(entities)
                }
                
                Resource.Success(result)
            } else {
                getUseCasesFromLocal(moduleId, page, pageSize)
            }
        } catch (e: Exception) {
            getUseCasesFromLocal(moduleId, page, pageSize)
        }
    }

    private suspend fun getUseCasesFromLocal(moduleId: String, page: Int, pageSize: Int): Resource<PagedResult<UseCase>> {
        val localUseCases = dao.getUseCasesByModule(moduleId).firstOrNull() ?: emptyList()
        if (localUseCases.isNotEmpty()) {
            val domainUseCases = localUseCases.map { it.toDomain() }
            val fromIndex = (page - 1) * pageSize
            val toIndex = minOf(fromIndex + pageSize, domainUseCases.size)
            
            if (fromIndex < domainUseCases.size) {
                val pagedItems = domainUseCases.subList(fromIndex, toIndex)
                return Resource.Success(
                    PagedResult(
                        items = pagedItems,
                        pageNumber = page,
                        pageSize = pageSize,
                        totalCount = domainUseCases.size,
                        totalPages = (domainUseCases.size + pageSize - 1) / pageSize,
                        hasPreviousPage = page > 1,
                        hasNextPage = toIndex < domainUseCases.size
                    )
                )
            }
        }
        return Resource.Error("No internet connection and no cached data available.")
    }

    override suspend fun getUseCaseById(useCaseId: String): Resource<UseCase> {
        return try {
            val response = api.getUseCaseById(useCaseId)
            if (response.isSuccessful && response.body() != null) {
                Resource.Success(response.body()!!.toDomain())
            } else {
                val localUseCase = dao.getUseCaseById(useCaseId)
                if (localUseCase != null) {
                    Resource.Success(localUseCase.toDomain())
                } else {
                    Resource.Error(response.message() ?: "An error occurred")
                }
            }
        } catch (e: Exception) {
            val localUseCase = dao.getUseCaseById(useCaseId)
            if (localUseCase != null) {
                Resource.Success(localUseCase.toDomain())
            } else {
                Resource.Error(e.message ?: "An error occurred")
            }
        }
    }

    private fun PagedResultDto<UseCaseDto>.toDomain(): PagedResult<UseCase> {
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

    private fun UseCaseDto.toDomain(): UseCase {
        return UseCase(
            id = id,
            moduleId = moduleId,
            title = title,
            description = description,
            importantNotes = importantNotes,
            isActive = isActive,
            startedDate = startedDate,
            taskCount = taskCount
        )
    }
    
    // Mappers for Entity
    private fun UseCase.toEntity(): UseCaseEntity {
        return UseCaseEntity(
            id = id,
            moduleId = moduleId,
            title = title,
            description = description,
            importantNotes = importantNotes,
            isActive = isActive,
            startedDate = startedDate,
            taskCount = taskCount
        )
    }
    
    private fun UseCaseEntity.toDomain(): UseCase {
        return UseCase(
            id = id,
            moduleId = moduleId,
            title = title,
            description = description,
            importantNotes = importantNotes,
            isActive = isActive,
            startedDate = startedDate,
            taskCount = taskCount
        )
    }
}
