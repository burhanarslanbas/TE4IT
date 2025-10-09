# TE4IT - Project Management System

## 🏗️ Architecture
- **Backend**: .NET 9 Web API (Clean Architecture + CQRS)
- **Frontend**: React + TypeScript
- **Mobile**: React Native
- **AI Service**: Python/FastAPI
- **Database**: PostgreSQL

## 🚀 Quick Start

### Backend (.NET API)
```bash
cd src/TE4IT.API
dotnet restore
dotnet run
```
- **URL**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### Frontend (React) - *Geliştirilecek*
```bash
cd frontend
# Frontend developer tarafından React ile oluşturulacak
# Detaylar için: frontend/README.md
```

### Mobile (Android) - *Geliştirilecek*
```bash
cd mobile
# Mobile developer tarafından Android (Kotlin/Java) ile oluşturulacak
# Detaylar için: mobile/README.md
```

### AI Service (Python/FastAPI) - *Geliştirilecek*
```bash
cd ai-service
# AI developer tarafından Python/FastAPI ile oluşturulacak
# Detaylar için: ai-service/README.md
```

## 📁 Project Structure

```
TE4IT/
├── 📁 src/                  # .NET Web API
│   ├── TE4IT.API/           # Web API Layer
│   ├── TE4IT.Application/   # Application Layer (CQRS)
│   ├── TE4IT.Domain/        # Domain Layer
│   ├── TE4IT.Infrastructure/# Infrastructure Layer
│   └── TE4IT.Persistence/   # Data Access Layer
├── 📁 frontend/             # React Web App (geliştirilecek)
├── 📁 mobile/               # Android App (geliştirilecek)
├── 📁 ai-service/           # Python AI/ML Service (geliştirilecek)
├── 📁 docs/                 # Documentation
├── 📁 shared/               # Shared resources
├── 📁 infrastructure/        # DevOps & Deployment
└── 📁 tools/                # Developer tools
```

## 🔐 Authentication

### Register
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

### Login
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

### Using Token
```http
GET /api/v1/users
Authorization: Bearer <access_token>
```

## 📊 API Endpoints

### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh token
- `POST /api/v1/auth/revoke` - Revoke token

### Users
- `GET /api/v1/users` - List users (paginated)
- `GET /api/v1/users/{id}` - Get user by ID
- `GET /api/v1/users/{id}/roles` - Get user roles
- `POST /api/v1/users/{id}/roles/{roleName}` - Assign role
- `DELETE /api/v1/users/{id}/roles/{roleName}` - Remove role

### Roles
- `GET /api/v1/roles` - List all roles
- `GET /api/v1/roles/{id}` - Get role by ID
- `POST /api/v1/roles` - Create role
- `PUT /api/v1/roles/{id}` - Update role
- `DELETE /api/v1/roles/{id}` - Delete role

### Projects
- `GET /api/v1/projects` - List projects (paginated)
- `GET /api/v1/projects/{id}` - Get project by ID
- `POST /api/v1/projects` - Create project

## 🤖 AI Service Integration

### Project Analysis
```http
POST /api/v1/ai/projects/{id}/analyze
Authorization: Bearer <access_token>
```

### Task Time Estimation
```http
POST /api/v1/ai/tasks/{id}/estimate-time
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "complexity": "medium",
  "description": "Implement user authentication"
}
```

## 🛠️ Development

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- Python 3.9+
- PostgreSQL 14+
- Docker (optional)

### Environment Setup
1. Copy `src/TE4IT.API/appsettings.Development.json.example` to `appsettings.Development.json`
2. Update database connection string
3. Run database migrations: `dotnet ef database update --project src/TE4IT.Persistence`

### Testing
```bash
# Backend tests
cd src
dotnet test

# Frontend tests (geliştirilecek)
cd frontend
# Frontend developer tarafından eklenir

# Mobile tests (geliştirilecek)
cd mobile
# Mobile developer tarafından eklenir

# AI service tests (geliştirilecek)
cd ai-service
# AI developer tarafından eklenir
```

## 📝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Team

- **Backend Developer**: .NET API, Authentication, Database
- **Frontend Developer**: React Web App, UI/UX
- **Mobile Developer**: React Native App
- **AI Developer**: Python AI/ML Service

## 🆘 Support

For support, email support@te4it.com or create an issue in this repository.
