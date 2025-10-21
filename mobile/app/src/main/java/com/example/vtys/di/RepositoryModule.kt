package com.example.vtys.di

import com.google.gson.Gson

object RepositoryModule

object GsonModule {
    fun provideGson(): Gson = Gson()
}

