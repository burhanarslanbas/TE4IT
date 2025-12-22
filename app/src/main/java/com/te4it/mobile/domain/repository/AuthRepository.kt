package com.te4it.mobile.domain.repository

import com.te4it.mobile.domain.model.AuthResult
import kotlinx.coroutines.flow.Flow

interface AuthRepository {
    suspend fun login(email: String, password: String): Flow<AuthResult<Unit>>
    suspend fun register(userName: String, email: String, password: String): Flow<AuthResult<Unit>>
    suspend fun logout()
    
    // Yeni şifre değiştirme akışı (uygulama içi)
    suspend fun changePassword(currentPassword: String, newPassword: String): Flow<AuthResult<Unit>>
    
    // Eski şifre sıfırlama akışı (web için)
    suspend fun forgotPassword(email: String): Flow<AuthResult<Unit>>
    suspend fun resetPassword(email: String, token: String, newPassword: String): Flow<AuthResult<Unit>>
}