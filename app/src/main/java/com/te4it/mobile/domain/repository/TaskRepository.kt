package com.te4it.mobile.domain.repository

import com.te4it.mobile.domain.model.PagedResult
import com.te4it.mobile.domain.model.Task
import com.te4it.mobile.domain.model.TaskRelation
import com.te4it.mobile.common.Resource

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
