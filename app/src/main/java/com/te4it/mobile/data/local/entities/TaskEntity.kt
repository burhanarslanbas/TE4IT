package com.te4it.mobile.data.local.entities

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "tasks")
data class TaskEntity(
    @PrimaryKey val id: String,
    val useCaseId: String,
    val creatorId: String,
    val assigneeId: String?,
    val assigneeName: String?,
    val title: String,
    val description: String?,
    val importantNotes: String?,
    val startedDate: String?,
    val dueDate: String?,
    val taskType: Int,
    val taskState: Int,
    val cachedAt: Long = System.currentTimeMillis()
)
