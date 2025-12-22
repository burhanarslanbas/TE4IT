package com.te4it.mobile.data.local.entities

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "modules")
data class ModuleEntity(
    @PrimaryKey val id: String,
    val projectId: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String,
    val useCaseCount: Int = 0,
    val cachedAt: Long = System.currentTimeMillis()
)
