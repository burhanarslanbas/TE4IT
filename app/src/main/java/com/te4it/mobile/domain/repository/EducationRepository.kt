package com.te4it.mobile.domain.repository

import com.te4it.mobile.common.Resource
import com.te4it.mobile.data.network.dto.PagedResult
import com.te4it.mobile.domain.model.education.Course

interface EducationRepository {
    suspend fun getCourses(page: Int, pageSize: Int, search: String?): Resource<PagedResult<Course>>
    suspend fun getCourseById(courseId: String): Resource<Course>
    suspend fun enrollInCourse(courseId: String): Resource<Unit>
    suspend fun completeContent(contentId: String, timeSpentMinutes: Int): Resource<Unit>
}
