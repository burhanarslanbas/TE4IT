package com.example.vtys.data.network.dto

import com.google.gson.annotations.SerializedName

data class RevokeRefreshTokenRequest(
    @SerializedName("refreshToken") val refreshToken: String
)
