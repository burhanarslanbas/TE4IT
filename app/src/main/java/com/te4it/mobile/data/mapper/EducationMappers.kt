package com.te4it.mobile.data.mapper

import com.te4it.mobile.data.network.dto.education.ContentDto
import com.te4it.mobile.data.network.dto.education.CourseDto
import com.te4it.mobile.data.network.dto.education.RoadmapDto
import com.te4it.mobile.data.network.dto.education.StepDto
import com.te4it.mobile.domain.model.education.ContentType
import com.te4it.mobile.domain.model.education.Course
import com.te4it.mobile.domain.model.education.CourseContent
import com.te4it.mobile.domain.model.education.Roadmap
import com.te4it.mobile.domain.model.education.RoadmapStep

fun CourseDto.toDomain(): Course {
    return Course(
        id = id,
        title = title,
        description = description,
        thumbnailUrl = thumbnailUrl,
        estimatedDurationMinutes = estimatedDurationMinutes ?: 0,
        // API uses isActive to indicate enrollment
        isEnrolled = userEnrollment?.isActive == true,
        progressPercentage = progressPercentage ?: 0,
        roadmap = roadmap?.toDomain()
        // Note: For simple list view, roadmap might be null
    )
}

fun RoadmapDto.toDomain(): Roadmap {
    // When mapping steps, we initially assume they are just the structure.
    // Enhanced progress logic (locked/completed) usually requires combining with ProgressDto,
    // but for simplicity in this iteration, we map what we have.
    return Roadmap(
        id = id ?: "",
        title = title,
        estimatedDurationMinutes = estimatedDurationMinutes,
        steps = steps.map { it.toDomain() }
    )
}

fun StepDto.toDomain(): RoadmapStep {
    return RoadmapStep(
        id = id,
        title = title,
        description = description,
        order = order,
        isRequired = isRequired,
        estimatedDurationMinutes = estimatedDurationMinutes,
        // Default values, should be updated with actual progress data ideally
        isLocked = false, 
        isCompleted = false,
        contents = contents?.map { it.toDomain() } ?: emptyList()
    )
}

fun ContentDto.toDomain(): CourseContent {
    return CourseContent(
        id = id,
        type = when (type) {
            1 -> ContentType.TEXT
            2 -> ContentType.VIDEO
            3 -> ContentType.DOCUMENT
            4 -> ContentType.EXTERNAL_LINK
            else -> ContentType.UNKNOWN
        },
        title = title,
        textContent = content,
        linkUrl = linkUrl,
        embedUrl = embedUrl,
        isRequired = isRequired,
        platform = platform,
        isCompleted = false // Default
    )
}
