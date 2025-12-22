package com.te4it.mobile.di

import android.content.Context
import com.te4it.mobile.data.local.TokenManager
import com.te4it.mobile.data.network.AuthApiService
import com.te4it.mobile.data.network.AuthInterceptor
import com.te4it.mobile.data.network.TokenAuthenticator
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

object NetworkModule {

    private const val BASE_URL = "https://te4it-api.azurewebsites.net/api/v1/"

    fun provideLoggingInterceptor(): HttpLoggingInterceptor {
        return HttpLoggingInterceptor().apply {
            level = HttpLoggingInterceptor.Level.BODY
        }
    }

    fun provideAuthInterceptor(tokenManager: TokenManager): AuthInterceptor {
        return AuthInterceptor(tokenManager)
    }

    fun provideTokenAuthenticator(
        tokenManager: TokenManager,
        authApiProvider: () -> AuthApiService
    ): TokenAuthenticator {
        return TokenAuthenticator(tokenManager, authApiProvider)
    }

    fun provideOkHttpClient(
        loggingInterceptor: HttpLoggingInterceptor,
        authInterceptor: AuthInterceptor,
        tokenAuthenticator: TokenAuthenticator
    ): OkHttpClient {
        return OkHttpClient.Builder()
            .addInterceptor(loggingInterceptor)
            .addInterceptor(authInterceptor)
            .authenticator(tokenAuthenticator)
            .connectTimeout(30, TimeUnit.SECONDS)
            .readTimeout(30, TimeUnit.SECONDS)
            .writeTimeout(30, TimeUnit.SECONDS)
            .build()
    }

    fun provideRetrofit(okHttpClient: OkHttpClient): Retrofit {
        return Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
    }

    fun provideAuthApiService(retrofit: Retrofit): AuthApiService {
        return retrofit.create(AuthApiService::class.java)
    }

    fun provideProjectApiService(retrofit: Retrofit): com.te4it.mobile.data.network.ProjectApiService {
        return retrofit.create(com.te4it.mobile.data.network.ProjectApiService::class.java)
    }

    fun provideModuleApiService(retrofit: Retrofit): com.te4it.mobile.data.network.ModuleApiService {
        return retrofit.create(com.te4it.mobile.data.network.ModuleApiService::class.java)
    }

    fun provideUseCaseApiService(retrofit: Retrofit): com.te4it.mobile.data.network.UseCaseApiService {
        return retrofit.create(com.te4it.mobile.data.network.UseCaseApiService::class.java)
    }

    fun provideTaskApiService(retrofit: Retrofit): com.te4it.mobile.data.network.TaskApiService {
        return retrofit.create(com.te4it.mobile.data.network.TaskApiService::class.java)
    }

    fun provideEducationApiService(retrofit: Retrofit): com.te4it.mobile.data.network.EducationApiService {
        return retrofit.create(com.te4it.mobile.data.network.EducationApiService::class.java)
    }

    fun provideTokenManager(context: Context): TokenManager {
        return TokenManager(context)
    }
}