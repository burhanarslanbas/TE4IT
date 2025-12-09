package com.example.vtys.data.local

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import com.example.vtys.data.local.dao.*
import com.example.vtys.data.local.entities.*

@Database(
    entities = [
        ProjectEntity::class,
        ModuleEntity::class,
        UseCaseEntity::class,
        TaskEntity::class,
        TaskSyncQueueEntity::class
    ],
    version = 1,
    exportSchema = false
)
abstract class AppDatabase : RoomDatabase() {
    abstract fun projectDao(): ProjectDao
    abstract fun moduleDao(): ModuleDao
    abstract fun useCaseDao(): UseCaseDao
    abstract fun taskDao(): TaskDao
    abstract fun taskSyncQueueDao(): TaskSyncQueueDao

    companion object {
        @Volatile
        private var INSTANCE: AppDatabase? = null

        fun getDatabase(context: Context): AppDatabase {
            return INSTANCE ?: synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    AppDatabase::class.java,
                    "vtys_database"
                ).build()
                INSTANCE = instance
                instance
            }
        }
    }
}
