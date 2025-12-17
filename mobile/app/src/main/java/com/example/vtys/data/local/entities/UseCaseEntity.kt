package com.example.vtys.data.local.entities

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "usecases")
data class UseCaseEntity(
    @PrimaryKey val id: String,
    val moduleId: String,
    val title: String,
    val description: String?,
    val importantNotes: String?,
    val isActive: Boolean,
    val startedDate: String,
    val taskCount: Int = 0,
    val cachedAt: Long = System.currentTimeMillis()
)
