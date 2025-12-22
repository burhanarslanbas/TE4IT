package com.te4it.mobile.data.network.dto

import com.google.gson.annotations.SerializedName

data class ProjectDto(
    @SerializedName("id") val id: String,
    @SerializedName("title") val title: String,
    @SerializedName("description") val description: String?,
    @SerializedName("isActive") val isActive: Boolean,
    @SerializedName("startedDate") val startedDate: String,
    @SerializedName("memberCount") val memberCount: Int = 0,
    @SerializedName("moduleCount") val moduleCount: Int = 0
)

data class ProjectDetailDto(
    @SerializedName("id") val id: String,
    @SerializedName("title") val title: String,
    @SerializedName("description") val description: String?,
    @SerializedName("isActive") val isActive: Boolean,
    @SerializedName("startedDate") val startedDate: String
)

data class ProjectMemberDto(
    @SerializedName("userId") val userId: String,
    @SerializedName("email") val email: String,
    @SerializedName("userName") val userName: String,
    @SerializedName("role") val role: Int,
    @SerializedName("roleName") val roleName: String? = null,
    @SerializedName("joinedDate") val joinedDate: String
)
