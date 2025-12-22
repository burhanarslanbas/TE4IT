package com.te4it.mobile.data.network.dto

import com.google.gson.annotations.SerializedName

/**
 * RFC 7807 Problem Details formatına uygun hata yanıt modeli
 */
data class ErrorResponse(
    @SerializedName("type")
    val type: String? = null,
    @SerializedName("title")
    val title: String? = null,
    @SerializedName("status")
    val status: Int? = null,
    @SerializedName("detail")
    val detail: String? = null,
    @SerializedName("instance")
    val instance: String? = null
)

