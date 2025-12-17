package com.example.vtys.domain.model

data class UseCase(
    val id: String,
    val moduleId: String,
    val title: String,
    val description: String?,
    val importantNotes: String?,
    val isActive: Boolean,
    val startedDate: String,
    val taskCount: Int = 0
)
