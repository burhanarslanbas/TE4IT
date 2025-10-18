# TE4IT - Sistem Diyagramları

## 1. Sistem Mimarisi Diyagramı

```mermaid
graph TB
    subgraph "Client Layer"
        WEB[Web Client<br/>React + TypeScript]
        MOBILE[Mobile Client<br/>Kotlin + Jetpack Compose]
    end
    
    subgraph "API Gateway"
        API[.NET 9 Web API<br/>Onion Architecture + CQRS]
    end
    
    subgraph "Services Layer"
        AUTH[Authentication Service<br/>JWT + Refresh Token]
        PROJECT[Project Management Service]
        TASK[Task Management Service]
        EDUCATION[Education Service]
        AI[AI Service<br/>FastAPI + Python]
    end
    
    subgraph "Data Layer"
        POSTGRES[(PostgreSQL<br/>Task Module)]
        MONGODB[(MongoDB<br/>Education Module)]
        REDIS[(Redis<br/>Cache)]
    end
    
    subgraph "External Services"
        FCM[Firebase Cloud Messaging<br/>Push Notifications]
    end
    
    WEB --> API
    MOBILE --> API
    API --> AUTH
    API --> PROJECT
    API --> TASK
    API --> EDUCATION
    API --> AI
    
    PROJECT --> POSTGRES
    TASK --> POSTGRES
    EDUCATION --> MONGODB
    AI --> REDIS
    
    MOBILE --> FCM
    
    style WEB fill:#e1f5fe
    style MOBILE fill:#e8f5e8
    style API fill:#fff3e0
    style POSTGRES fill:#f3e5f5
    style MONGODB fill:#e8f5e8
    style REDIS fill:#ffebee
    style AI fill:#f1f8e9
```

## 2. Veritabanı ER Diyagramı

```mermaid
erDiagram
    Users {
        Guid Id PK
        string Email
        string UserName
        string PasswordHash
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Roles {
        Guid Id PK
        string Name
        string Description
        DateTime CreatedAt
    }
    
    UserRoles {
        Guid UserId FK
        Guid RoleId FK
        DateTime AssignedAt
    }
    
    Projects {
        Guid Id PK
        string Title
        string Description
        Guid CreatorId FK
        DateTime StartedDate
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Modules {
        Guid Id PK
        Guid ProjectId FK
        string Title
        string Description
        Guid CreatorId FK
        DateTime StartedDate
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    UseCases {
        Guid Id PK
        Guid ModuleId FK
        string Title
        string Description
        string ImportantNotes
        Guid CreatorId FK
        DateTime StartedDate
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Tasks {
        Guid Id PK
        Guid UseCaseId FK
        string Title
        string Description
        string ImportantNotes
        Guid CreatorId FK
        Guid AssigneeId FK
        DateTime StartedDate
        DateTime DueDate
        string TaskType
        string TaskState
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    TaskRelations {
        Guid Id PK
        Guid SourceTaskId FK
        Guid TargetTaskId FK
        string RelationType
        DateTime CreatedAt
    }
    
    Courses {
        ObjectId _id PK
        string title
        string description
        string instructor
        number duration
        string difficulty
        array tags
        Date createdAt
        Date updatedAt
    }
    
    Lessons {
        ObjectId _id PK
        ObjectId courseId FK
        string title
        string content
        number order
        number duration
        string videoUrl
        array attachments
        Date createdAt
    }
    
    Enrollments {
        ObjectId _id PK
        string userId FK
        ObjectId courseId FK
        Date enrolledAt
        Date completedAt
        number progress
    }
    
    Progress {
        ObjectId _id PK
        string userId FK
        ObjectId lessonId FK
        number percentage
        Date completedAt
        number timeSpent
    }
    
    Users ||--o{ UserRoles : "has"
    Roles ||--o{ UserRoles : "assigned to"
    Users ||--o{ Projects : "creates"
    Users ||--o{ Modules : "creates"
    Users ||--o{ UseCases : "creates"
    Users ||--o{ Tasks : "creates"
    Users ||--o{ Tasks : "assigned to"
    
    Projects ||--o{ Modules : "contains"
    Modules ||--o{ UseCases : "contains"
    UseCases ||--o{ Tasks : "contains"
    
    Tasks ||--o{ TaskRelations : "source"
    Tasks ||--o{ TaskRelations : "target"
    
    Courses ||--o{ Lessons : "contains"
    Courses ||--o{ Enrollments : "enrolled in"
    Lessons ||--o{ Progress : "tracked in"
```

## 3. Kullanıcı Akış Diyagramı

```mermaid
flowchart TD
    START([Kullanıcı Girişi]) --> LOGIN{Kimlik Doğrulama}
    LOGIN -->|Başarılı| DASHBOARD[Dashboard]
    LOGIN -->|Başarısız| LOGIN
    
    DASHBOARD --> PROJECT_MGMT[Proje Yönetimi]
    DASHBOARD --> TASK_MGMT[Görev Yönetimi]
    DASHBOARD --> EDUCATION[Eğitim Modülü]
    DASHBOARD --> AI_INSIGHTS[AI İçgörüler]
    
    PROJECT_MGMT --> CREATE_PROJECT[Proje Oluştur]
    PROJECT_MGMT --> VIEW_PROJECTS[Projeleri Görüntüle]
    CREATE_PROJECT --> ADD_MODULE[Modül Ekle]
    ADD_MODULE --> ADD_USECASE[Kullanım Senaryosu Ekle]
    ADD_USECASE --> ADD_TASK[Görev Ekle]
    
    TASK_MGMT --> VIEW_TASKS[Görevleri Görüntüle]
    TASK_MGMT --> CREATE_TASK[Görev Oluştur]
    TASK_MGMT --> UPDATE_STATUS[Durum Güncelle]
    VIEW_TASKS --> TASK_DETAILS[Görev Detayları]
    TASK_DETAILS --> ASSIGN_TASK[Görev Ata]
    
    EDUCATION --> VIEW_COURSES[Kursları Görüntüle]
    EDUCATION --> ENROLL_COURSE[Kursa Kayıt Ol]
    EDUCATION --> TRACK_PROGRESS[İlerleme Takip Et]
    VIEW_COURSES --> COURSE_DETAILS[Kurs Detayları]
    COURSE_DETAILS --> START_LESSON[Derse Başla]
    
    AI_INSIGHTS --> PROJECT_ANALYSIS[Proje Analizi]
    AI_INSIGHTS --> TASK_PREDICTION[Görev Tahmini]
    AI_INSIGHTS --> LEARNING_RECOMMENDATIONS[Öğrenme Önerileri]
    
    MOBILE_USER[Mobil Kullanıcı] --> MOBILE_DASHBOARD[Mobil Dashboard]
    MOBILE_DASHBOARD --> MOBILE_READ[Okuma Modu]
    MOBILE_READ --> MOBILE_PROJECTS[Projeleri Görüntüle]
    MOBILE_READ --> MOBILE_TASKS[Görevleri Görüntüle]
    MOBILE_READ --> MOBILE_COURSES[Kursları Görüntüle]
    
    style START fill:#e1f5fe
    style DASHBOARD fill:#e8f5e8
    style MOBILE_USER fill:#fff3e0
    style AI_INSIGHTS fill:#f1f8e9
```

## 4. API Endpoint Diyagramı

```mermaid
graph TB
    subgraph "Authentication Endpoints"
        AUTH_REG[POST /auth/register]
        AUTH_LOGIN[POST /auth/login]
        AUTH_REFRESH[POST /auth/refresh]
        AUTH_LOGOUT[POST /auth/logout]
    end
    
    subgraph "Project Management Endpoints"
        PROJ_GET[GET /projects]
        PROJ_POST[POST /projects]
        PROJ_PUT[PUT /projects/{id}]
        PROJ_DELETE[DELETE /projects/{id}]
        MODULE_GET[GET /projects/{id}/modules]
        MODULE_POST[POST /projects/{id}/modules]
        USECASE_GET[GET /modules/{id}/usecases]
        USECASE_POST[POST /modules/{id}/usecases]
    end
    
    subgraph "Task Management Endpoints"
        TASK_GET[GET /tasks]
        TASK_POST[POST /tasks]
        TASK_PUT[PUT /tasks/{id}]
        TASK_DELETE[DELETE /tasks/{id}]
        TASK_ASSIGN[POST /tasks/{id}/assign]
        TASK_STATUS[PUT /tasks/{id}/status]
        TASK_RELATION[POST /tasks/{id}/relations]
    end
    
    subgraph "Education Endpoints"
        COURSE_GET[GET /courses]
        COURSE_POST[POST /courses]
        COURSE_DETAIL[GET /courses/{id}]
        LESSON_GET[GET /courses/{id}/lessons]
        ENROLL_POST[POST /courses/{id}/enroll]
        PROGRESS_GET[GET /progress]
        PROGRESS_POST[POST /progress]
    end
    
    subgraph "AI Service Endpoints"
        AI_PROJECT[POST /ai/project-analysis]
        AI_TASK[POST /ai/task-prediction]
        AI_RECOMMEND[GET /ai/recommendations]
    end
    
    subgraph "Response Types"
        SUCCESS[200 OK]
        CREATED[201 Created]
        BAD_REQUEST[400 Bad Request]
        UNAUTHORIZED[401 Unauthorized]
        NOT_FOUND[404 Not Found]
        SERVER_ERROR[500 Internal Server Error]
    end
    
    AUTH_REG --> CREATED
    AUTH_LOGIN --> SUCCESS
    PROJ_GET --> SUCCESS
    PROJ_POST --> CREATED
    TASK_GET --> SUCCESS
    TASK_POST --> CREATED
    COURSE_GET --> SUCCESS
    
    style AUTH_REG fill:#e1f5fe
    style PROJ_POST fill:#e8f5e8
    style TASK_POST fill:#fff3e0
    style COURSE_GET fill:#f3e5f5
    style AI_PROJECT fill:#f1f8e9
```

## 5. Sprint Timeline Diyagramı

```mermaid
gantt
    title TE4IT Geliştirme Süreci - 6 Sprint
    dateFormat  YYYY-MM-DD
    section Sprint 1
    Backend Altyapı     :done, s1-backend, 2025-01-01, 7d
    Frontend Setup      :done, s1-frontend, 2025-01-01, 7d
    Mobile Setup        :done, s1-mobile, 2025-01-01, 7d
    AI Setup           :done, s1-ai, 2025-01-01, 7d
    
    section Sprint 2
    Proje Yönetimi API  :active, s2-backend, 2025-01-08, 7d
    Proje UI           :active, s2-frontend, 2025-01-08, 7d
    Proje Mobile       :active, s2-mobile, 2025-01-08, 7d
    Proje Analizi      :active, s2-ai, 2025-01-08, 7d
    
    section Sprint 3
    Görev Yönetimi API :s3-backend, 2025-01-15, 7d
    Görev UI           :s3-frontend, 2025-01-15, 7d
    Görev Mobile       :s3-mobile, 2025-01-15, 7d
    Görev Tahmini      :s3-ai, 2025-01-15, 7d
    
    section Sprint 4
    Eğitim API         :s4-backend, 2025-01-22, 7d
    Eğitim UI          :s4-frontend, 2025-01-22, 7d
    Eğitim Mobile       :s4-mobile, 2025-01-22, 7d
    Öğrenme Analizi    :s4-ai, 2025-01-22, 7d
    
    section Sprint 5
    Frontend Polish     :s5-frontend, 2025-01-29, 7d
    Mobile Polish       :s5-mobile, 2025-01-29, 7d
    API Optimizasyon    :s5-backend, 2025-01-29, 7d
    AI Optimizasyon     :s5-ai, 2025-01-29, 7d
    
    section Sprint 6
    Final Testing       :s6-testing, 2025-02-05, 7d
    Production Deploy   :s6-deploy, 2025-02-05, 7d
    Documentation       :s6-docs, 2025-02-05, 7d
    Launch              :milestone, s6-launch, 2025-02-12, 0d
```

## 6. Teknoloji Stack Diyagramı

```mermaid
graph TB
    subgraph "Frontend Layer"
        REACT[React 18 + TypeScript]
        MUI[Material-UI]
        REDUX[Redux Toolkit]
        VITE[Vite Build Tool]
    end
    
    subgraph "Mobile Layer"
        KOTLIN[Kotlin]
        COMPOSE[Jetpack Compose]
        MVVM[MVVM Architecture]
        RETROFIT[Retrofit HTTP Client]
        ROOM[Room Database]
        FCM[Firebase Cloud Messaging]
    end
    
    subgraph "Backend Layer"
        DOTNET[.NET 9 Web API]
        ONION[Onion Architecture]
        CQRS[CQRS Pattern]
        MEDIATR[MediatR]
        FLUENT[FluentValidation]
        SERILOG[Serilog]
    end
    
    subgraph "AI Service Layer"
        FASTAPI[FastAPI Python]
        SKLEARN[scikit-learn]
        PANDAS[pandas + numpy]
        BEAUTIFULSOUP[BeautifulSoup]
    end
    
    subgraph "Database Layer"
        POSTGRES[PostgreSQL 15+]
        MONGODB[MongoDB]
        REDIS_CACHE[Redis Cache]
        EF_CORE[Entity Framework Core 9.0]
    end
    
    subgraph "Infrastructure Layer"
        DOCKER[Docker Containers]
        NGINX[Nginx Load Balancer]
        SSL[SSL/TLS Certificates]
        MONITORING[Application Monitoring]
    end
    
    REACT --> DOTNET
    KOTLIN --> DOTNET
    DOTNET --> POSTGRES
    DOTNET --> MONGODB
    DOTNET --> REDIS_CACHE
    DOTNET --> FASTAPI
    
    style REACT fill:#e1f5fe
    style KOTLIN fill:#e8f5e8
    style DOTNET fill:#fff3e0
    style FASTAPI fill:#f1f8e9
    style POSTGRES fill:#f3e5f5
    style MONGODB fill:#e8f5e8
```

## 7. Güvenlik Mimarisi Diyagramı

```mermaid
graph TB
    subgraph "Client Security"
        HTTPS[HTTPS Encryption]
        JWT_TOKEN[JWT Access Token]
        REFRESH_TOKEN[Refresh Token]
        CORS[CORS Policy]
    end
    
    subgraph "API Security"
        AUTH_MIDDLEWARE[Authentication Middleware]
        AUTHORIZATION[Authorization Policies]
        RATE_LIMIT[Rate Limiting]
        VALIDATION[Input Validation]
    end
    
    subgraph "Data Security"
        PASSWORD_HASH[Password Hashing<br/>BCrypt]
        DATA_ENCRYPTION[Data Encryption at Rest]
        SQL_INJECTION[SQL Injection Prevention]
        XSS_PROTECTION[XSS Protection]
    end
    
    subgraph "Infrastructure Security"
        FIREWALL[Firewall Rules]
        VPN[VPN Access]
        BACKUP_ENCRYPTION[Encrypted Backups]
        AUDIT_LOG[Audit Logging]
    end
    
    HTTPS --> AUTH_MIDDLEWARE
    JWT_TOKEN --> AUTHORIZATION
    REFRESH_TOKEN --> AUTH_MIDDLEWARE
    AUTH_MIDDLEWARE --> PASSWORD_HASH
    AUTHORIZATION --> DATA_ENCRYPTION
    RATE_LIMIT --> FIREWALL
    
    style HTTPS fill:#e1f5fe
    style JWT_TOKEN fill:#e8f5e8
    style AUTH_MIDDLEWARE fill:#fff3e0
    style PASSWORD_HASH fill:#f3e5f5
    style FIREWALL fill:#ffebee
```
