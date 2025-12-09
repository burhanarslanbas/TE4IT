package com.example.vtys.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.example.vtys.data.local.entities.ModuleEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface ModuleDao {
    @Query("SELECT * FROM modules WHERE projectId = :projectId ORDER BY startedDate DESC")
    fun getModulesByProject(projectId: String): Flow<List<ModuleEntity>>

    @Query("SELECT * FROM modules WHERE id = :moduleId")
    suspend fun getModuleById(moduleId: String): ModuleEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertModules(modules: List<ModuleEntity>)
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertModule(module: ModuleEntity)

    @Query("DELETE FROM modules WHERE projectId = :projectId")
    suspend fun clearModulesByProject(projectId: String)
}
