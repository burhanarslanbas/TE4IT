package com.te4it.mobile.data.repository

import com.te4it.mobile.common.Resource
import com.te4it.mobile.data.mapper.toDomain
import com.te4it.mobile.data.network.EducationApiService
import com.te4it.mobile.data.network.dto.PagedResult
import com.te4it.mobile.domain.model.education.Course
import com.te4it.mobile.domain.repository.EducationRepository
import javax.inject.Inject

class EducationRepositoryImpl @Inject constructor(
    private val api: EducationApiService
) : EducationRepository {

    override suspend fun getCourses(
        page: Int,
        pageSize: Int,
        search: String?
    ): Resource<PagedResult<Course>> {
        return try {
            val response = api.getCourses(page, pageSize, search)
            if (response.isSuccessful && response.body() != null) {
                val dtoResult = response.body()!!
                android.util.Log.d("EducationRepo", "getCourses success: ${dtoResult.items.size} items found. Total: ${dtoResult.totalCount}")
                val domainItems = dtoResult.items.map { it.toDomain() }
                
                val domainResult = PagedResult(
                    items = domainItems,
                    pageNumber = dtoResult.pageNumber,
                    pageSize = dtoResult.pageSize,
                    totalCount = dtoResult.totalCount,
                    totalPages = dtoResult.totalPages,
                    hasPreviousPage = dtoResult.hasPreviousPage,
                    hasNextPage = dtoResult.hasNextPage
                )
                Resource.Success(domainResult)
            } else {
                android.util.Log.e("EducationRepo", "getCourses failed: ${response.code()} ${response.message()}")
                Resource.Error(response.message() ?: "Bir hata oluştu")
            }
        } catch (e: Exception) {
            android.util.Log.e("EducationRepo", "getCourses exception: ${e.localizedMessage}")
            Resource.Error(e.localizedMessage ?: "Bağlantı hatası")
        }
    }

    override suspend fun getCourseById(courseId: String): Resource<Course> {
        return try {
            val response = api.getCourseById(courseId)
            if (response.isSuccessful && response.body() != null) {
                Resource.Success(response.body()!!.toDomain())
            } else {
                Resource.Error(response.message() ?: "Kurs bulunamadı")
            }
        } catch (e: Exception) {
            Resource.Error(e.localizedMessage ?: "Bağlantı hatası")
        }
    }

    override suspend fun enrollInCourse(courseId: String): Resource<Unit> {
        return try {
            val response = api.enrollInCourse(courseId)
            if (response.isSuccessful) {
                Resource.Success(Unit)
            } else {
                Resource.Error(response.message())
            }
        } catch (e: Exception) {
            Resource.Error(e.localizedMessage ?: "Unknown error")
        }
    }

    override suspend fun completeContent(contentId: String, timeSpentMinutes: Int): Resource<Unit> {
         return try {
             val request = mapOf("timeSpentMinutes" to timeSpentMinutes)
             val response = api.completeContent(contentId, request)
             if (response.isSuccessful) {
                 Resource.Success(Unit)
             } else {
                 Resource.Error(response.message())
             }
         } catch (e: Exception) {
             Resource.Error(e.localizedMessage ?: "Unknown error")
         }
    }
}
