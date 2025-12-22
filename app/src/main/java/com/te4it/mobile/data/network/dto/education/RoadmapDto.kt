package com.te4it.mobile.data.network.dto.education

data class RoadmapDto(
    val id: String?,
    val title: String,
    val description: String?,
    val estimatedDurationMinutes: Int,
    val steps: List<StepDto>
)

data class StepDto(
    val id: String,
    val title: String,
    val description: String?,
    val order: Int,
    val isRequired: Boolean,
    val estimatedDurationMinutes: Int,
    val contentCount: Int?,
    val contents: List<ContentDto>?
)

data class ContentDto(
    val id: String,
    val type: Int, // 1: Text, 2: VideoLink, 3: DocumentLink, 4: ExternalLink
    val title: String,
    val description: String?,
    val content: String?, // For Text type
    val linkUrl: String?,
    val embedUrl: String?, // For Video type
    val platform: String?, // "youtube", "vimeo", etc.
    val order: Int,
    val isRequired: Boolean
)
