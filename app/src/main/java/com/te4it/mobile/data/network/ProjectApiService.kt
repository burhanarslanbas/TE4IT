package com.te4it.mobile.data.network

import com.te4it.mobile.data.network.dto.PagedResult
import com.te4it.mobile.data.network.dto.ProjectDetailDto
import com.te4it.mobile.data.network.dto.ProjectDto
import com.te4it.mobile.data.network.dto.ProjectMemberDto
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path
import retrofit2.http.Query

interface ProjectApiService {
    
    @GET("projects")
    suspend fun getProjects(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("search") search: String? = null,
        @Query("isActive") isActive: Boolean? = null
    ): Response<PagedResult<ProjectDto>>
    
    @GET("projects/{id}")
    suspend fun getProjectById(
        @Path("id") projectId: String
    ): Response<ProjectDetailDto>
    
    @GET("projects/{projectId}/members")
    suspend fun getProjectMembers(
        @Path("projectId") projectId: String
    ): Response<List<ProjectMemberDto>>
}
