package com.example.vtys.domain.repository

import com.example.vtys.domain.model.PagedResult
import com.example.vtys.domain.model.Task
import com.example.vtys.domain.model.TaskRelation
import com.example.vtys.common.Resource

interface TaskRepository {
    suspend fun getTasksByUseCase(
        useCaseId: String,
        page: Int,
        pageSize: Int,
        state: Int?,
        type: Int?,
        assigneeId: String?,
        dueDateFrom: String?,
        dueDateTo: String?,
        search: String?
    ): Resource<PagedResult<Task>>

    suspend fun getTaskById(taskId: String): Resource<Task>

    suspend fun getTaskRelations(taskId: String): Resource<List<TaskRelation>>

    suspend fun changeTaskState(taskId: String, newState: Int): Resource<Unit>

    suspend fun getTasksByAssignee(assigneeId: String): Resource<List<Task>>
}
