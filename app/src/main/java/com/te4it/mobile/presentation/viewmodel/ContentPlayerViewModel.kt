package com.te4it.mobile.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.te4it.mobile.common.Resource
import com.te4it.mobile.domain.model.education.CourseContent
import com.te4it.mobile.domain.repository.EducationRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class ContentPlayerViewModel(
    private val repository: EducationRepository
) : ViewModel() {

    private val _content = MutableStateFlow<CourseContent?>(null)
    val content: StateFlow<CourseContent?> = _content

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    fun loadContent(courseId: String, contentId: String) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null

            // Since we don't have a direct getContentById, we fetch the course and filter.
            // In a production app, we might want to cache this or use a Room DAO.
            when (val result = repository.getCourseById(courseId)) {
                is Resource.Success -> {
                    val course = result.data
                    if (course != null && course.roadmap != null) {
                        val foundContent = course.roadmap.steps
                            .flatMap { it.contents }
                            .find { it.id == contentId }
                        
                        if (foundContent != null) {
                            _content.value = foundContent
                        } else {
                            _error.value = "İçerik bulunamadı."
                        }
                    } else {
                        _error.value = "Kurs veya müfredat bilgisi yüklenemedi."
                    }
                    _isLoading.value = false
                }
                is Resource.Error -> {
                    _error.value = result.message
                    _isLoading.value = false
                }
                is Resource.Loading -> {
                     // handled by _isLoading init
                }
            }
        }
    }
}
