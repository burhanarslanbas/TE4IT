package com.te4it.mobile.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.te4it.mobile.common.Resource
import com.te4it.mobile.domain.model.education.Course
import com.te4it.mobile.domain.repository.EducationRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class EducationViewModel(
    private val repository: EducationRepository
) : ViewModel() {

    private val _courses = MutableStateFlow<List<Course>>(emptyList())
    val courses: StateFlow<List<Course>> = _courses

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    init {
        loadCourses()
    }

    fun loadCourses() {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            
            // For MVP, just fetching page 1
            when (val result = repository.getCourses(1, 20, null)) {
                is Resource.Success -> {
                    _courses.value = result.data?.items ?: emptyList()
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
}
