package com.te4it.mobile.data.network

import com.te4it.mobile.data.network.dto.PagedResult
import com.te4it.mobile.data.network.dto.education.CourseDto
import com.te4it.mobile.data.network.dto.education.CourseProgressDto
import com.te4it.mobile.data.network.dto.education.EnrollmentResponseDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Query

interface EducationApiService {
    
    @GET("education/courses")
    suspend fun getCourses(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 10,
        @Query("search") search: String? = null
    ): Response<PagedResult<CourseDto>>

    @GET("education/courses/{courseId}")
    suspend fun getCourseById(
        @Path("courseId") courseId: String
    ): Response<CourseDto>

    @POST("education/courses/{courseId}/enroll")
    suspend fun enrollInCourse(
        @Path("courseId") courseId: String
    ): Response<EnrollmentResponseDto>

    @GET("education/courses/{courseId}/progress")
    suspend fun getCourseProgress(
        @Path("courseId") courseId: String
    ): Response<CourseProgressDto>

    @POST("education/contents/{contentId}/complete")
    suspend fun completeContent(
        @Path("contentId") contentId: String,
        @Body request: Map<String, Int> // e.g., {"timeSpentMinutes": 10}
    ): Response<Unit>
    
    @GET("education/progress/dashboard")
    suspend fun getProgressDashboard(): Response<Map<String, Any>> // Can be refined later based on actual response
}
