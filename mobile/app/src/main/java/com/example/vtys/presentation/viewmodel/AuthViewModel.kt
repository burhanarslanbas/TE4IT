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

data class AuthUiState(
    val isLoading: Boolean = false,
    val isSuccess: Boolean = false,
    val errorMessage: String? = null
)

class AuthViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _loginState = MutableStateFlow(AuthUiState())
    val loginState: StateFlow<AuthUiState> = _loginState.asStateFlow()

    private val _registerState = MutableStateFlow(AuthUiState())
    val registerState: StateFlow<AuthUiState> = _registerState.asStateFlow()

    fun login(email: String, password: String) {
        viewModelScope.launch {
            authRepository.login(email, password).collect { result ->
                when (result) {
                    is AuthResult.Loading -> {
                        _loginState.value = AuthUiState(isLoading = true)
                    }
                    is AuthResult.Success -> {
                        _loginState.value = AuthUiState(isSuccess = true)
                    }
                    is AuthResult.Error -> {
                        _loginState.value = AuthUiState(errorMessage = result.message)
                    }
                }
            }
        }
    }

    fun register(userName: String, email: String, password: String) {
        viewModelScope.launch {
            authRepository.register(userName, email, password).collect { result ->
                when (result) {
                    is AuthResult.Loading -> {
                        _registerState.value = AuthUiState(isLoading = true)
                    }
                    is AuthResult.Success -> {
                        _registerState.value = AuthUiState(isSuccess = true)
                    }
                    is AuthResult.Error -> {
                        _registerState.value = AuthUiState(errorMessage = result.message)
                    }
                }
            }
        }
    }

    fun resetLoginState() {
        _loginState.value = AuthUiState()
    }

    fun resetRegisterState() {
        _registerState.value = AuthUiState()
    }
}

class AuthViewModelFactory(
    private val authRepository: AuthRepository
) : ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if (modelClass.isAssignableFrom(AuthViewModel::class.java)) {
            @Suppress("UNCHECKED_CAST")
            return AuthViewModel(authRepository) as T
        }
        throw IllegalArgumentException("Unknown ViewModel class")
    }
}

