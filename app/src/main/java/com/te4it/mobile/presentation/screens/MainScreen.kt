package com.te4it.mobile.presentation.screens

import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Assignment
import androidx.compose.material.icons.filled.Folder
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.School
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory

sealed class BottomNavItem(val route: String, val icon: androidx.compose.ui.graphics.vector.ImageVector, val label: String) {
    object Dashboard : BottomNavItem("dashboard", Icons.Default.Home, "Anasayfa")
    object Projects : BottomNavItem("projects", Icons.Default.Folder, "Projeler")
    object Tasks : BottomNavItem("tasks", Icons.Default.Assignment, "Görevler")
    object Education : BottomNavItem("education", androidx.compose.material.icons.Icons.Default.School, "Eğitim")
    object Profile : BottomNavItem("profile", Icons.Default.Person, "Profil")
}

@Composable
fun MainScreen(
    viewModelFactory: ViewModelFactory,
    onNavigateToProjectDetail: (String) -> Unit,
    onNavigateToTaskDetail: (String) -> Unit,
    onNavigateToNotifications: () -> Unit,
    onNavigateToSettings: () -> Unit,
    onNavigateToAbout: () -> Unit,
    onNavigateToEditProfile: () -> Unit,
    onNavigateToChangePassword: () -> Unit,
    onLogout: () -> Unit,
    onNavigateToCourseDetail: (String) -> Unit
) {
    val navController = rememberNavController()
    val items = listOf(
        BottomNavItem.Dashboard,
        BottomNavItem.Projects,
        BottomNavItem.Tasks,
        BottomNavItem.Education,
        BottomNavItem.Profile
    )

    Scaffold(
        bottomBar = {
            NavigationBar {
                val navBackStackEntry by navController.currentBackStackEntryAsState()
                val currentRoute = navBackStackEntry?.destination?.route

                items.forEach { item ->
                     NavigationBarItem(
                        icon = { Icon(item.icon, contentDescription = item.label) },
                        label = { Text(item.label) },
                        selected = currentRoute == item.route,
                        onClick = {
                            navController.navigate(item.route) {
                                popUpTo(navController.graph.findStartDestination().id) {
                                    saveState = true
                                }
                                launchSingleTop = true
                                restoreState = true
                            }
                        }
                    )
                }
            }
        }
    ) { innerPadding ->
        NavHost(
            navController = navController,
            startDestination = BottomNavItem.Dashboard.route,
            modifier = Modifier.padding(innerPadding)
        ) {
            composable(BottomNavItem.Dashboard.route) {
                DashboardScreen(
                    viewModelFactory = viewModelFactory,
                    onNavigateToProjectList = {
                         navController.navigate(BottomNavItem.Projects.route) {
                            popUpTo(navController.graph.findStartDestination().id) {
                                saveState = true
                            }
                            launchSingleTop = true
                            restoreState = true
                        }
                    },
                    onNavigateToProjectDetail = onNavigateToProjectDetail,
                    onNavigateToTaskDetail = onNavigateToTaskDetail,
                    onNavigateToProfile = {
                        navController.navigate(BottomNavItem.Profile.route) {
                            popUpTo(navController.graph.findStartDestination().id) {
                                saveState = true
                            }
                            launchSingleTop = true
                            restoreState = true
                        }
                    },
                    onNavigateToNotifications = onNavigateToNotifications
                )
            }
            composable(BottomNavItem.Projects.route) {
                ProjectListScreen(
                    viewModelFactory = viewModelFactory,
                    onNavigateBack = { navController.navigate(BottomNavItem.Dashboard.route) },
                    onNavigateToProjectDetail = onNavigateToProjectDetail
                )
            }
            composable(BottomNavItem.Tasks.route) {
                TaskListScreen(
                    viewModelFactory = viewModelFactory,
                    onNavigateBack = { navController.navigate(BottomNavItem.Dashboard.route) },
                    onNavigateToTaskDetail = onNavigateToTaskDetail
                )
            }
            composable(BottomNavItem.Education.route) {
                val viewModel: com.te4it.mobile.presentation.viewmodel.EducationViewModel = androidx.lifecycle.viewmodel.compose.viewModel(factory = viewModelFactory)
                com.te4it.mobile.presentation.screens.education.EducationDashboardScreen(
                    navController = androidx.navigation.compose.rememberNavController(), // Dummy controller or fix EducationScreen to take callback
                    viewModel = viewModel
                )
                // Fix: EducationDashboardScreen needs proper callbacks or pass main navController if compatible.
                // The EducationDashboardScreen expects a NavController to navigate to details.
                // But this nested NavHost's controller is local. We should pass the external navigation callback to MainScreen.
                // Wait, logic: MainScreen creates a NavHost. EducationDashboardScreen is inside it. 
                // When we click course, we want to go to CourseDetail. CourseDetail is defined in original NavGraph. 
                // BUT MainScreen is a composable inside NavGraph. 
                // So onNavigateToCourseDetail needs to be passed up.
                
                // Redoing EducationDashboardScreen to accept callback instead of NavController is better, but for now let's wrap it.
                 com.te4it.mobile.presentation.screens.education.MainScreenEducationWrapper(
                     viewModel = viewModel,
                     onNavigateToCourseDetail = onNavigateToCourseDetail
                 )
            }
            composable(BottomNavItem.Profile.route) {
                com.te4it.mobile.presentation.screens.profile.ProfileScreen(
                    viewModelFactory = viewModelFactory,
                    onNavigateToSettings = onNavigateToSettings,
                    onNavigateToAbout = onNavigateToAbout,
                    onNavigateToEditProfile = onNavigateToEditProfile,
                    onNavigateToChangePassword = onNavigateToChangePassword,
                    onLogout = onLogout
                )
            }
        }
    }
}

