package com.example.vtys.data.network

import com.example.vtys.data.network.dto.AuthResponse
import com.example.vtys.data.network.dto.ChangePasswordRequest
import com.example.vtys.data.network.dto.ForgotPasswordRequest
import com.example.vtys.data.network.dto.LoginRequest
import com.example.vtys.data.network.dto.RegisterRequest
import com.example.vtys.data.network.dto.ResetPasswordCommand
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthApiService {
    
    @POST("auth/login")
    suspend fun login(@Body request: LoginRequest): Response<AuthResponse>
    
    @POST("auth/register")
    suspend fun register(@Body request: RegisterRequest): Response<AuthResponse>
    
    @POST("auth/forgotPassword")
    suspend fun forgotPassword(@Body request: ForgotPasswordRequest): Response<Unit>
    
    @POST("auth/resetPassword")
    suspend fun resetPassword(@Body request: ResetPasswordCommand): Response<Unit>
    
    @POST("auth/changePassword")
    suspend fun changePassword(@Body request: ChangePasswordRequest): Response<Unit>
}