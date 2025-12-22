package com.te4it.mobile.domain.model

data class Project(
    val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String,
    val memberCount: Int = 0,
    val moduleCount: Int = 0
)

data class ProjectDetail(
    val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String
)

data class ProjectMember(
    val userId: String,
    val email: String,
    val fullName: String,
    val role: Int,
    val roleName: String,
    val addedDate: String
)
