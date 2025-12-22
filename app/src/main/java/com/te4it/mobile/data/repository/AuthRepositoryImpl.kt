package com.te4it.mobile.data.repository

import com.te4it.mobile.data.local.TokenManager
import com.te4it.mobile.data.network.AuthApiService
import com.te4it.mobile.data.network.dto.ChangePasswordRequest
import com.te4it.mobile.data.network.dto.ErrorResponse
import com.te4it.mobile.data.network.dto.ForgotPasswordRequest
import com.te4it.mobile.data.network.dto.LoginRequest
import com.te4it.mobile.data.network.dto.RegisterRequest
import com.te4it.mobile.data.network.dto.ResetPasswordCommand
import com.te4it.mobile.domain.model.AuthResult
import com.te4it.mobile.domain.repository.AuthRepository
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
                // Kullanıcı bilgilerini sakla
                val userName = extractUserNameFromEmail(email)
                tokenManager.saveUserInfo(email, userName)
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string(), response.code())
                // Login hatası için daha açıklayıcı mesaj
                val userFriendlyMessage = when (response.code()) {
                    400, 401 -> "Kullanıcı adı veya şifre hatalı. Lütfen bilgilerinizi kontrol edin."
                    else -> errorMessage
                }
                emit(AuthResult.Error(userFriendlyMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Giriş işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin."))
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
                // Kullanıcı bilgilerini sakla
                tokenManager.saveUserInfo(email, userName)
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string(), response.code())
                // Register hatası için daha açıklayıcı mesaj
                val userFriendlyMessage = when (response.code()) {
                    400 -> "Kayıt işlemi başarısız oldu. Lütfen girdiğiniz bilgileri kontrol edin."
                    409 -> "Bu e-posta adresi zaten kullanımda. Lütfen farklı bir e-posta adresi deneyin."
                    else -> errorMessage
                }
                emit(AuthResult.Error(userFriendlyMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Kayıt işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin."))
        }
    }

    override suspend fun logout() {
        tokenManager.clearTokens()
    }

    // Yeni şifre değiştirme akışı (uygulama içi)
    override suspend fun changePassword(currentPassword: String, newPassword: String): Flow<AuthResult<Unit>> = flow {
        emit(AuthResult.Loading)
        try {
            val request = ChangePasswordRequest(currentPassword, newPassword, newPassword)
            val response = authApiService.changePassword(request)
            if (response.isSuccessful) {
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string(), response.code())
                // Şifre değiştirme hatası için daha açıklayıcı mesaj
                val userFriendlyMessage = when (response.code()) {
                    400 -> "Şifre değiştirme işlemi başarısız oldu. Mevcut şifrenizi doğru girdiğinizden emin olun."
                    401 -> "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın."
                    else -> errorMessage
                }
                emit(AuthResult.Error(userFriendlyMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Şifre değiştirme işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin."))
        }
    }

    // Eski şifre sıfırlama akışı (web için)
    override suspend fun forgotPassword(email: String): Flow<AuthResult<Unit>> = flow {
        emit(AuthResult.Loading)
        try {
            val request = ForgotPasswordRequest(email)
            val response = authApiService.forgotPassword(request)
            if (response.isSuccessful) {
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string(), response.code())
                // Forgot password hatası için daha açıklayıcı mesaj
                val userFriendlyMessage = when (response.code()) {
                    400 -> "Geçersiz e-posta adresi. Lütfen doğru bir e-posta adresi girdiğinizden emin olun."
                    404 -> "Bu e-posta adresine sahip bir kullanıcı bulunamadı."
                    else -> errorMessage
                }
                emit(AuthResult.Error(userFriendlyMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Şifre sıfırlama isteği gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin."))
        }
    }

    override suspend fun resetPassword(
        email: String,
        token: String,
        newPassword: String
    ): Flow<AuthResult<Unit>> = flow {
        emit(AuthResult.Loading)
        try {
            val request = ResetPasswordCommand(email, token, newPassword)
            val response = authApiService.resetPassword(request)
            if (response.isSuccessful) {
                emit(AuthResult.Success(Unit))
            } else {
                val errorMessage = parseErrorResponse(response.errorBody()?.string(), response.code())
                // Reset password hatası için daha açıklayıcı mesaj
                val userFriendlyMessage = when (response.code()) {
                    400 -> "Şifre sıfırlama işlemi başarısız oldu. Token'ın süresi dolmuş olabilir veya şifre gereksinimlerini karşılamıyor olabilir."
                    404 -> "Geçersiz token. Lütfen yeni bir şifre sıfırlama isteği gönderin."
                    else -> errorMessage
                }
                emit(AuthResult.Error(userFriendlyMessage))
            }
        } catch (e: Exception) {
            emit(AuthResult.Error(e.message ?: "Şifre sıfırlama işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin."))
        }
    }

    private fun parseErrorResponse(errorBody: String?, httpCode: Int = -1): String {
        return try {
            if (errorBody != null) {
                val errorResponse = gson.fromJson(errorBody, ErrorResponse::class.java)
                // Use the detail field if available, otherwise use title, otherwise provide a generic message
                errorResponse.detail ?: errorResponse.title ?: getGenericErrorMessage(httpCode)
            } else {
                getGenericErrorMessage(httpCode)
            }
        } catch (e: Exception) {
            // If parsing fails, return a generic error message based on HTTP code
            getGenericErrorMessage(httpCode)
        }
    }

    private fun getGenericErrorMessage(httpCode: Int): String {
        return when (httpCode) {
            400 -> "Geçersiz istek. Lütfen girdiğiniz bilgileri kontrol edin."
            401 -> "Kullanıcı adı veya şifre hatalı. Lütfen bilgilerinizi kontrol edin."
            403 -> "Erişim reddedildi. Bu işlemi yapma yetkiniz yok."
            404 -> "İstenilen kaynak bulunamadı."
            500 -> "Sunucu hatası. Lütfen daha sonra tekrar deneyin."
            else -> "Bir hata oluştu. Lütfen daha sonra tekrar deneyin."
        }
    }

    // Email adresinden kullanıcı adı çıkarma
    private fun extractUserNameFromEmail(email: String): String {
        return email.substringBefore("@")
    }
}