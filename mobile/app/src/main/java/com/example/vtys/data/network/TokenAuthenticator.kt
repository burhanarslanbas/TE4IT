package com.example.vtys.data.network

import com.example.vtys.data.local.TokenManager
import com.example.vtys.data.network.dto.RefreshTokenRequest
import okhttp3.Authenticator
import okhttp3.Request
import okhttp3.Response
import okhttp3.Route

class TokenAuthenticator(
    private val tokenManager: TokenManager,
    private val authApiProvider: () -> AuthApiService // Circular dependency'yi önlemek için lambda kullanıyoruz
) : Authenticator {

    override fun authenticate(route: Route?, response: Response): Request? {
        // 401 response geldiğinde token yenile
        // Eğer zaten yenileme denendiyse (header'da yeni token varsa) tekrar deneme
        if (responseCount(response) >= 3) {
            return null // Sonsuz döngüyü önle
        }

        val refreshToken = tokenManager.getRefreshTokenSync() ?: return null

        return try {
            val authApi = authApiProvider()
            val newTokenResponse = authApi.refreshToken(
                RefreshTokenRequest(refreshToken)
            ).execute()

            if (newTokenResponse.isSuccessful && newTokenResponse.body() != null) {
                val newTokens = newTokenResponse.body()!!
                tokenManager.saveTokensSync(
                    newTokens.accessToken,
                    newTokens.refreshToken
                )

                response.request.newBuilder()
                    .header("Authorization", "Bearer ${newTokens.accessToken}")
                    .build()
            } else {
                null
            }
        } catch (e: Exception) {
            null
        }
    }

    private fun responseCount(response: Response): Int {
        var result = 1
        var prior = response.priorResponse
        while (prior != null) {
            result++
            prior = prior.priorResponse
        }
        return result
    }
}
