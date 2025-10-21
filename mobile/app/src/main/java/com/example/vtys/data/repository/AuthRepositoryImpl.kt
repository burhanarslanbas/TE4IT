package com.example.vtys.data.repository

import com.example.vtys.data.local.TokenManager
import com.example.vtys.data.network.AuthApiService
import com.example.vtys.data.network.dto.ErrorResponse
import com.example.vtys.data.network.dto.LoginRequest
import com.example.vtys.data.network.dto.RegisterRequest
import com.example.vtys.domain.model.AuthResult
import com.example.vtys.domain.repository.AuthRepository
import com.google.gson.Gson
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flow

class AuthRepositoryImpl(
    private val authApiService: AuthApiService,
    private val tokenManager: TokenManager,
    private val gson: Gson
) : AuthRepository {

    override suspend fun login(email: String, password: String): Flow<AuthResult<Unit>> = flow {
        emit(AuthResult.Loading)
        try {
            val response = authApiService.login(LoginRequest(email, password))
            if (response.isSuccessful && response.body() != null) {
                val authResponse = response.body()!!
                tokenManager.saveTokens(
                    accessToken = authResponse.accessToken,
                    refreshToken = authResponse.refreshToken
                )
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string())
                emit(AuthResult.Error(errorMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Bilinmeyen bir hata oluştu"))
        }
    }

    override suspend fun register(
        userName: String,
        email: String,
        password: String
    ): Flow<AuthResult<Unit>> = flow {
        emit(AuthResult.Loading)
        try {
            val response = authApiService.register(RegisterRequest(userName, email, password))
            if (response.isSuccessful && response.body() != null) {
                val authResponse = response.body()!!
                tokenManager.saveTokens(
                    accessToken = authResponse.accessToken,
                    refreshToken = authResponse.refreshToken
                )
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string())
                emit(AuthResult.Error(errorMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Bilinmeyen bir hata oluştu"))
        }
    }

    override suspend fun logout() {
        tokenManager.clearTokens()
    }

    private fun parseErrorResponse(errorBody: String?): String {
        return try {
            if (errorBody != null) {
                val errorResponse = gson.fromJson(errorBody, ErrorResponse::class.java)
                errorResponse.detail ?: errorResponse.title ?: "Bir hata oluştu"
            } else {
                "Bir hata oluştu"
            }
        } catch (e: Exception) {
            "Bir hata oluştu"
        }
    }
}

