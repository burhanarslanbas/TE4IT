package com.te4it.mobile.presentation.navigation

import android.content.Context
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.remember
import androidx.navigation.NavHostController
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.navArgument
import com.te4it.mobile.di.NetworkModule
import com.te4it.mobile.di.RepositoryModule
import com.te4it.mobile.presentation.screens.*
import com.te4it.mobile.presentation.screens.profile.ChangePasswordScreen
import com.te4it.mobile.presentation.screens.profile.EditProfileScreen
import com.te4it.mobile.presentation.screens.profile.ProfileScreen
import com.te4it.mobile.presentation.viewmodel.ViewModelFactory

sealed class Screen(val route: String) {
    data object Login : Screen("login")
    data object Register : Screen("register")
    data object Home : Screen("home")
    data object Profile : Screen("profile")
    data object EditProfile : Screen("edit_profile")
    data object ChangePassword : Screen("change_password")
    data object ProjectList : Screen("project_list")
    data object ProjectDetail : Screen("project_detail/{projectId}") {
        fun createRoute(projectId: String) = "project_detail/$projectId"
    }
    data object ModuleDetail : Screen("module_detail/{moduleId}") {
        fun createRoute(moduleId: String) = "module_detail/$moduleId"
    }
    data object UseCaseDetail : Screen("use_case_detail/{useCaseId}") {
        fun createRoute(useCaseId: String) = "use_case_detail/$useCaseId"
    }
    data object TaskDetail : Screen("task_detail/{taskId}") {
        fun createRoute(taskId: String) = "task_detail/$taskId"
    }
    data object Settings : Screen("settings")
    data object About : Screen("about")
    data object Notifications : Screen("notifications")
    data object EducationList : Screen("education_list")
    data object CourseDetail : Screen("course_detail/{courseId}") {
        fun createRoute(courseId: String) = "course_detail/$courseId"
    }
    data object Content : Screen("content_view/{courseId}/{contentId}") {
        fun createRoute(courseId: String, contentId: String) = "content_view/$courseId/$contentId"
    }
}

@Composable
fun NavGraph(
    navController: NavHostController,
    startDestination: String = Screen.Login.route,
    context: Context
) {
    // Dependencies
    val tokenManager = remember { NetworkModule.provideTokenManager(context) }
    val authInterceptor = remember { NetworkModule.provideAuthInterceptor(tokenManager) }
    val loggingInterceptor = remember { NetworkModule.provideLoggingInterceptor() }
    
    // Circular dependency handling for TokenAuthenticator
    // Simple mutable holder for the lambda
    val authApiHolder = remember { 
        object {
            var api: com.te4it.mobile.data.network.AuthApiService? = null
        }
    }
    
    val authApiProvider: () -> com.te4it.mobile.data.network.AuthApiService = remember {
        { authApiHolder.api!! }
    }
    
    val tokenAuthenticator = remember { NetworkModule.provideTokenAuthenticator(tokenManager, authApiProvider) }
    val okHttpClient = remember { NetworkModule.provideOkHttpClient(loggingInterceptor, authInterceptor, tokenAuthenticator) }
    val retrofit = remember { NetworkModule.provideRetrofit(okHttpClient) }
    
    val authApi = remember { NetworkModule.provideAuthApiService(retrofit) }
    // Initialize the provider
    LaunchedEffect(authApi) {
        authApiHolder.api = authApi
    }

    val projectApi = remember { NetworkModule.provideProjectApiService(retrofit) }
    val moduleApi = remember { NetworkModule.provideModuleApiService(retrofit) }
    val useCaseApi = remember { NetworkModule.provideUseCaseApiService(retrofit) }
    val taskApi = remember { NetworkModule.provideTaskApiService(retrofit) }
    val educationApi = remember { NetworkModule.provideEducationApiService(retrofit) }

    // Database & DAOs
    val database = remember { com.te4it.mobile.di.DatabaseModule.provideDatabase(context) }
    val projectDao = remember { com.te4it.mobile.di.DatabaseModule.provideProjectDao(database) }
    val moduleDao = remember { com.te4it.mobile.di.DatabaseModule.provideModuleDao(database) }
    val useCaseDao = remember { com.te4it.mobile.di.DatabaseModule.provideUseCaseDao(database) }
    val taskDao = remember { com.te4it.mobile.di.DatabaseModule.provideTaskDao(database) }

    val projectRepository = remember { RepositoryModule.provideProjectRepository(projectApi, projectDao) }
    val moduleRepository = remember { RepositoryModule.provideModuleRepository(moduleApi, moduleDao) }
    val useCaseRepository = remember { RepositoryModule.provideUseCaseRepository(useCaseApi, useCaseDao) }
    val taskRepository = remember { RepositoryModule.provideTaskRepository(taskApi, taskDao) }
    val educationRepository = remember { RepositoryModule.provideEducationRepository(educationApi) }

    val viewModelFactory = remember {
        ViewModelFactory(
            projectRepository,
            moduleRepository,
            useCaseRepository,
            taskRepository,
            educationRepository,
            tokenManager
        )
    }

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
            MainScreen(
                viewModelFactory = viewModelFactory,
                onNavigateToProjectDetail = { projectId ->
                    navController.navigate(Screen.ProjectDetail.createRoute(projectId))
                },
                onNavigateToTaskDetail = { taskId ->
                    navController.navigate(Screen.TaskDetail.createRoute(taskId))
                },
                onNavigateToNotifications = {
                    navController.navigate(Screen.Notifications.route)
                },
                onNavigateToSettings = {
                    navController.navigate(Screen.Settings.route)
                },
                onNavigateToAbout = {
                    navController.navigate(Screen.About.route)
                },
                onNavigateToEditProfile = {
                    navController.navigate(Screen.EditProfile.route)
                },
                onNavigateToChangePassword = {
                    navController.navigate(Screen.ChangePassword.route)
                },
                onLogout = {
                    navController.navigate(Screen.Login.route) {
                        popUpTo(Screen.Home.route) { inclusive = true }
                    }
                },
                onNavigateToCourseDetail = { courseId ->
                    navController.navigate(Screen.CourseDetail.createRoute(courseId))
                }
            )
        }

        composable(
            route = Screen.ProjectDetail.route,
            arguments = listOf(navArgument("projectId") { type = NavType.StringType })
        ) { backStackEntry ->
            val projectId = backStackEntry.arguments?.getString("projectId") ?: return@composable
            ProjectDetailScreen(
                projectId = projectId,
                viewModelFactory = viewModelFactory,
                onNavigateBack = { navController.popBackStack() },
                onNavigateToModuleDetail = { moduleId ->
                    navController.navigate(Screen.ModuleDetail.createRoute(moduleId))
                }
            )
        }

        composable(
            route = Screen.ModuleDetail.route,
            arguments = listOf(navArgument("moduleId") { type = NavType.StringType })
        ) { backStackEntry ->
            val moduleId = backStackEntry.arguments?.getString("moduleId") ?: return@composable
            ModuleDetailScreen(
                moduleId = moduleId,
                viewModelFactory = viewModelFactory,
                onNavigateBack = { navController.popBackStack() },
                onNavigateToUseCaseDetail = { useCaseId ->
                    navController.navigate(Screen.UseCaseDetail.createRoute(useCaseId))
                }
            )
        }

        composable(
            route = Screen.UseCaseDetail.route,
            arguments = listOf(navArgument("useCaseId") { type = NavType.StringType })
        ) { backStackEntry ->
            val useCaseId = backStackEntry.arguments?.getString("useCaseId") ?: return@composable
            UseCaseDetailScreen(
                useCaseId = useCaseId,
                viewModelFactory = viewModelFactory,
                onNavigateBack = { navController.popBackStack() },
                onNavigateToTaskDetail = { taskId ->
                    navController.navigate(Screen.TaskDetail.createRoute(taskId))
                }
            )
        }

        composable(
            route = Screen.TaskDetail.route,
            arguments = listOf(navArgument("taskId") { type = NavType.StringType })
        ) { backStackEntry ->
            val taskId = backStackEntry.arguments?.getString("taskId") ?: return@composable
            TaskDetailScreen(
                taskId = taskId,
                viewModelFactory = viewModelFactory,
                onNavigateBack = { navController.popBackStack() }
            )
        }



        composable(Screen.ChangePassword.route) {
            ChangePasswordScreen(
                navController = navController
            )
        }
        
        composable(Screen.EditProfile.route) {
            EditProfileScreen(
                navController = navController,
                tokenManager = tokenManager
            )
        }
        
        composable(Screen.Settings.route) {
            com.te4it.mobile.presentation.screens.profile.SettingsScreen(
                navController = navController
            )
        }
        
        composable(Screen.About.route) {
            com.te4it.mobile.presentation.screens.profile.AboutScreen(
                navController = navController
            )
        }

        composable(Screen.Notifications.route) {
            NotificationScreen(
                onNavigateBack = { navController.popBackStack() }
            )
        }

        composable(Screen.EducationList.route) {
            val viewModel: com.te4it.mobile.presentation.viewmodel.EducationViewModel = androidx.lifecycle.viewmodel.compose.viewModel(factory = viewModelFactory)
            com.te4it.mobile.presentation.screens.education.EducationDashboardScreen(
                navController = navController,
                viewModel = viewModel
            )
        }

        composable(
            route = Screen.CourseDetail.route,
            arguments = listOf(navArgument("courseId") { type = NavType.StringType })
        ) { backStackEntry ->
             val courseId = backStackEntry.arguments?.getString("courseId") ?: return@composable
             val viewModel: com.te4it.mobile.presentation.viewmodel.CourseDetailViewModel = androidx.lifecycle.viewmodel.compose.viewModel(factory = viewModelFactory)
             com.te4it.mobile.presentation.screens.education.CourseDetailScreen(
                 navController = navController,
                 viewModel = viewModel,
                 courseId = courseId
             )
        }

        composable(
            route = Screen.Content.route,
            arguments = listOf(
                navArgument("courseId") { type = NavType.StringType },
                navArgument("contentId") { type = NavType.StringType }
            )
        ) { backStackEntry ->
            val courseId = backStackEntry.arguments?.getString("courseId") ?: return@composable
            val contentId = backStackEntry.arguments?.getString("contentId") ?: return@composable
            
            // ViewModel will be created inside ContentScreen or passed via factory
            // For now, we update the Screen signature
            val viewModel: com.te4it.mobile.presentation.viewmodel.ContentPlayerViewModel = androidx.lifecycle.viewmodel.compose.viewModel(factory = viewModelFactory)
            
            com.te4it.mobile.presentation.screens.education.ContentScreen(
                courseId = courseId,
                contentId = contentId,
                viewModel = viewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
    }
}