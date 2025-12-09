package com.example.vtys.data.network

import com.example.vtys.data.local.TokenManager
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.runBlocking
import okhttp3.Interceptor
import okhttp3.Response

class AuthInterceptor(
    private val tokenManager: TokenManager
) : Interceptor {
    
    override fun intercept(chain: Interceptor.Chain): Response {
        val originalRequest = chain.request()
        val originalUrl = originalRequest.url
        
        // Check if this is a protected endpoint (not login or register)
        val isProtectedEndpoint = !originalUrl.encodedPath.contains("/auth/login") && 
                                 !originalUrl.encodedPath.contains("/auth/register")
        
        return if (isProtectedEndpoint) {
            // For protected endpoints, add the Authorization header
            val accessToken = runBlocking { 
                tokenManager.getAccessToken().first() 
            }
            
            if (!accessToken.isNullOrEmpty()) {
                val authenticatedRequest = originalRequest.newBuilder()
                    .addHeader("Authorization", "Bearer $accessToken")
                    .build()
                chain.proceed(authenticatedRequest)
            } else {
                // If no token is available, proceed with original request
                // The API will return 401 which we'll handle in the repository
                chain.proceed(originalRequest)
            }
        } else {
            // For login/register endpoints, proceed without authentication
            chain.proceed(originalRequest)
        }
    }
}