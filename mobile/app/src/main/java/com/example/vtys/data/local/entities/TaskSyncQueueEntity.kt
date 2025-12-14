package com.example.vtys.data.local.entities

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "task_sync_queue")
data class TaskSyncQueueEntity(
    @PrimaryKey(autoGenerate = true) val id: Int = 0,
    val taskId: String,
    val newState: Int,
    val timestamp: Long = System.currentTimeMillis()
)
