package com.example.vtys.presentation.navigation

import android.content.Context
import androidx.compose.runtime.Composable
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import com.example.vtys.presentation.screens.LoginScreen
import com.example.vtys.presentation.screens.RegisterScreen
import com.example.vtys.presentation.screens.TempHomeScreen
import com.example.vtys.presentation.screens.profile.ChangePasswordScreen
import com.example.vtys.presentation.screens.profile.EditProfileScreen
import com.example.vtys.presentation.screens.profile.ProfileScreen
import com.example.vtys.data.local.TokenManager
import com.example.vtys.di.NetworkModule

sealed class Screen(val route: String) {
    data object Login : Screen("login")
    data object Register : Screen("register")
    data object Home : Screen("home")
    data object Profile : Screen("profile")
    data object EditProfile : Screen("edit_profile")
    data object ChangePassword : Screen("change_password")
}

@Composable
fun NavGraph(
    navController: NavHostController,
    startDestination: String = Screen.Login.route,
    context: Context // Context parametresi ekledik
) {
    NavHost(
        navController = navController,
        startDestination = startDestination
    ) {
        composable(Screen.Login.route) {
            LoginScreen(
                onNavigateToRegister = {
                    navController.navigate(Screen.Register.route)
                },
                onLoginSuccess = {
                    navController.navigate(Screen.Home.route) {
                        popUpTo(Screen.Login.route) { inclusive = true }
                    }
                }
            )
        }

        composable(Screen.Register.route) {
            RegisterScreen(
                onNavigateToLogin = {
                    navController.popBackStack()
                },
                onRegisterSuccess = {
                    navController.navigate(Screen.Home.route) {
                        popUpTo(Screen.Login.route) { inclusive = true }
                    }
                }
            )
        }

        composable(Screen.Home.route) {
            TempHomeScreen(
                onNavigateToProfile = {
                    navController.navigate(Screen.Profile.route)
                }
            )
        }

        composable(Screen.Profile.route) {
            ProfileScreen(
                navController = navController,
                onLogout = {
                    navController.navigate(Screen.Login.route) {
                        popUpTo(Screen.Profile.route) { inclusive = true }
                    }
                }
            )
        }

        composable(Screen.ChangePassword.route) {
            ChangePasswordScreen(
                navController = navController
            )
        }
        
        composable(Screen.EditProfile.route) {
            val tokenManager = NetworkModule.provideTokenManager(context)
            EditProfileScreen(
                navController = navController,
                tokenManager = tokenManager
            )
        }
    }
}