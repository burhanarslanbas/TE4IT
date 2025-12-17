package com.example.vtys.data.network.dto

import com.google.gson.annotations.SerializedName

data class ModuleDto(
    @SerializedName("id") val id: String,
    @SerializedName("projectId") val projectId: String? = null,
    @SerializedName("title") val title: String,
    @SerializedName("description") val description: String?,
    @SerializedName("isActive") val isActive: Boolean,
    @SerializedName("startedDate") val startedDate: String,
    @SerializedName("useCaseCount") val useCaseCount: Int = 0
)
