package com.example.vtys.domain.repository

import com.example.vtys.domain.model.PagedResult
import com.example.vtys.domain.model.UseCase
import com.example.vtys.common.Resource

interface UseCaseRepository {
    suspend fun getUseCasesByModule(
        moduleId: String,
        page: Int,
        pageSize: Int,
        isActive: Boolean?,
        search: String?
    ): Resource<PagedResult<UseCase>>

    suspend fun getUseCaseById(useCaseId: String): Resource<UseCase>
}
