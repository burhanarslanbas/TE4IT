package com.te4it.mobile.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.te4it.mobile.common.Resource
import com.te4it.mobile.domain.model.education.Course
import com.te4it.mobile.domain.repository.EducationRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class CourseDetailViewModel(
    private val repository: EducationRepository
) : ViewModel() {

    private val _course = MutableStateFlow<Course?>(null)
    val course: StateFlow<Course?> = _course

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    private val _enrollmentStatus = MutableStateFlow<String?>(null)
    val enrollmentStatus: StateFlow<String?> = _enrollmentStatus

    fun loadCourseDetails(courseId: String) {
        viewModelScope.launch {
            _isLoading.value = true
            when (val result = repository.getCourseById(courseId)) {
                is Resource.Success -> {
                    _course.value = result.data
                    _isLoading.value = false
                }
                is Resource.Error -> {
                    _error.value = result.message
                    _isLoading.value = false
                }
                is Resource.Loading -> {
                    _isLoading.value = true
                }
            }
        }
    }

    fun enrollInCourse(courseId: String) {
        viewModelScope.launch {
            _isLoading.value = true
            when (val result = repository.enrollInCourse(courseId)) {
                is Resource.Success -> {
                    _enrollmentStatus.value = "Success"
                    loadCourseDetails(courseId) // Reload to update status
                }
                is Resource.Error -> {
                    _enrollmentStatus.value = "Error: ${result.message}"
                    _isLoading.value = false
                }
                is Resource.Loading -> {}
            }
        }
    }
    
    fun clearEnrollmentStatus() {
        _enrollmentStatus.value = null
    }
}
