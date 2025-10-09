# TE4IT AI Service

FastAPI-based AI/ML service for project management analysis and automation.

## Features

- **Project Analysis**: Analyze project descriptions and requirements
- **Task Time Estimation**: ML-based time estimation for tasks
- **Risk Assessment**: Identify potential project risks
- **Smart Recommendations**: Suggest improvements and optimizations
- **Natural Language Processing**: Process project documentation

## Quick Start

```bash
# Install dependencies
pip install -r requirements.txt

# Run development server
uvicorn main:app --reload

# Run tests
pytest

# Format code
black .
isort .
```

## API Endpoints

### Health Check
- `GET /health` - Service health status

### Project Analysis
- `POST /analyze/project` - Analyze project data
- `POST /analyze/requirements` - Analyze requirements

### Task Management
- `POST /estimate/time` - Estimate task completion time
- `POST /estimate/complexity` - Assess task complexity

### Risk Assessment
- `POST /assess/risks` - Identify project risks
- `POST /assess/dependencies` - Analyze task dependencies

## Environment Variables

```bash
# Database
DATABASE_URL=postgresql://user:pass@localhost/te4it_ai

# Redis
REDIS_URL=redis://localhost:6379

# Backend API
BACKEND_API_URL=https://localhost:5001
BACKEND_API_KEY=your_api_key

# Model Configuration
MODEL_PATH=./models/
CACHE_TTL=3600
```

## Development

### Project Structure
```
ai-service/
├── main.py              # FastAPI app
├── models/              # ML models
├── services/            # Business logic
├── api/                 # API routes
├── schemas/             # Pydantic models
├── utils/               # Utility functions
├── tests/               # Test files
└── requirements.txt     # Dependencies
```

### Adding New Features

1. Create new service in `services/`
2. Add API route in `api/`
3. Define schemas in `schemas/`
4. Write tests in `tests/`
5. Update documentation

## Integration with Backend

The AI service integrates with the .NET backend through HTTP API calls:

```python
# Example: Analyze project
async def analyze_project(project_id: str):
    # Fetch project data from backend
    project_data = await fetch_project_from_backend(project_id)
    
    # Run AI analysis
    analysis = await run_analysis(project_data)
    
    # Send results back to backend
    await send_results_to_backend(project_id, analysis)
```

## Deployment

### Docker
```bash
docker build -t te4it-ai-service .
docker run -p 8000:8000 te4it-ai-service
```

### Production
```bash
uvicorn main:app --host 0.0.0.0 --port 8000 --workers 4
```
