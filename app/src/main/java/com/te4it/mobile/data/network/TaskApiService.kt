package com.te4it.mobile.data.network

import com.te4it.mobile.data.network.dto.ChangeTaskStateRequest
import com.te4it.mobile.data.network.dto.PagedResult
import com.te4it.mobile.data.network.dto.TaskDto
import com.te4it.mobile.data.network.dto.TaskRelationDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.PATCH
import retrofit2.http.Path
import retrofit2.http.Query

interface TaskApiService {
    
    @GET("tasks/usecases/{useCaseId}")
    suspend fun getTasksByUseCase(
        @Path("useCaseId") useCaseId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("state") state: Int? = null,
        @Query("type") type: Int? = null,
        @Query("assigneeId") assigneeId: String? = null,
        @Query("dueDateFrom") dueDateFrom: String? = null,
        @Query("dueDateTo") dueDateTo: String? = null,
        @Query("search") search: String? = null
    ): Response<PagedResult<TaskDto>>
    
    @GET("tasks/{id}")
    suspend fun getTaskById(
        @Path("id") taskId: String
    ): Response<TaskDto>
    
    @GET("tasks/{taskId}/relations")
    suspend fun getTaskRelations(
        @Path("taskId") taskId: String
    ): Response<List<TaskRelationDto>>
    
    @PATCH("tasks/{id}/state")
    suspend fun changeTaskState(
        @Path("id") taskId: String,
        @Body request: ChangeTaskStateRequest
    ): Response<Unit>
}
