package com.te4it.mobile.domain.model

import com.te4it.mobile.data.network.dto.TaskRelationType
import com.te4it.mobile.data.network.dto.TaskState
import com.te4it.mobile.data.network.dto.TaskType

data class Task(
    val id: String,
    val useCaseId: String,
    val creatorId: String,
    val assigneeId: String?,
    val assigneeName: String?,
    val title: String,
    val description: String?,
    val importantNotes: String?,
    val startedDate: String?,
    val dueDate: String?,
    val taskType: TaskType?,
    val taskState: TaskState?,
    val relations: List<TaskRelation> = emptyList()
)

data class TaskRelation(
    val id: String,
    val targetTaskId: String,
    val relationType: TaskRelationType?,
    val targetTaskTitle: String
)
