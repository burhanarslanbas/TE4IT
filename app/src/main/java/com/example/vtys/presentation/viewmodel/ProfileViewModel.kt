package com.example.vtys.presentation.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.example.vtys.domain.model.AuthResult
import com.example.vtys.domain.repository.AuthRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class ProfileUiState(
    val isLoading: Boolean = false,
    val isSuccess: Boolean = false,
    val errorMessage: String? = null
)

// Yeni şifre değiştirme için UI state
data class ChangePasswordUiState(
    val isLoading: Boolean = false,
    val isSuccess: Boolean = false,
    val errorMessage: String? = null
)

// Eski şifre sıfırlama için UI state
data class ResetPasswordUiState(
    val isLoading: Boolean = false,
    val isSuccess: Boolean = false,
    val errorMessage: String? = null,
    val emailSent: Boolean = false, // Email gönderildi mi?
    val tokenReceived: Boolean = false // Token alındı mı?
)

class ProfileViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _changePasswordState = MutableStateFlow(ChangePasswordUiState())
    val changePasswordState: StateFlow<ChangePasswordUiState> = _changePasswordState.asStateFlow()

    private val _resetPasswordState = MutableStateFlow(ResetPasswordUiState())
    val resetPasswordState: StateFlow<ResetPasswordUiState> = _resetPasswordState.asStateFlow()

    private val _logoutState = MutableStateFlow(ProfileUiState())
    val logoutState: StateFlow<ProfileUiState> = _logoutState.asStateFlow()

    // Yeni şifre değiştirme akışı (uygulama içi)
    fun changePassword(currentPassword: String, newPassword: String) {
        viewModelScope.launch {
            authRepository.changePassword(currentPassword, newPassword).collect { result ->
                when (result) {
                    is AuthResult.Loading -> {
                        _changePasswordState.value = ChangePasswordUiState(isLoading = true)
                    }
                    is AuthResult.Success -> {
                        _changePasswordState.value = ChangePasswordUiState(isSuccess = true)
                    }
                    is AuthResult.Error -> {
                        _changePasswordState.value = ChangePasswordUiState(errorMessage = result.message)
                    }
                }
            }
        }
    }

    // Eski şifre sıfırlama akışı (web için)
    fun forgotPassword(email: String) {
        viewModelScope.launch {
            authRepository.forgotPassword(email).collect { result ->
                when (result) {
                    is AuthResult.Loading -> {
                        _resetPasswordState.value = ResetPasswordUiState(isLoading = true)
                    }
                    is AuthResult.Success -> {
                        _resetPasswordState.value = ResetPasswordUiState(emailSent = true)
                    }
                    is AuthResult.Error -> {
                        _resetPasswordState.value = ResetPasswordUiState(errorMessage = result.message)
                    }
                }
            }
        }
    }

    fun resetPassword(email: String, token: String, newPassword: String) {
        viewModelScope.launch {
            authRepository.resetPassword(email, token, newPassword).collect { result ->
                when (result) {
                    is AuthResult.Loading -> {
                        _resetPasswordState.value = ResetPasswordUiState(isLoading = true)
                    }
                    is AuthResult.Success -> {
                        _resetPasswordState.value = ResetPasswordUiState(isSuccess = true)
                    }
                    is AuthResult.Error -> {
                        _resetPasswordState.value = ResetPasswordUiState(errorMessage = result.message)
                    }
                }
            }
        }
    }

    fun logout() {
        viewModelScope.launch {
            authRepository.logout()
            _logoutState.value = ProfileUiState(isSuccess = true)
        }
    }

    fun resetChangePasswordState() {
        _changePasswordState.value = ChangePasswordUiState()
    }

    fun resetResetPasswordState() {
        _resetPasswordState.value = ResetPasswordUiState()
    }

    fun resetLogoutState() {
        _logoutState.value = ProfileUiState()
    }
}

class ProfileViewModelFactory(
    private val authRepository: AuthRepository
) : ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if (modelClass.isAssignableFrom(ProfileViewModel::class.java)) {
            @Suppress("UNCHECKED_CAST")
            return ProfileViewModel(authRepository) as T
        }
        throw IllegalArgumentException("Unknown ViewModel class")
    }
}