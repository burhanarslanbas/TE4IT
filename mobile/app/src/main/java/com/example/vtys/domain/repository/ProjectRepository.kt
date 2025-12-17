package com.example.vtys.domain.repository

import com.example.vtys.domain.model.PagedResult
import com.example.vtys.domain.model.Project
import com.example.vtys.domain.model.ProjectDetail
import com.example.vtys.domain.model.ProjectMember
import com.example.vtys.common.Resource

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
