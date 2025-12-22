package com.te4it.mobile.data.network

import com.te4it.mobile.data.network.dto.AuthResponse
import com.te4it.mobile.data.network.dto.ChangePasswordRequest
import com.te4it.mobile.data.network.dto.ForgotPasswordRequest
import com.te4it.mobile.data.network.dto.LoginRequest
import com.te4it.mobile.data.network.dto.RegisterRequest
import com.te4it.mobile.data.network.dto.ResetPasswordCommand
import com.te4it.mobile.data.network.dto.RefreshTokenRequest
import com.te4it.mobile.data.network.dto.RevokeRefreshTokenRequest
import com.te4it.mobile.data.network.dto.UserDto
import retrofit2.Call
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
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

    @POST("auth/refreshToken")
    fun refreshToken(@Body request: RefreshTokenRequest): Call<AuthResponse> // Call kullanıyoruz çünkü Authenticator içinde senkron çağırmamız gerekebilir veya execute() kullanacağız.

    @POST("auth/revokeRefreshToken")
    suspend fun revokeRefreshToken(@Body request: RevokeRefreshTokenRequest): Response<Unit>

    @GET("auth/me")
    suspend fun me(): Response<UserDto>
}