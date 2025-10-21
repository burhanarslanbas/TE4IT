package com.example.vtys.domain.repository

import com.example.vtys.domain.model.AuthResult
import kotlinx.coroutines.flow.Flow

interface AuthRepository {
    suspend fun login(email: String, password: String): Flow<AuthResult<Unit>>
    suspend fun register(userName: String, email: String, password: String): Flow<AuthResult<Unit>>
    suspend fun logout()
}

