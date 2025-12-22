package com.te4it.mobile.data.network.dto

import com.google.gson.annotations.SerializedName

data class PagedResult<T>(
    @SerializedName("items") val items: List<T>,
    @SerializedName("pageNumber") val pageNumber: Int,
    @SerializedName("pageSize") val pageSize: Int,
    @SerializedName("totalCount") val totalCount: Int,
    @SerializedName("totalPages") val totalPages: Int,
    @SerializedName("hasPreviousPage") val hasPreviousPage: Boolean,
    @SerializedName("hasNextPage") val hasNextPage: Boolean
)
