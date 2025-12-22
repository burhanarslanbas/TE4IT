package com.te4it.mobile.data.network.dto

import com.google.gson.annotations.SerializedName

data class ResetPasswordCommand(
    @SerializedName("email")
    val email: String,
    @SerializedName("token")
    val token: String,
    @SerializedName("newPassword")
    val newPassword: String
)