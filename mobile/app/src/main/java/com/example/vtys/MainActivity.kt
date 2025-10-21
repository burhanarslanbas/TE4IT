package com.example.vtys

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.navigation.compose.rememberNavController
import com.example.vtys.presentation.navigation.NavGraph
import com.example.vtys.ui.theme.VtysTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            VtysTheme {
                val navController = rememberNavController()
                NavGraph(navController = navController)
            }
        }
    }
}