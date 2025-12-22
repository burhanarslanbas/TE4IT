package com.te4it.mobile

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.navigation.compose.rememberNavController
import com.te4it.mobile.presentation.navigation.NavGraph
import com.te4it.mobile.ui.theme.VtysTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            VtysTheme {
                val navController = rememberNavController()
                NavGraph(navController = navController, context = this)
            }
        }
    }
}