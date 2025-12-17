package com.example.vtys.domain.repository

import com.example.vtys.domain.model.Module
import com.example.vtys.domain.model.PagedResult
import com.example.vtys.common.Resource

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
