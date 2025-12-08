# Mobile API Integration Guide - TE4IT

> **Hedef:** Android Kotlin Developers  
> **Versiyon:** 2.0  
> **Güncelleme:** 2024-12-08  
> **Odak:** Read-Only Dashboard App

## 🎯 Mobil Uygulama Yaklaşımı

Bu dokümantasyon, **monitoring ve dashboard odaklı** mobil uygulama için API entegrasyonunu açıklar.

### Temel Prensipler
- ✅ **Read-Heavy:** %95 okuma, %5 yazma
- ✅ **Tek Yazma İşlemi:** Task durum değiştirme
- ✅ **Offline-First:** Cache-first yaklaşım
- ✅ **Dashboard Fokus:** Metrikler ve özet bilgiler

---

## 📡 API Endpoints Kategorileri

### 1. Authentication (Auth)
```
✅ POST   /api/v1/auth/login
✅ POST   /api/v1/auth/register
✅ POST   /api/v1/auth/refreshToken
✅ POST   /api/v1/auth/changePassword
✅ POST   /api/v1/auth/revokeRefreshToken
✅ GET    /api/v1/auth/me
```

### 2. Projects (Read-Only)
```
✅ GET    /api/v1/projects
✅ GET    /api/v1/projects/{id}
✅ GET    /api/v1/projects/{projectId}/members
❌ POST   /api/v1/projects                        (Mobilde YOK)
❌ PUT    /api/v1/projects/{id}                   (Mobilde YOK)
❌ DELETE /api/v1/projects/{id}                   (Mobilde YOK)
```

### 3. Modules (Read-Only)
```
✅ GET    /api/v1/modules/projects/{projectId}
✅ GET    /api/v1/modules/{id}
❌ POST   /api/v1/modules/projects/{projectId}    (Mobilde YOK)
❌ PUT    /api/v1/modules/{id}                    (Mobilde YOK)
❌ DELETE /api/v1/modules/{id}                    (Mobilde YOK)
```

### 4. Use Cases (Read-Only)
```
✅ GET    /api/v1/usecases/modules/{moduleId}
✅ GET    /api/v1/usecases/{id}
❌ POST   /api/v1/usecases/modules/{moduleId}     (Mobilde YOK)
❌ PUT    /api/v1/usecases/{id}                   (Mobilde YOK)
❌ DELETE /api/v1/usecases/{id}                   (Mobilde YOK)
```

### 5. Tasks (Read + Limited Write)
```
✅ GET    /api/v1/tasks/usecases/{useCaseId}
✅ GET    /api/v1/tasks/{id}
✅ GET    /api/v1/tasks/{taskId}/relations
⚠️  PATCH  /api/v1/tasks/{id}/state               ← TEK YAZMA İŞLEMİ!
❌ POST   /api/v1/tasks/usecases/{useCaseId}      (Mobilde YOK)
❌ PUT    /api/v1/tasks/{id}                      (Mobilde YOK)
❌ POST   /api/v1/tasks/{id}/assign                (Mobilde YOK)
❌ DELETE /api/v1/tasks/{id}                      (Mobilde YOK)
```

---

## 🔧 Retrofit Setup

### 1. Dependencies

```kotlin
// app/build.gradle.kts
dependencies {
    // Networking
    implementation("com.squareup.retrofit2:retrofit:2.9.0")
    implementation("com.squareup.retrofit2:converter-gson:2.9.0")
    implementation("com.squareup.okhttp3:okhttp:4.12.0")
    implementation("com.squareup.okhttp3:logging-interceptor:4.12.0")
    
    // Coroutines
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.7.3")
    
    // Dependency Injection
    implementation("com.google.dagger:hilt-android:2.48")
    kapt("com.google.dagger:hilt-compiler:2.48")
    
    // DataStore (Token storage)
    implementation("androidx.datastore:datastore-preferences:1.0.0")
    
    // Room (Caching)
    implementation("androidx.room:room-runtime:2.6.0")
    implementation("androidx.room:room-ktx:2.6.0")
    kapt("androidx.room:room-compiler:2.6.0")
}
```

### 2. API Configuration

```kotlin
// network/ApiConfig.kt
object ApiConfig {
    const val BASE_URL = "https://te4it-api.azurewebsites.net"
    const val API_VERSION = "v1"
    const val TIMEOUT_CONNECT = 30L
    const val TIMEOUT_READ = 30L
    const val TIMEOUT_WRITE = 30L
}
```

### 3. Auth Interceptor

```kotlin
// network/AuthInterceptor.kt
class AuthInterceptor(
    private val tokenManager: TokenManager
) : Interceptor {
    
    override fun intercept(chain: Interceptor.Chain): Response {
        val original = chain.request()
        
        val token = tokenManager.getAccessTokenSync()
        
        val request = if (token != null) {
            original.newBuilder()
                .header("Authorization", "Bearer $token")
                .build()
        } else {
            original
        }
        
        return chain.proceed(request)
    }
}
```

### 4. Token Refresh Authenticator

```kotlin
// network/TokenAuthenticator.kt
class TokenAuthenticator(
    private val tokenManager: TokenManager,
    private val authApi: AuthApiService
) : Authenticator {
    
    override fun authenticate(route: Route?, response: Response): Request? {
        // 401 response geldiğinde token yenile
        if (response.code == 401) {
            val refreshToken = tokenManager.getRefreshTokenSync() ?: return null
            
            return try {
                val newTokenResponse = authApi.refreshToken(
                    RefreshTokenRequest(refreshToken)
                ).execute()
                
                if (newTokenResponse.isSuccessful) {
                    val newTokens = newTokenResponse.body()!!
                    tokenManager.saveTokensSync(
                        newTokens.accessToken,
                        newTokens.refreshToken
                    )
                    
                    response.request.newBuilder()
                        .header("Authorization", "Bearer ${newTokens.accessToken}")
                        .build()
                } else {
                    null
                }
            } catch (e: Exception) {
                null
            }
        }
        
        return null
    }
}
```

### 5. Retrofit Instance

```kotlin
// network/RetrofitClient.kt
@Module
@InstallIn(SingletonComponent::class)
object NetworkModule {
    
    @Provides
    @Singleton
    fun provideOkHttpClient(
        authInterceptor: AuthInterceptor,
        tokenAuthenticator: TokenAuthenticator
    ): OkHttpClient {
        return OkHttpClient.Builder()
            .connectTimeout(ApiConfig.TIMEOUT_CONNECT, TimeUnit.SECONDS)
            .readTimeout(ApiConfig.TIMEOUT_READ, TimeUnit.SECONDS)
            .writeTimeout(ApiConfig.TIMEOUT_WRITE, TimeUnit.SECONDS)
            .addInterceptor(authInterceptor)
            .authenticator(tokenAuthenticator)
            .addInterceptor(HttpLoggingInterceptor().apply {
                level = if (BuildConfig.DEBUG) {
                    HttpLoggingInterceptor.Level.BODY
                } else {
                    HttpLoggingInterceptor.Level.NONE
                }
            })
            .build()
    }
    
    @Provides
    @Singleton
    fun provideRetrofit(okHttpClient: OkHttpClient): Retrofit {
        return Retrofit.Builder()
            .baseUrl(ApiConfig.BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
    }
    
    @Provides
    @Singleton
    fun provideAuthApi(retrofit: Retrofit): AuthApiService {
        return retrofit.create(AuthApiService::class.java)
    }
    
    @Provides
    @Singleton
    fun provideProjectApi(retrofit: Retrofit): ProjectApiService {
        return retrofit.create(ProjectApiService::class.java)
    }
    
    @Provides
    @Singleton
    fun provideTaskApi(retrofit: Retrofit): TaskApiService {
        return retrofit.create(TaskApiService::class.java)
    }
}
```

---

## 📦 Data Models

### Project Models

```kotlin
// data/models/Project.kt
data class Project(
    val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String,
    val memberCount: Int = 0,
    val moduleCount: Int = 0
)

data class ProjectDetail(
    val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String
)

data class ProjectMember(
    val userId: String,
    val email: String,
    val fullName: String,
    val role: Int,
    val roleName: String,
    val addedDate: String
)
```

### Module Models

```kotlin
// data/models/Module.kt
data class Module(
    val id: String,
    val projectId: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: String,
    val useCaseCount: Int = 0
)
```

### UseCase Models

```kotlin
// data/models/UseCase.kt
data class UseCase(
    val id: String,
    val moduleId: String,
    val title: String,
    val description: String?,
    val importantNotes: String?,
    val isActive: Boolean,
    val startedDate: String,
    val taskCount: Int = 0
)
```

### Task Models

```kotlin
// data/models/Task.kt
data class Task(
    val id: String,
    val useCaseId: String,
    val creatorId: String,
    val assigneeId: String?,
    val assigneeName: String?,
    val title: String,
    val description: String?,
    val importantNotes: String?,
    val startedDate: String?,
    val dueDate: String?,
    val taskType: TaskType,
    val taskState: TaskState,
    val relations: List<TaskRelation> = emptyList()
)

enum class TaskType(val value: Int) {
    DOCUMENTATION(1),
    FEATURE(2),
    TEST(3),
    BUG(4);
    
    companion object {
        fun fromValue(value: Int) = entries.firstOrNull { it.value == value }
    }
}

enum class TaskState(val value: Int) {
    NOT_STARTED(1),
    IN_PROGRESS(2),
    COMPLETED(3),
    CANCELLED(4);
    
    companion object {
        fun fromValue(value: Int) = entries.firstOrNull { it.value == value }
    }
}

data class TaskRelation(
    val id: String,
    val targetTaskId: String,
    val relationType: TaskRelationType,
    val targetTaskTitle: String
)

enum class TaskRelationType(val value: Int) {
    BLOCKS(1),
    RELATES_TO(2),
    FIXES(3),
    DUPLICATES(4);
    
    companion object {
        fun fromValue(value: Int) = entries.firstOrNull { it.value == value }
    }
}
```

### Pagination Models

```kotlin
// data/models/PagedResult.kt
data class PagedResult<T>(
    val items: List<T>,
    val pageNumber: Int,
    val pageSize: Int,
    val totalCount: Int,
    val totalPages: Int,
    val hasPreviousPage: Boolean,
    val hasNextPage: Boolean
)
```

---

## 🔌 API Service Interfaces

### Project API Service

```kotlin
// network/api/ProjectApiService.kt
interface ProjectApiService {
    
    @GET("/api/v1/projects")
    suspend fun getProjects(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("search") search: String? = null,
        @Query("isActive") isActive: Boolean? = null
    ): Response<PagedResult<Project>>
    
    @GET("/api/v1/projects/{id}")
    suspend fun getProjectById(
        @Path("id") projectId: String
    ): Response<ProjectDetail>
    
    @GET("/api/v1/projects/{projectId}/members")
    suspend fun getProjectMembers(
        @Path("projectId") projectId: String
    ): Response<List<ProjectMember>>
}
```

### Module API Service

```kotlin
// network/api/ModuleApiService.kt
interface ModuleApiService {
    
    @GET("/api/v1/modules/projects/{projectId}")
    suspend fun getModulesByProject(
        @Path("projectId") projectId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("isActive") isActive: Boolean? = null,
        @Query("search") search: String? = null
    ): Response<PagedResult<Module>>
    
    @GET("/api/v1/modules/{id}")
    suspend fun getModuleById(
        @Path("id") moduleId: String
    ): Response<Module>
}
```

### UseCase API Service

```kotlin
// network/api/UseCaseApiService.kt
interface UseCaseApiService {
    
    @GET("/api/v1/usecases/modules/{moduleId}")
    suspend fun getUseCasesByModule(
        @Path("moduleId") moduleId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("isActive") isActive: Boolean? = null,
        @Query("search") search: String? = null
    ): Response<PagedResult<UseCase>>
    
    @GET("/api/v1/usecases/{id}")
    suspend fun getUseCaseById(
        @Path("id") useCaseId: String
    ): Response<UseCase>
}
```

### Task API Service

```kotlin
// network/api/TaskApiService.kt
interface TaskApiService {
    
    @GET("/api/v1/tasks/usecases/{useCaseId}")
    suspend fun getTasksByUseCase(
        @Path("useCaseId") useCaseId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("state") state: Int? = null,
        @Query("type") type: Int? = null,
        @Query("assigneeId") assigneeId: String? = null,
        @Query("dueDateFrom") dueDateFrom: String? = null,
        @Query("dueDateTo") dueDateTo: String? = null,
        @Query("search") search: String? = null
    ): Response<PagedResult<Task>>
    
    @GET("/api/v1/tasks/{id}")
    suspend fun getTaskById(
        @Path("id") taskId: String
    ): Response<Task>
    
    @GET("/api/v1/tasks/{taskId}/relations")
    suspend fun getTaskRelations(
        @Path("taskId") taskId: String
    ): Response<List<TaskRelation>>
    
    // ⚠️ TEK YAZMA İŞLEMİ!
    @PATCH("/api/v1/tasks/{id}/state")
    suspend fun changeTaskState(
        @Path("id") taskId: String,
        @Body request: ChangeTaskStateRequest
    ): Response<Unit>
}

data class ChangeTaskStateRequest(
    val newState: Int
)
```

---

## 🏗️ Repository Pattern

### Project Repository

```kotlin
// data/repository/ProjectRepository.kt
class ProjectRepository @Inject constructor(
    private val projectApi: ProjectApiService,
    private val projectDao: ProjectDao,
    private val networkHelper: NetworkHelper
) {
    
    suspend fun getProjects(
        page: Int = 1,
        pageSize: Int = 20,
        forceRefresh: Boolean = false
    ): Result<PagedResult<Project>> {
        return try {
            // Offline-first: Önce cache'den dene
            if (!forceRefresh && !networkHelper.isNetworkAvailable()) {
                val cachedProjects = projectDao.getAllProjects()
                if (cachedProjects.isNotEmpty()) {
                    return Result.success(
                        PagedResult(
                            items = cachedProjects.map { it.toDomain() },
                            pageNumber = 1,
                            pageSize = cachedProjects.size,
                            totalCount = cachedProjects.size,
                            totalPages = 1,
                            hasPreviousPage = false,
                            hasNextPage = false
                        )
                    )
                }
            }
            
            // Network'ten çek
            val response = projectApi.getProjects(page, pageSize)
            
            if (response.isSuccessful) {
                val data = response.body()!!
                
                // Cache'e kaydet
                if (page == 1) {
                    projectDao.deleteAll()
                    projectDao.insertAll(data.items.map { it.toEntity() })
                }
                
                Result.success(data)
            } else {
                Result.failure(Exception("Error: ${response.code()}"))
            }
        } catch (e: Exception) {
            // Hata durumunda cache'den dön
            val cachedProjects = projectDao.getAllProjects()
            if (cachedProjects.isNotEmpty()) {
                Result.success(
                    PagedResult(
                        items = cachedProjects.map { it.toDomain() },
                        pageNumber = 1,
                        pageSize = cachedProjects.size,
                        totalCount = cachedProjects.size,
                        totalPages = 1,
                        hasPreviousPage = false,
                        hasNextPage = false
                    )
                )
            } else {
                Result.failure(e)
            }
        }
    }
    
    suspend fun getProjectById(projectId: String): Result<ProjectDetail> {
        return try {
            val response = projectApi.getProjectById(projectId)
            
            if (response.isSuccessful) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Error: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
```

### Task Repository

```kotlin
// data/repository/TaskRepository.kt
class TaskRepository @Inject constructor(
    private val taskApi: TaskApiService,
    private val taskDao: TaskDao,
    private val networkHelper: NetworkHelper
) {
    
    suspend fun getTasksByUseCase(
        useCaseId: String,
        filters: TaskFilters = TaskFilters()
    ): Result<PagedResult<Task>> {
        return try {
            val response = taskApi.getTasksByUseCase(
                useCaseId = useCaseId,
                page = filters.page,
                pageSize = filters.pageSize,
                state = filters.state?.value,
                type = filters.type?.value,
                assigneeId = filters.assigneeId,
                dueDateFrom = filters.dueDateFrom,
                dueDateTo = filters.dueDateTo,
                search = filters.search
            )
            
            if (response.isSuccessful) {
                val data = response.body()!!
                
                // Cache'e kaydet
                if (filters.page == 1 && filters.isEmpty()) {
                    taskDao.insertAll(data.items.map { it.toEntity() })
                }
                
                Result.success(data)
            } else {
                Result.failure(Exception("Error: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // ⚠️ TEK YAZMA İŞLEMİ!
    suspend fun changeTaskState(
        taskId: String,
        newState: TaskState
    ): Result<Unit> {
        return try {
            val response = taskApi.changeTaskState(
                taskId,
                ChangeTaskStateRequest(newState.value)
            )
            
            if (response.isSuccessful) {
                // Local cache'i güncelle
                taskDao.updateTaskState(taskId, newState.value)
                Result.success(Unit)
            } else {
                Result.failure(Exception("Error: ${response.code()}"))
            }
        } catch (e: Exception) {
            // Offline durumunda queue'ya ekle
            if (!networkHelper.isNetworkAvailable()) {
                taskDao.addToSyncQueue(taskId, newState.value)
                Result.success(Unit) // Kullanıcıya başarılı göster
            } else {
                Result.failure(e)
            }
        }
    }
}

data class TaskFilters(
    val page: Int = 1,
    val pageSize: Int = 20,
    val state: TaskState? = null,
    val type: TaskType? = null,
    val assigneeId: String? = null,
    val dueDateFrom: String? = null,
    val dueDateTo: String? = null,
    val search: String? = null
) {
    fun isEmpty() = state == null && type == null && assigneeId == null && 
                    dueDateFrom == null && dueDateTo == null && search.isNullOrEmpty()
}
```

---

## 💾 Local Database (Room)

### Database Setup

```kotlin
// data/local/AppDatabase.kt
@Database(
    entities = [
        ProjectEntity::class,
        TaskEntity::class,
        TaskSyncQueueEntity::class
    ],
    version = 1
)
abstract class AppDatabase : RoomDatabase() {
    abstract fun projectDao(): ProjectDao
    abstract fun taskDao(): TaskDao
}
```

### Project DAO

```kotlin
// data/local/dao/ProjectDao.kt
@Dao
interface ProjectDao {
    
    @Query("SELECT * FROM projects ORDER BY startedDate DESC")
    suspend fun getAllProjects(): List<ProjectEntity>
    
    @Query("SELECT * FROM projects WHERE id = :projectId")
    suspend fun getProjectById(projectId: String): ProjectEntity?
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAll(projects: List<ProjectEntity>)
    
    @Query("DELETE FROM projects")
    suspend fun deleteAll()
}
```

### Task DAO

```kotlin
// data/local/dao/TaskDao.kt
@Dao
interface TaskDao {
    
    @Query("SELECT * FROM tasks WHERE useCaseId = :useCaseId")
    suspend fun getTasksByUseCase(useCaseId: String): List<TaskEntity>
    
    @Query("SELECT * FROM tasks WHERE assigneeId = :assigneeId")
    suspend fun getTasksByAssignee(assigneeId: String): List<TaskEntity>
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAll(tasks: List<TaskEntity>)
    
    @Query("UPDATE tasks SET taskState = :newState WHERE id = :taskId")
    suspend fun updateTaskState(taskId: String, newState: Int)
    
    // Sync queue
    @Insert
    suspend fun addToSyncQueue(item: TaskSyncQueueEntity)
    
    @Query("SELECT * FROM task_sync_queue")
    suspend fun getSyncQueue(): List<TaskSyncQueueEntity>
    
    @Delete
    suspend fun removeFromSyncQueue(item: TaskSyncQueueEntity)
}
```

### Entities

```kotlin
// data/local/entities/ProjectEntity.kt
@Entity(tableName = "projects")
data class ProjectEntity(
    @PrimaryKey val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: Long,
    val cachedAt: Long = System.currentTimeMillis()
)

// data/local/entities/TaskEntity.kt
@Entity(tableName = "tasks")
data class TaskEntity(
    @PrimaryKey val id: String,
    val useCaseId: String,
    val title: String,
    val description: String?,
    val taskType: Int,
    val taskState: Int,
    val assigneeId: String?,
    val assigneeName: String?,
    val dueDate: Long?,
    val cachedAt: Long = System.currentTimeMillis()
)

// data/local/entities/TaskSyncQueueEntity.kt
@Entity(tableName = "task_sync_queue")
data class TaskSyncQueueEntity(
    @PrimaryKey(autoGenerate = true) val id: Int = 0,
    val taskId: String,
    val newState: Int,
    val timestamp: Long = System.currentTimeMillis()
)
```

---

## 📊 Dashboard Metrics

### Dashboard Data Aggregation

```kotlin
// domain/usecase/GetDashboardDataUseCase.kt
class GetDashboardDataUseCase @Inject constructor(
    private val projectRepository: ProjectRepository,
    private val taskRepository: TaskRepository
) {
    
    suspend operator fun invoke(): Result<DashboardData> {
        return try {
            // Tüm projeleri çek
            val projectsResult = projectRepository.getProjects(pageSize = 100)
            if (projectsResult.isFailure) {
                return Result.failure(projectsResult.exceptionOrNull()!!)
            }
            
            val projects = projectsResult.getOrNull()!!.items
            
            // Kullanıcıya atanan task'ları çek
            val myTasksResult = taskRepository.getMyTasks()
            val myTasks = myTasksResult.getOrNull() ?: emptyList()
            
            // Metrikleri hesapla
            val dashboardData = DashboardData(
                totalProjects = projects.size,
                activeProjects = projects.count { it.isActive },
                myTasks = MyTasksMetrics(
                    total = myTasks.size,
                    notStarted = myTasks.count { it.taskState == TaskState.NOT_STARTED },
                    inProgress = myTasks.count { it.taskState == TaskState.IN_PROGRESS },
                    completed = myTasks.count { it.taskState == TaskState.COMPLETED }
                ),
                thisWeek = WeekMetrics(
                    completedTasks = myTasks.count { 
                        it.taskState == TaskState.COMPLETED && 
                        isThisWeek(it.startedDate)
                    },
                    overdueTasks = myTasks.count { 
                        it.dueDate != null && 
                        isPast(it.dueDate) && 
                        it.taskState != TaskState.COMPLETED
                    },
                    upcomingDeadlines = myTasks.count { 
                        it.dueDate != null && 
                        isWithinDays(it.dueDate, 7)
                    }
                ),
                recentProjects = projects.take(4)
            )
            
            Result.success(dashboardData)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}

data class DashboardData(
    val totalProjects: Int,
    val activeProjects: Int,
    val myTasks: MyTasksMetrics,
    val thisWeek: WeekMetrics,
    val recentProjects: List<Project>
)

data class MyTasksMetrics(
    val total: Int,
    val notStarted: Int,
    val inProgress: Int,
    val completed: Int
)

data class WeekMetrics(
    val completedTasks: Int,
    val overdueTasks: Int,
    val upcomingDeadlines: Int
)
```

---

## 🔄 Background Sync

### WorkManager Setup

```kotlin
// worker/TaskSyncWorker.kt
class TaskSyncWorker(
    context: Context,
    params: WorkerParameters,
    private val taskRepository: TaskRepository
) : CoroutineWorker(context, params) {
    
    override suspend fun doWork(): Result {
        return try {
            // Sync queue'dan bekleyen işlemleri al
            val pendingSync = taskRepository.getPendingSyncItems()
            
            pendingSync.forEach { item ->
                val result = taskRepository.changeTaskState(
                    item.taskId,
                    TaskState.fromValue(item.newState)!!
                )
                
                if (result.isSuccess) {
                    taskRepository.removeSyncItem(item)
                }
            }
            
            Result.success()
        } catch (e: Exception) {
            Result.retry()
        }
    }
}

// Periodic sync setup
class SyncScheduler @Inject constructor(
    private val workManager: WorkManager
) {
    
    fun scheduleSyncWork() {
        val syncRequest = PeriodicWorkRequestBuilder<TaskSyncWorker>(
            15, TimeUnit.MINUTES
        )
            .setConstraints(
                Constraints.Builder()
                    .setRequiredNetworkType(NetworkType.CONNECTED)
                    .build()
            )
            .build()
        
        workManager.enqueueUniquePeriodicWork(
            "task_sync",
            ExistingPeriodicWorkPolicy.KEEP,
            syncRequest
        )
    }
}
```

---

## 🎯 Kullanım Örnekleri

### Dashboard Screen

```kotlin
// ui/dashboard/DashboardViewModel.kt
@HiltViewModel
class DashboardViewModel @Inject constructor(
    private val getDashboardDataUseCase: GetDashboardDataUseCase
) : ViewModel() {
    
    private val _uiState = MutableStateFlow<DashboardUiState>(DashboardUiState.Loading)
    val uiState: StateFlow<DashboardUiState> = _uiState.asStateFlow()
    
    init {
        loadDashboard()
    }
    
    fun loadDashboard(forceRefresh: Boolean = false) {
        viewModelScope.launch {
            _uiState.value = DashboardUiState.Loading
            
            val result = getDashboardDataUseCase()
            
            _uiState.value = if (result.isSuccess) {
                DashboardUiState.Success(result.getOrNull()!!)
            } else {
                DashboardUiState.Error(result.exceptionOrNull()?.message ?: "Unknown error")
            }
        }
    }
}

sealed class DashboardUiState {
    object Loading : DashboardUiState()
    data class Success(val data: DashboardData) : DashboardUiState()
    data class Error(val message: String) : DashboardUiState()
}
```

### Task Detail Screen

```kotlin
// ui/task/TaskDetailViewModel.kt
@HiltViewModel
class TaskDetailViewModel @Inject constructor(
    private val taskRepository: TaskRepository,
    savedStateHandle: SavedStateHandle
) : ViewModel() {
    
    private val taskId: String = savedStateHandle["taskId"]!!
    
    private val _uiState = MutableStateFlow<TaskDetailUiState>(TaskDetailUiState.Loading)
    val uiState: StateFlow<TaskDetailUiState> = _uiState.asStateFlow()
    
    init {
        loadTask()
    }
    
    fun changeTaskState(newState: TaskState) {
        viewModelScope.launch {
            val result = taskRepository.changeTaskState(taskId, newState)
            
            if (result.isSuccess) {
                // UI'ı güncelle
                loadTask()
                // Success toast göster
            } else {
                // Error toast göster
            }
        }
    }
}
```

---

## ✅ Best Practices

### 1. Error Handling

```kotlin
sealed class ApiResult<out T> {
    data class Success<T>(val data: T) : ApiResult<T>()
    data class Error(val exception: Exception) : ApiResult<Nothing>()
    object Loading : ApiResult<Nothing>()
}

suspend fun <T> safeApiCall(
    apiCall: suspend () -> Response<T>
): ApiResult<T> {
    return try {
        val response = apiCall()
        if (response.isSuccessful) {
            ApiResult.Success(response.body()!!)
        } else {
            ApiResult.Error(Exception("Error: ${response.code()}"))
        }
    } catch (e: IOException) {
        ApiResult.Error(Exception("Network error"))
    } catch (e: Exception) {
        ApiResult.Error(e)
    }
}
```

### 2. Retry Mechanism

```kotlin
suspend fun <T> retryRequest(
    maxRetries: Int = 3,
    initialDelay: Long = 1000,
    maxDelay: Long = 10000,
    factor: Double = 2.0,
    block: suspend () -> T
): T {
    var currentDelay = initialDelay
    repeat(maxRetries - 1) {
        try {
            return block()
        } catch (e: Exception) {
            delay(currentDelay)
            currentDelay = (currentDelay * factor).toLong().coerceAtMost(maxDelay)
        }
    }
    return block()
}
```

### 3. Network Monitoring

```kotlin
class NetworkHelper @Inject constructor(
    @ApplicationContext private val context: Context
) {
    
    fun isNetworkAvailable(): Boolean {
        val connectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) 
            as ConnectivityManager
        val network = connectivityManager.activeNetwork ?: return false
        val capabilities = connectivityManager.getNetworkCapabilities(network) ?: return false
        return capabilities.hasCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET)
    }
    
    fun observeNetworkState(): Flow<Boolean> = callbackFlow {
        val connectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) 
            as ConnectivityManager
        
        val callback = object : ConnectivityManager.NetworkCallback() {
            override fun onAvailable(network: Network) {
                trySend(true)
            }
            
            override fun onLost(network: Network) {
                trySend(false)
            }
        }
        
        connectivityManager.registerDefaultNetworkCallback(callback)
        
        awaitClose {
            connectivityManager.unregisterNetworkCallback(callback)
        }
    }
}
```

---

## 📚 İlgili Dokümantasyonlar

- **MOBILE_APP_SPECIFICATION.md** - Uygulama özellikleri ve ekranlar
- **API_QUICK_REFERENCE.md** - Tüm endpoint'lerin özeti
- **MOBILE_INTEGRATION_GUIDE.md** - Auth detayları

---

**Son Güncelleme:** 2024-12-08  
**Versiyon:** 2.0  
**Durum:** Ready for Implementation 🚀
