package com.example.vtys.di

import android.content.Context
import com.example.vtys.data.local.AppDatabase
import com.example.vtys.data.local.dao.*

object DatabaseModule {
    fun provideDatabase(context: Context): AppDatabase {
        return AppDatabase.getDatabase(context)
    }

    fun provideProjectDao(database: AppDatabase): ProjectDao {
        return database.projectDao()
    }

    fun provideModuleDao(database: AppDatabase): ModuleDao {
        return database.moduleDao()
    }

    fun provideUseCaseDao(database: AppDatabase): UseCaseDao {
        return database.useCaseDao()
    }

    fun provideTaskDao(database: AppDatabase): TaskDao {
        return database.taskDao()
    }
    
    fun provideTaskSyncQueueDao(database: AppDatabase): TaskSyncQueueDao {
        return database.taskSyncQueueDao()
    }
}
