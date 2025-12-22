package com.te4it.mobile.data.network.dto.education

data class EnrollmentResponseDto(
    val id: String?,
    val courseId: String?,
    val userId: String?,
    val enrolledAt: String?,
    val startedAt: String?,
    val completedAt: String?
)

data class CourseProgressDto(
    val courseId: String,
    val enrollmentId: String,
    val progressPercentage: Int,
    val completedSteps: Int,
    val totalSteps: Int,
    val timeSpentMinutes: Int,
    val steps: List<StepProgressDto>
)

data class StepProgressDto(
    val stepId: String,
    val title: String,
    val order: Int,
    val isCompleted: Boolean,
    val completedAt: String?,
    val contents: List<ContentProgressDto>
)

data class ContentProgressDto(
    val contentId: String,
    val title: String,
    val isCompleted: Boolean,
    val completedAt: String?
)
