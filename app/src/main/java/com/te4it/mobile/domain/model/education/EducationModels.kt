package com.te4it.mobile.domain.model.education

data class Course(
    val id: String,
    val title: String,
    val description: String,
    val thumbnailUrl: String?,
    val estimatedDurationMinutes: Int,
    val isEnrolled: Boolean,
    val progressPercentage: Int, // 0-100
    val roadmap: Roadmap? = null
)

data class Roadmap(
    val id: String,
    val title: String,
    val estimatedDurationMinutes: Int,
    val steps: List<RoadmapStep>
)

data class RoadmapStep(
    val id: String,
    val title: String,
    val description: String?,
    val order: Int,
    val isRequired: Boolean,
    val estimatedDurationMinutes: Int,
    val isLocked: Boolean, // Domain logic will calculate this
    val isCompleted: Boolean,
    val contents: List<CourseContent>
)

data class CourseContent(
    val id: String,
    val type: ContentType,
    val title: String,
    val textContent: String?,
    val linkUrl: String?,
    val embedUrl: String?,
    val isRequired: Boolean,
    val isCompleted: Boolean, // From progress
    val platform: String?
)

enum class ContentType {
    TEXT, VIDEO, DOCUMENT, EXTERNAL_LINK, UNKNOWN
}
