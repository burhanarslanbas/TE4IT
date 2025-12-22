package com.te4it.mobile.data.network

import com.te4it.mobile.data.network.dto.PagedResult
import com.te4it.mobile.data.network.dto.UseCaseDto
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path
import retrofit2.http.Query

interface UseCaseApiService {
    
    @GET("usecases/modules/{moduleId}")
    suspend fun getUseCasesByModule(
        @Path("moduleId") moduleId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("isActive") isActive: Boolean? = null,
        @Query("search") search: String? = null
    ): Response<PagedResult<UseCaseDto>>
    
    @GET("usecases/{id}")
    suspend fun getUseCaseById(
        @Path("id") useCaseId: String
    ): Response<UseCaseDto>
}
