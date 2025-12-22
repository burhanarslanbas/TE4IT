package com.te4it.mobile.data.local

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
 
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.runBlocking
 

private val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "auth_prefs")
//Samet.123! bunu silme kendime not bıraktım
class TokenManager(
    private val context: Context
) {
    private val dataStore = context.dataStore

    companion object {
        private val ACCESS_TOKEN_KEY = stringPreferencesKey("access_token")
        private val REFRESH_TOKEN_KEY = stringPreferencesKey("refresh_token")
        private val USER_EMAIL_KEY = stringPreferencesKey("user_email")
        private val USER_NAME_KEY = stringPreferencesKey("user_name")
        private val USER_ID_KEY = stringPreferencesKey("user_id")
    }

    suspend fun saveTokens(accessToken: String, refreshToken: String) {
        val userId = decodeUserIdFromToken(accessToken)
        dataStore.edit { preferences ->
            preferences[ACCESS_TOKEN_KEY] = accessToken
            preferences[REFRESH_TOKEN_KEY] = refreshToken
            if (userId != null) {
                preferences[USER_ID_KEY] = userId
            }
        }
    }

    // Kullanıcı bilgilerini saklama
    suspend fun saveUserInfo(email: String, userName: String) {
        dataStore.edit { preferences ->
            preferences[USER_EMAIL_KEY] = email
            preferences[USER_NAME_KEY] = userName
        }
    }

    // Kullanıcı bilgilerini güncelleme
    suspend fun updateUserInfo(userName: String) {
        dataStore.edit { preferences ->
            preferences[USER_NAME_KEY] = userName
        }
    }

    fun getAccessToken(): Flow<String?> {
        return dataStore.data.map { preferences ->
            preferences[ACCESS_TOKEN_KEY]
        }
    }

    fun getRefreshToken(): Flow<String?> {
        return dataStore.data.map { preferences ->
            preferences[REFRESH_TOKEN_KEY]
        }
    }

    // Kullanıcı bilgilerini alma
    fun getUserEmail(): Flow<String?> {
        return dataStore.data.map { preferences ->
            preferences[USER_EMAIL_KEY]
        }
    }

    fun getUserName(): Flow<String?> {
        return dataStore.data.map { preferences ->
            preferences[USER_NAME_KEY]
        }
    }

    fun getUserId(): Flow<String?> {
        return dataStore.data.map { preferences ->
            preferences[USER_ID_KEY]
        }
    }

    suspend fun clearTokens() {
        dataStore.edit { preferences ->
            preferences.remove(ACCESS_TOKEN_KEY)
            preferences.remove(REFRESH_TOKEN_KEY)
            preferences.remove(USER_EMAIL_KEY)
            preferences.remove(USER_NAME_KEY)
            preferences.remove(USER_ID_KEY)
        }
    }

    suspend fun isLoggedIn(): Boolean {
        var token: String? = null
        dataStore.edit {  }
        dataStore.data.map { preferences ->
            token = preferences[ACCESS_TOKEN_KEY]
        }
        return !token.isNullOrEmpty()
    }
    fun getAccessTokenSync(): String? {
        var token: String? = null
        kotlinx.coroutines.runBlocking {
            dataStore.data.firstOrNull()?.let { preferences ->
                token = preferences[ACCESS_TOKEN_KEY]
            }
        }
        return token
    }

    fun getRefreshTokenSync(): String? {
        var token: String? = null
        kotlinx.coroutines.runBlocking {
            dataStore.data.firstOrNull()?.let { preferences ->
                token = preferences[REFRESH_TOKEN_KEY]
            }
        }
        return token
    }

    fun getUserIdSync(): String? {
        var userId: String? = null
        kotlinx.coroutines.runBlocking {
            dataStore.data.firstOrNull()?.let { preferences ->
                userId = preferences[USER_ID_KEY]
            }
        }
        return userId
    }

    fun saveTokensSync(accessToken: String, refreshToken: String) {
        kotlinx.coroutines.runBlocking {
            saveTokens(accessToken, refreshToken)
        }
    }

    private fun decodeUserIdFromToken(token: String): String? {
        try {
            val parts = token.split(".")
            if (parts.size == 3) {
                val payload = parts[1]
                val decodedBytes = android.util.Base64.decode(payload, android.util.Base64.URL_SAFE)
                val decodedString = String(decodedBytes, java.nio.charset.StandardCharsets.UTF_8)
                val jsonObject = org.json.JSONObject(decodedString)
                // Assuming the claim for user ID is "nameid" or "sub" or "id"
                // Adjust based on your JWT structure. Common ones:
                return jsonObject.optString("nameid") 
                    ?: jsonObject.optString("sub")
                    ?: jsonObject.optString("id")
            }
        } catch (e: Exception) {
            e.printStackTrace()
        }
        return null
    }
}