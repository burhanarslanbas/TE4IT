package com.example.vtys.data.network.dto

import com.google.gson.annotations.SerializedName

data class UseCaseDto(
    @SerializedName("id") val id: String,
    @SerializedName("moduleId") val moduleId: String,
    @SerializedName("title") val title: String,
    @SerializedName("description") val description: String?,
    @SerializedName("importantNotes") val importantNotes: String?,
    @SerializedName("isActive") val isActive: Boolean,
    @SerializedName("startedDate") val startedDate: String,
    @SerializedName("taskCount") val taskCount: Int = 0
)
