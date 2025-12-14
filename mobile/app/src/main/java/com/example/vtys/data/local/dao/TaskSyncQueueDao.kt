package com.example.vtys.data.local.dao

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.example.vtys.data.local.entities.TaskSyncQueueEntity

@Dao
interface TaskSyncQueueDao {
    @Query("SELECT * FROM task_sync_queue ORDER BY timestamp ASC")
    suspend fun getAllPendingSyncs(): List<TaskSyncQueueEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertSync(sync: TaskSyncQueueEntity)

    @Delete
    suspend fun deleteSync(sync: TaskSyncQueueEntity)
}
