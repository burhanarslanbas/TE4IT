package com.te4it.mobile.data.network.dto.education

data class CourseDto(
    val id: String,
    val title: String,
    val description: String,
    val thumbnailUrl: String?,
    val estimatedDurationMinutes: Int?,
    val stepCount: Int?,
    val enrollmentCount: Int?,
    val createdAt: String,
    val progressPercentage: Int?,
    // For detail view, these might be present
    val roadmap: RoadmapDto?,
    val userEnrollment: UserEnrollmentDto?
)

data class UserEnrollmentDto(
    // mapped to 'isActive' from JSON
    val isActive: Boolean,
    val enrolledAt: String?,
    val startedAt: String?,
    val completedAt: String?
)
