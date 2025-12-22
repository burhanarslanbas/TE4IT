package com.te4it.mobile.domain.repository

import com.te4it.mobile.domain.model.PagedResult
import com.te4it.mobile.domain.model.Project
import com.te4it.mobile.domain.model.ProjectDetail
import com.te4it.mobile.domain.model.ProjectMember
import com.te4it.mobile.common.Resource

interface ProjectRepository {
    suspend fun getProjects(
        page: Int,
        pageSize: Int,
        search: String?,
        isActive: Boolean?
    ): Resource<PagedResult<Project>>

    suspend fun getProjectById(projectId: String): Resource<ProjectDetail>

    suspend fun getProjectMembers(projectId: String): Resource<List<ProjectMember>>
}
