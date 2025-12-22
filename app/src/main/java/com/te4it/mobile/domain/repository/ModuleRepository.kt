package com.te4it.mobile.domain.repository

import com.te4it.mobile.domain.model.Module
import com.te4it.mobile.domain.model.PagedResult
import com.te4it.mobile.common.Resource

interface ModuleRepository {
    suspend fun getModulesByProject(
        projectId: String,
        page: Int,
        pageSize: Int,
        isActive: Boolean?,
        search: String?
    ): Resource<PagedResult<Module>>

    suspend fun getModuleById(moduleId: String): Resource<Module>
}
