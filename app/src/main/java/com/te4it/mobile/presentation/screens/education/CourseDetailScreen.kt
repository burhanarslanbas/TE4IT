package com.te4it.mobile.presentation.screens.education

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.PlayCircleFilled
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.te4it.mobile.domain.model.education.Course
import com.te4it.mobile.domain.model.education.RoadmapStep
import com.te4it.mobile.presentation.viewmodel.CourseDetailViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CourseDetailScreen(
    navController: NavController,
    viewModel: CourseDetailViewModel,
    courseId: String
) {
    val course by viewModel.course.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val enrollmentStatus by viewModel.enrollmentStatus.collectAsState()

    LaunchedEffect(courseId) {
        viewModel.loadCourseDetails(courseId)
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(course?.title ?: "Kurs Detayı") },
                navigationIcon = {
                    IconButton(onClick = { navController.popBackStack() }) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Geri")
                    }
                }
            )
        }
    ) { padding ->
        Box(modifier = Modifier.padding(padding).fillMaxSize()) {
            if (isLoading) {
                CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
            } else if (error != null) {
                Text("Hata: $error", modifier = Modifier.align(Alignment.Center))
            } else {
                course?.let { currentCourse ->
                    Column {
                        CourseHeader(currentCourse, onEnroll = { viewModel.enrollInCourse(currentCourse.id) })
                        
                        enrollmentStatus?.let {
                            Text(
                                text = it, 
                                color = if (it.contains("Success")) Color.Green else Color.Red,
                                modifier = Modifier.padding(8.dp)
                            )
                        }

                        if (currentCourse.roadmap != null) {
                            LazyColumn(modifier = Modifier.fillMaxWidth()) {
                                item {
                                    Text(
                                        "Müfredat",
                                        style = MaterialTheme.typography.titleLarge,
                                        modifier = Modifier.padding(16.dp)
                                    )
                                }
                                items(currentCourse.roadmap.steps) { step ->
                                    StepItem(step, isEnrolled = currentCourse.isEnrolled) { contentId ->
                                         // Navigate to content
                                         navController.navigate("content_view/${currentCourse.id}/$contentId")
                                    }
                                }
                            }
                        } else {
                             Text("Müfredat bilgisi bulunamadı.", modifier = Modifier.padding(16.dp))
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun CourseHeader(course: Course, onEnroll: () -> Unit) {
    Column(modifier = Modifier.padding(16.dp)) {
        Text(course.description, style = MaterialTheme.typography.bodyLarge)
        Spacer(modifier = Modifier.height(16.dp))
        if (!course.isEnrolled) {
            Button(onClick = onEnroll, modifier = Modifier.fillMaxWidth()) {
                Text("Kursa Kayıt Ol")
            }
        } else {
             OutlinedButton(onClick = {}, modifier = Modifier.fillMaxWidth(), enabled = false) {
                Text("Bu kursa kayıtlısınız")
            }
        }
    }
}

@Composable
fun StepItem(step: RoadmapStep, isEnrolled: Boolean, onContentClick: (String) -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp, vertical = 8.dp),
        colors = CardDefaults.cardColors(
            containerColor = if (step.isLocked) Color.LightGray else MaterialTheme.colorScheme.surface
        )
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                if (step.isLocked) {
                    Icon(Icons.Default.Lock, contentDescription = "Locked")
                } else if (step.isCompleted) {
                    Icon(Icons.Default.CheckCircle, contentDescription = "Completed", tint = Color.Green)
                } else {
                    Icon(Icons.Default.PlayCircleFilled, contentDescription = "Active")
                }
                Spacer(modifier = Modifier.width(8.dp))
                Text(step.title, style = MaterialTheme.typography.titleMedium)
            }
            
            // Show step description if available
            if (!step.description.isNullOrBlank()) {
                Spacer(modifier = Modifier.height(4.dp))
                Text(
                    text = step.description,
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
            
            // Show contents for unlocked steps
            if (!step.isLocked && step.contents.isNotEmpty()) {
                 Spacer(modifier = Modifier.height(8.dp))
                 step.contents.forEach { content ->
                     Row(
                         modifier = Modifier
                            .fillMaxWidth()
                            .clickable(enabled = isEnrolled) { 
                                if (isEnrolled) onContentClick(content.id) 
                            }
                            .padding(vertical = 4.dp),
                         verticalAlignment = Alignment.CenterVertically
                     ) {
                         // Show content type icon
                         val iconText = when {
                             content.embedUrl != null -> "🎥"
                             content.textContent != null -> "📝"
                             else -> "📄"
                         }
                         Text(iconText)
                         Spacer(modifier = Modifier.width(4.dp))
                         Text(
                             text = content.title,
                             style = MaterialTheme.typography.bodyMedium,
                             color = if (isEnrolled) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.onSurfaceVariant
                         )
                         if (!isEnrolled) {
                             Spacer(modifier = Modifier.width(4.dp))
                             Text(
                                 text = "(Kayıt olun)",
                                 style = MaterialTheme.typography.bodySmall,
                                 color = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.6f)
                             )
                         }
                     }
                 }
            }
        }
    }
}
