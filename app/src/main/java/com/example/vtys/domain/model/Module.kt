package com.example.vtys.domain.model

data class Module(
    val id: String,
    val projectId: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String,
    val useCaseCount: Int = 0
)
