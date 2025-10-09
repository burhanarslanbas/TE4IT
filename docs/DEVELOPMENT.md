# TE4IT Development Guidelines

## 🎯 Project Overview
TE4IT is a comprehensive project management system with multiple client applications consuming a shared backend API.

## 🏗️ Architecture

### Backend (.NET)
- **Framework**: .NET 9 Web API
- **Architecture**: Clean Architecture + CQRS
- **Database**: PostgreSQL
- **Authentication**: JWT Bearer Tokens
- **Documentation**: Swagger/OpenAPI

### Frontend (React)
- **Framework**: React 18 + TypeScript
- **State Management**: Zustand
- **Data Fetching**: TanStack Query
- **UI**: Tailwind CSS + Lucide Icons
- **Forms**: React Hook Form + Zod

### Mobile (React Native)
- **Framework**: React Native 0.72
- **Navigation**: React Navigation 6
- **State Management**: Zustand
- **UI**: React Native Paper
- **Forms**: React Hook Form + Zod

### AI Service (Python)
- **Framework**: FastAPI
- **ML**: scikit-learn, transformers
- **Database**: PostgreSQL
- **Cache**: Redis
- **Queue**: Celery

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- Python 3.9+
- PostgreSQL 14+
- Git

### Development Setup

1. **Clone Repository**
   ```bash
   git clone <repository-url>
   cd TE4IT-Monorepo
   ```

2. **Backend Setup**
   ```bash
   cd backend/TE4IT.API
   dotnet restore
   # Update appsettings.Development.json with your DB connection
   dotnet ef database update --project ../TE4IT.Persistence
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd frontend
   npm install
   npm start
   ```

4. **Mobile Setup**
   ```bash
   cd mobile
   npm install
   npx react-native run-android
   # or
   npx react-native run-ios
   ```

5. **AI Service Setup**
   ```bash
   cd ai-service
   pip install -r requirements.txt
   uvicorn main:app --reload
   ```

## 📋 Development Workflow

### Git Branching Strategy
- `main` - Production-ready code
- `develop` - Integration branch
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `hotfix/*` - Critical fixes

### Commit Convention
```
type(scope): description

feat(auth): add JWT token refresh
fix(api): resolve pagination issue
docs(readme): update setup instructions
```

### Pull Request Process
1. Create feature branch from `develop`
2. Implement changes with tests
3. Update documentation if needed
4. Create PR to `develop`
5. Code review and approval
6. Merge to `develop`

## 🔧 Code Standards

### Backend (.NET)
- Follow Clean Architecture principles
- Use CQRS pattern for commands/queries
- Implement proper error handling
- Write unit tests for business logic
- Use dependency injection
- Follow REST API conventions

### Frontend (React)
- Use TypeScript for type safety
- Implement proper error boundaries
- Use custom hooks for logic reuse
- Follow component composition patterns
- Write unit tests with Jest/React Testing Library
- Use ESLint and Prettier

### Mobile (React Native)
- Follow React Native best practices
- Use TypeScript for type safety
- Implement proper navigation patterns
- Handle platform differences
- Write unit tests
- Use ESLint and Prettier

### AI Service (Python)
- Follow PEP 8 style guide
- Use type hints
- Implement proper error handling
- Write unit tests with pytest
- Use async/await for I/O operations
- Document API endpoints

## 🧪 Testing

### Backend Testing
```bash
cd backend
dotnet test
```

### Frontend Testing
```bash
cd frontend
npm test
```

### Mobile Testing
```bash
cd mobile
npm test
```

### AI Service Testing
```bash
cd ai-service
pytest
```

## 📦 Deployment

### Development
- Use Docker Compose for local development
- Each service runs independently
- Hot reload enabled for all services

### Production
- Use Docker containers
- Nginx reverse proxy
- SSL certificates
- Environment-specific configurations

## 🔐 Security Guidelines

### Authentication
- Use JWT tokens with short expiration
- Implement refresh token rotation
- Validate all inputs
- Use HTTPS in production

### API Security
- Rate limiting
- CORS configuration
- Input validation
- SQL injection prevention
- XSS protection

### Data Protection
- Encrypt sensitive data
- Use environment variables for secrets
- Implement proper access controls
- Regular security audits

## 📚 Documentation

### API Documentation
- Swagger/OpenAPI for backend
- Postman collections
- API versioning strategy

### Code Documentation
- XML comments for .NET
- JSDoc for JavaScript/TypeScript
- Docstrings for Python
- README files for each service

## 🐛 Debugging

### Backend Debugging
- Use Visual Studio debugger
- Enable detailed logging
- Use Swagger for API testing
- Database query analysis

### Frontend Debugging
- React Developer Tools
- Browser DevTools
- Network tab analysis
- Console logging

### Mobile Debugging
- React Native Debugger
- Flipper integration
- Device logs
- Metro bundler logs

### AI Service Debugging
- FastAPI automatic docs
- Python debugger
- Logging configuration
- Performance profiling

## 🚨 Common Issues

### Backend Issues
- Database connection problems
- JWT token validation errors
- CORS configuration issues
- Entity Framework migrations

### Frontend Issues
- CORS errors
- Authentication token issues
- State management problems
- Build configuration

### Mobile Issues
- Metro bundler problems
- Platform-specific issues
- Navigation errors
- Build configuration

### AI Service Issues
- Model loading errors
- Database connection issues
- Memory management
- API integration problems

## 📞 Support

### Team Communication
- Use GitHub Issues for bugs
- Use GitHub Discussions for questions
- Regular team meetings
- Code review sessions

### Getting Help
- Check existing documentation
- Search GitHub issues
- Ask team members
- Create detailed issue reports

## 🔄 Continuous Integration

### Automated Checks
- Code formatting (Prettier, Black)
- Linting (ESLint, Flake8)
- Type checking (TypeScript, MyPy)
- Unit tests
- Security scanning

### Deployment Pipeline
- Build and test all services
- Run integration tests
- Deploy to staging
- Manual approval for production
- Automated rollback on failure
