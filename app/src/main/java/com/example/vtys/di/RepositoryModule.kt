package com.example.vtys.di

import com.example.vtys.data.network.ModuleApiService
import com.example.vtys.data.network.ProjectApiService
import com.example.vtys.data.network.TaskApiService
import com.example.vtys.data.network.UseCaseApiService
import com.example.vtys.data.repository.ModuleRepositoryImpl
import com.example.vtys.data.repository.ProjectRepositoryImpl
import com.example.vtys.data.repository.TaskRepositoryImpl
import com.example.vtys.data.repository.UseCaseRepositoryImpl
import com.example.vtys.domain.repository.ModuleRepository
import com.example.vtys.domain.repository.ProjectRepository
import com.example.vtys.domain.repository.TaskRepository
import com.example.vtys.domain.repository.UseCaseRepository
import com.google.gson.Gson

import com.example.vtys.data.local.dao.*

object RepositoryModule {
    fun provideProjectRepository(api: ProjectApiService, dao: ProjectDao): ProjectRepository {
        return ProjectRepositoryImpl(api, dao)
    }

    fun provideModuleRepository(api: ModuleApiService, dao: ModuleDao): ModuleRepository {
        return ModuleRepositoryImpl(api, dao)
    }

    fun provideUseCaseRepository(api: UseCaseApiService, dao: UseCaseDao): UseCaseRepository {
        return UseCaseRepositoryImpl(api, dao)
    }

    fun provideTaskRepository(api: TaskApiService, dao: TaskDao): TaskRepository {
        return TaskRepositoryImpl(api, dao)
    }
}

object GsonModule {
    fun provideGson(): Gson = Gson()
}

