package com.example.vtys.data.local.entities

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "projects")
data class ProjectEntity(
    @PrimaryKey val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String,
    val memberCount: Int = 0,
    val moduleCount: Int = 0,
    val cachedAt: Long = System.currentTimeMillis()
)
