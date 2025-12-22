package com.te4it.mobile.di

import com.te4it.mobile.data.network.ModuleApiService
import com.te4it.mobile.data.network.ProjectApiService
import com.te4it.mobile.data.network.TaskApiService
import com.te4it.mobile.data.network.UseCaseApiService
import com.te4it.mobile.data.repository.ModuleRepositoryImpl
import com.te4it.mobile.data.repository.ProjectRepositoryImpl
import com.te4it.mobile.data.repository.TaskRepositoryImpl
import com.te4it.mobile.data.repository.UseCaseRepositoryImpl
import com.te4it.mobile.domain.repository.ModuleRepository
import com.te4it.mobile.domain.repository.ProjectRepository
import com.te4it.mobile.domain.repository.TaskRepository
import com.te4it.mobile.domain.repository.UseCaseRepository
import com.google.gson.Gson

import com.te4it.mobile.data.local.dao.*

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

    fun provideEducationRepository(api: com.te4it.mobile.data.network.EducationApiService): com.te4it.mobile.domain.repository.EducationRepository {
        return com.te4it.mobile.data.repository.EducationRepositoryImpl(api)
    }
}

object GsonModule {
    fun provideGson(): Gson = Gson()
}

