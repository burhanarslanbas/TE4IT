package com.te4it.mobile.data.network.dto

import com.google.gson.annotations.SerializedName

data class TaskDto(
    @SerializedName("id") val id: String,
    @SerializedName("useCaseId") val useCaseId: String,
    @SerializedName("creatorId") val creatorId: String,
    @SerializedName("assigneeId") val assigneeId: String?,
    @SerializedName("assigneeName") val assigneeName: String?,
    @SerializedName("title") val title: String,
    @SerializedName("description") val description: String?,
    @SerializedName("importantNotes") val importantNotes: String?,
    @SerializedName("startedDate") val startedDate: String?,
    @SerializedName("dueDate") val dueDate: String?,
    @SerializedName("taskType") val taskType: Int, // Enum value as Int
    @SerializedName("taskState") val taskState: Int, // Enum value as Int
    @SerializedName("relations") val relations: List<TaskRelationDto> = emptyList()
)

data class TaskRelationDto(
    @SerializedName("id") val id: String,
    @SerializedName("targetTaskId") val targetTaskId: String,
    @SerializedName("relationType") val relationType: Int, // Enum value as Int
    @SerializedName("targetTaskTitle") val targetTaskTitle: String
)

data class ChangeTaskStateRequest(
    @SerializedName("newState") val newState: Int
)

enum class TaskType(val value: Int) {
    DOCUMENTATION(1),
    FEATURE(2),
    TEST(3),
    BUG(4);

    companion object {
        fun fromValue(value: Int) = entries.firstOrNull { it.value == value }
    }
}

enum class TaskState(val value: Int) {
    NOT_STARTED(1),
    IN_PROGRESS(2),
    COMPLETED(3),
    CANCELLED(4);

    companion object {
        fun fromValue(value: Int) = entries.firstOrNull { it.value == value }
    }
}

enum class TaskRelationType(val value: Int) {
    BLOCKS(1),
    RELATES_TO(2),
    FIXES(3),
    DUPLICATES(4);

    companion object {
        fun fromValue(value: Int) = entries.firstOrNull { it.value == value }
    }
}
