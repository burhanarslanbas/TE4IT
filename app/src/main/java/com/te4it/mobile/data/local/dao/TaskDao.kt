package com.te4it.mobile.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.te4it.mobile.data.local.entities.TaskEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface TaskDao {
    @Query("SELECT * FROM tasks WHERE useCaseId = :useCaseId ORDER BY startedDate DESC")
    fun getTasksByUseCase(useCaseId: String): Flow<List<TaskEntity>>

    @Query("SELECT * FROM tasks WHERE id = :taskId")
    suspend fun getTaskById(taskId: String): TaskEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertTasks(tasks: List<TaskEntity>)
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertTask(task: TaskEntity)
    
    @Query("UPDATE tasks SET taskState = :newState WHERE id = :taskId")
    suspend fun updateTaskState(taskId: String, newState: Int)

    @Query("SELECT * FROM tasks WHERE assigneeId = :assigneeId ORDER BY startedDate DESC")
    fun getTasksByAssignee(assigneeId: String): Flow<List<TaskEntity>>

    @Query("DELETE FROM tasks WHERE useCaseId = :useCaseId")
    suspend fun clearTasksByUseCase(useCaseId: String)
}
