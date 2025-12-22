package com.te4it.mobile.data.network.dto

import com.google.gson.annotations.SerializedName

data class RevokeRefreshTokenRequest(
    @SerializedName("refreshToken") val refreshToken: String
)
