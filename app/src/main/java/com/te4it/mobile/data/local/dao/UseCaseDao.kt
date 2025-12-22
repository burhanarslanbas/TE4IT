package com.te4it.mobile.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.te4it.mobile.data.local.entities.UseCaseEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface UseCaseDao {
    @Query("SELECT * FROM usecases WHERE moduleId = :moduleId ORDER BY startedDate DESC")
    fun getUseCasesByModule(moduleId: String): Flow<List<UseCaseEntity>>

    @Query("SELECT * FROM usecases WHERE id = :useCaseId")
    suspend fun getUseCaseById(useCaseId: String): UseCaseEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertUseCases(useCases: List<UseCaseEntity>)
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertUseCase(useCase: UseCaseEntity)

    @Query("DELETE FROM usecases WHERE moduleId = :moduleId")
    suspend fun clearUseCasesByModule(moduleId: String)
}
