package com.example.vtys.data.network

import com.example.vtys.data.network.dto.ModuleDto
import com.example.vtys.data.network.dto.PagedResult
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path
import retrofit2.http.Query

interface ModuleApiService {
    
    @GET("modules/projects/{projectId}")
    suspend fun getModulesByProject(
        @Path("projectId") projectId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("isActive") isActive: Boolean? = null,
        @Query("search") search: String? = null
    ): Response<PagedResult<ModuleDto>>
    
    @GET("modules/{id}")
    suspend fun getModuleById(
        @Path("id") moduleId: String
    ): Response<ModuleDto>
}
