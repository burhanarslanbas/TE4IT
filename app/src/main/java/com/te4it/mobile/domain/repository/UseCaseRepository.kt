package com.te4it.mobile.domain.repository

import com.te4it.mobile.domain.model.PagedResult
import com.te4it.mobile.domain.model.UseCase
import com.te4it.mobile.common.Resource

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
