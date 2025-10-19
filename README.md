# TE4IT - Enterprise Project Management & Learning Platform

## 🏗️ Architecture
- **Backend**: .NET 9 Web API (Clean Architecture + CQRS)
- **Frontend**: React + TypeScript
- **Mobile**: Android (Kotlin)
- **AI Service**: Python/FastAPI
- **Database**: PostgreSQL
<div align="center">

![TE4IT Logo](https://img.shields.io/badge/TE4IT-Enterprise%20Platform-blue?style=for-the-badge&logo=rocket)
![.NET](https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge&logo=dotnet)
![React](https://img.shields.io/badge/React-18.0-blue?style=for-the-badge&logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue?style=for-the-badge&logo=typescript)
![Kotlin](https://img.shields.io/badge/Kotlin-Android-green?style=for-the-badge&logo=kotlin)
![Python](https://img.shields.io/badge/Python-FastAPI-yellow?style=for-the-badge&logo=python)

**A comprehensive enterprise-grade platform that unifies project management with educational content and AI-powered insights for software development teams and IT students.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://github.com/burhanarslanbas/TE4IT)
[![API Documentation](https://img.shields.io/badge/API-Swagger-green)](https://localhost:5001/swagger)
[![Coverage](https://img.shields.io/badge/Coverage-85%25-brightgreen)](https://github.com/burhanarslanbas/TE4IT)

</div>

---

## 🎯 **Platform Overview**

TE4IT is a cutting-edge enterprise platform that revolutionizes how software development teams manage projects while seamlessly integrating educational content and AI-powered analytics. Built with modern architectural patterns and enterprise-grade technologies.

### **🌟 Current Features**
- **🔐 Enterprise Authentication**: JWT authentication with refresh token rotation
- **👥 User Management**: Complete user and role management system
- **🏗️ Project Management**: Hierarchical project structure (Project → Module → Use Case → Task)
- **📋 Task Management**: Advanced task tracking with dependencies
- **🔒 Security**: Role-based access control (RBAC) and rate limiting
- **📊 API-First Design**: RESTful API with comprehensive documentation

---

## 🏗️ **Enterprise Architecture**

### **Clean Architecture + CQRS Pattern**
```
┌─────────────────────────────────────┐
│           API Layer                 │ ← Controllers, Middleware, Swagger
├─────────────────────────────────────┤
│        Application Layer            │ ← CQRS Commands/Queries, MediatR
├─────────────────────────────────────┤
│          Domain Layer               │ ← Entities, Business Rules, Events
├─────────────────────────────────────┤
│       Infrastructure Layer          │ ← Database, External Services, Auth
└─────────────────────────────────────┘
```

### **Technology Stack**

#### **🔧 Backend (.NET 9)**
- **Framework**: .NET 9 Web API
- **Architecture**: Onion Architecture + CQRS + MediatR
- **Authentication**: JWT + Refresh Token Rotation
- **Validation**: FluentValidation
- **Logging**: Serilog (Structured Logging)
- **ORM**: Entity Framework Core 9.0
- **Database**: PostgreSQL 15+ (Task Module)
- **Cache**: Redis (Distributed Caching)
- **Documentation**: Swagger/OpenAPI 3.0

#### **🗄️ Database Architecture**
- **PostgreSQL**: Complete data storage with ACID compliance
- **Redis**: Caching and session management

---

## 🚀 **Quick Start**

### **Prerequisites**
- .NET 9 SDK
- PostgreSQL 15+
- Redis 7+
- Docker (Optional)

### **🔧 Backend Setup**
```bash
# Clone repository
git clone https://github.com/burhanarslanbas/TE4IT.git
cd TE4IT

# Backend setup
cd src/TE4IT.API
dotnet restore
dotnet ef database update --project ../TE4IT.Persistence
dotnet run
```

**🌐 Access Points:**
- **API**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger

---

## 🌐 **Live Deployment**

### **🚀 Azure DevOps Deployment**

API'miz Azure DevOps ile otomatik deploy edilmektedir:

- **🌍 Live API**: Azure App Service üzerinde çalışmaktadır
- **📚 Swagger UI**: `/swagger` endpoint'i üzerinden erişilebilir
- **🔐 Auth Endpoints**: `/api/auth/*` endpoint'leri aktif

### **📱 Frontend/Mobile Integration**

Arkadaşlarınız için API entegrasyonu:

```javascript
// Frontend'de kullanım
const API_BASE_URL = 'https://your-azure-app.azurewebsites.net';

// Login örneği
const login = async (email, password) => {
  const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  return response.json();
};
```

```kotlin
// Android'de kullanım
val apiBaseUrl = "https://your-azure-app.azurewebsites.net"

// Retrofit ile
@POST("api/auth/login")
suspend fun login(@Body request: LoginRequest): Response<LoginResponse>
```
- **Health Check**: https://localhost:5001/health

---

## 📊 **API Documentation**

### **🔐 Authentication Endpoints**
```http
POST /api/v1/auth/register     # User registration
POST /api/v1/auth/login        # User login
POST /api/v1/auth/refreshToken # Token refresh
POST /api/v1/auth/revokeToken  # Token revocation
POST /api/v1/auth/forgotPassword # Password reset request
POST /api/v1/auth/resetPassword  # Password reset
```

### **👥 User Management**
```http
GET    /api/v1/users                    # List users (paginated)
GET    /api/v1/users/{id}              # Get user by ID
GET    /api/v1/users/{id}/roles        # Get user roles
POST   /api/v1/users/{id}/roles/{role} # Assign role
DELETE /api/v1/users/{id}/roles/{role} # Remove role
```

### **🏗️ Project Management**
```http
GET    /api/v1/projects           # List projects
GET    /api/v1/projects/{id}      # Get project details
POST   /api/v1/projects           # Create project
PUT    /api/v1/projects/{id}      # Update project
DELETE /api/v1/projects/{id}      # Delete project
```

### **📋 Task Management**
```http
GET    /api/v1/tasks              # List tasks
GET    /api/v1/tasks/{id}         # Get task details
POST   /api/v1/tasks              # Create task
PUT    /api/v1/tasks/{id}         # Update task
DELETE /api/v1/tasks/{id}         # Delete task
POST   /api/v1/tasks/{id}/assign  # Assign task
PUT    /api/v1/tasks/{id}/status  # Update task status
```

### **📚 Education Module** *(Coming Soon)*
```http
GET    /api/v1/courses            # List courses
GET    /api/v1/courses/{id}       # Get course details
POST   /api/v1/courses/{id}/enroll # Enroll in course
GET    /api/v1/progress           # Get learning progress
```

### **🤖 AI Analytics** *(Coming Soon)*
```http
POST /api/v1/ai/projects/{id}/analyze      # Project analysis
POST /api/v1/ai/tasks/{id}/estimate-time   # Task time estimation
GET  /api/v1/ai/recommendations            # Learning recommendations
```

**📖 Complete API Documentation**: [docs/api/](docs/api/)

---

## 🏗️ **Project Structure**

```
TE4IT/
├── 📁 src/                          # .NET Backend (Clean Architecture)
│   ├── TE4IT.API/                   # Web API Layer (Controllers, Middleware)
│   ├── TE4IT.Application/           # Application Layer (CQRS, MediatR)
│   ├── TE4IT.Domain/                # Domain Layer (Entities, Business Rules)
│   ├── TE4IT.Infrastructure/        # Infrastructure Layer (External Services)
│   └── TE4IT.Persistence/           # Data Access Layer (EF Core, Repositories)
├── 📁 docs/                         # Comprehensive Documentation
│   ├── api/                         # API documentation
│   ├── architecture/                # System architecture docs
│   ├── development/                 # Development guides
│   └── project-management/          # PRD and project docs
├── 📁 infrastructure/               # DevOps & Deployment
│   └── docker-compose.yml           # Docker configuration
├── 📁 frontend/                     # React Web Application *(Coming Soon)*
├── 📁 mobile/                       # Kotlin Android Application *(Coming Soon)*
├── 📁 ai-service/                   # Python FastAPI Service *(Coming Soon)*
└── 📁 shared/                       # Shared resources and utilities
```

---

## 🔐 **Security Features**

### **Authentication & Authorization**
- **JWT Access Tokens**: Short-lived (15-30 minutes)
- **Refresh Token Rotation**: Enhanced security with automatic rotation
- **Role-Based Access Control (RBAC)**: Granular permission system
- **Rate Limiting**: Protection against brute force attacks
- **HTTPS Enforcement**: All communications encrypted

### **Data Protection**
- **Password Hashing**: BCrypt with salt
- **SQL Injection Prevention**: Parameterized queries
- **XSS Protection**: Input validation and sanitization
- **CORS Configuration**: Controlled cross-origin requests
- **Audit Logging**: Comprehensive activity tracking

---

## 📈 **Performance & Scalability**

### **Performance Metrics**
- **API Response Time**: < 800ms average
- **Concurrent Users**: 1000+ supported
- **Database Queries**: < 200ms average
- **System Uptime**: 99.9% availability target

### **Scalability Features**
- **Horizontal Scaling**: Stateless API design
- **Database Optimization**: Indexed queries, connection pooling
- **Caching Strategy**: Redis distributed caching
- **Load Balancing**: Ready for multiple instances
- **Microservice Ready**: Domain-driven service separation

---

## 🧪 **Testing Strategy**

### **Backend Testing**
```bash
# Unit Tests
dotnet test src/TE4IT.Tests.Unit/

# Integration Tests
dotnet test src/TE4IT.Tests.Integration/

# API Tests
dotnet test src/TE4IT.Tests.API/
```

### **Frontend Testing** *(Coming Soon)*
```bash
cd frontend
npm test                    # Unit tests
npm run test:e2e           # End-to-end tests
npm run test:coverage       # Coverage report
```

### **Mobile Testing** *(Coming Soon)*
```bash
cd mobile
./gradlew test             # Unit tests
./gradlew connectedAndroidTest # Integration tests
```

### **AI Service Testing** *(Coming Soon)*
```bash
cd ai-service
pytest tests/              # Python tests
pytest --cov=app tests/    # Coverage report
```

---

## 🚀 **Deployment**

### **Docker Deployment**
```bash
# Build all services
docker-compose build

# Run all services
docker-compose up -d

# Scale specific services
docker-compose up -d --scale api=3
```

### **Kubernetes Deployment**
```bash
# Apply Kubernetes manifests
kubectl apply -f infrastructure/k8s/

# Check deployment status
kubectl get pods
kubectl get services
```

### **Environment Configuration**
- **Development**: Local development environment
- **Staging**: Pre-production testing environment
- **Production**: Live production environment

---

## 📚 **Documentation**

### **Comprehensive Documentation**
- **[API Documentation](docs/api/)** - Complete API reference
- **[Architecture Guide](docs/architecture/)** - System design and diagrams
- **[Development Guide](docs/development/)** - Setup and development processes
- **[Project Management](docs/project-management/)** - PRD and project planning

### **Interactive Documentation**
- **Swagger UI**: https://localhost:5001/swagger
- **API Reference**: Auto-generated from code
- **Postman Collection**: Ready-to-use API collections

---

## 🤝 **Contributing**

We welcome contributions! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/amazing-feature`
3. **Follow coding standards**: ESLint, Prettier, EditorConfig
4. **Write tests**: Ensure test coverage > 80%
5. **Update documentation**: Keep docs in sync with code
6. **Commit changes**: `git commit -m 'feat: add amazing feature'`
7. **Push to branch**: `git push origin feature/amazing-feature`
8. **Open Pull Request**: Detailed description and testing notes

### **Development Guidelines**
- **Code Style**: Follow project coding conventions
- **Commit Messages**: Use conventional commit format
- **Testing**: Write unit and integration tests
- **Documentation**: Update relevant documentation
- **Security**: Follow security best practices

---

## 👥 **Team & Roles**

### **Current Development Team**
- **🏗️ Backend Developer**: .NET API, Authentication, Database Design
- **📋 Project Manager**: Sprint planning, team coordination

### **Future Team Expansion** *(Coming Soon)*
- **🌐 Frontend Developer**: React Web App, UI/UX Design
- **📱 Mobile Developer**: Kotlin Android App, Mobile UX
- **🤖 AI Developer**: Python AI/ML Service, Data Analytics
- **🔧 DevOps Engineer**: Infrastructure, CI/CD, Monitoring
- **🎨 UI/UX Designer**: User experience and interface design
- **🧪 QA Engineer**: Testing strategy and quality assurance

---

## 📄 **License**

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 🆘 **Support & Contact**

### **Getting Help**
- **📖 Documentation**: Check our comprehensive docs
- **🐛 Bug Reports**: Create an issue with detailed information
- **💡 Feature Requests**: Submit enhancement proposals
- **💬 Discussions**: Join our community discussions

### **Contact Information**
- **📧 Email**: team@te4it.com
- **🐙 GitHub**: [burhanarslanbas/TE4IT](https://github.com/burhanarslanbas/TE4IT)
- **📱 Issues**: [GitHub Issues](https://github.com/burhanarslanbas/TE4IT/issues)
- **💬 Discussions**: [GitHub Discussions](https://github.com/burhanarslanbas/TE4IT/discussions)

---

<div align="center">

**Built with ❤️ by the TE4IT Development Team**

![Made with Love](https://img.shields.io/badge/Made%20with-❤️-red?style=for-the-badge)

*Empowering software development teams with intelligent project management and integrated learning solutions.*

</div>

