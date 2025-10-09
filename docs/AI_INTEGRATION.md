# TE4IT AI Service Integration Guide

## Overview
This guide explains how to integrate the AI service with the .NET backend for enhanced project management capabilities.

## Backend Integration

### 1. Add AI Service Client

Create a new service in `TE4IT.Infrastructure`:

```csharp
// TE4IT.Infrastructure/AI/AIServiceClient.cs
public interface IAIServiceClient
{
    Task<ProjectAnalysisResponse> AnalyzeProjectAsync(string projectId, string description, CancellationToken ct = default);
    Task<TaskEstimationResponse> EstimateTaskTimeAsync(string taskId, string description, string complexity, CancellationToken ct = default);
    Task<RiskAssessmentResponse> AssessProjectRisksAsync(string projectId, object projectData, CancellationToken ct = default);
}

public class AIServiceClient : IAIServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AIServiceClient> _logger;

    public AIServiceClient(HttpClient httpClient, ILogger<AIServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProjectAnalysisResponse> AnalyzeProjectAsync(string projectId, string description, CancellationToken ct = default)
    {
        var request = new ProjectAnalysisRequest
        {
            ProjectId = projectId,
            Description = description
        };

        var response = await _httpClient.PostAsJsonAsync("/analyze/project", request, ct);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<ProjectAnalysisResponse>(cancellationToken: ct);
    }

    // ... other methods
}
```

### 2. Register Services

In `TE4IT.Infrastructure/DependencyInjection/ServiceRegistration.cs`:

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // ... existing services

    // AI Service
    services.AddHttpClient<IAIServiceClient, AIServiceClient>(client =>
    {
        client.BaseAddress = new Uri(configuration["AIService:BaseUrl"] ?? "http://localhost:8000");
        client.DefaultRequestHeaders.Add("X-API-Key", configuration["AIService:ApiKey"] ?? "");
    });

    return services;
}
```

### 3. Add Configuration

In `appsettings.json`:

```json
{
  "AIService": {
    "BaseUrl": "http://localhost:8000",
    "ApiKey": "your-api-key-here"
  }
}
```

### 4. Create AI-Enhanced Commands

```csharp
// TE4IT.Application/Features/Projects/Commands/AnalyzeProject/AnalyzeProjectCommand.cs
public sealed record AnalyzeProjectCommand(Guid ProjectId) : IRequest<AnalyzeProjectCommandResponse>;

public sealed record AnalyzeProjectCommandResponse(
    float ComplexityScore,
    int EstimatedDurationDays,
    string RiskLevel,
    List<string> Recommendations
);

// Handler
public sealed class AnalyzeProjectCommandHandler : IRequestHandler<AnalyzeProjectCommand, AnalyzeProjectCommandResponse>
{
    private readonly IAIServiceClient _aiService;
    private readonly IProjectReadRepository _projectRepository;

    public async Task<AnalyzeProjectCommandResponse> Handle(AnalyzeProjectCommand request, CancellationToken ct)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, ct);
        if (project is null) throw new ResourceNotFoundException("Project not found");

        var analysis = await _aiService.AnalyzeProjectAsync(
            request.ProjectId.ToString(),
            project.Description,
            ct
        );

        return new AnalyzeProjectCommandResponse(
            analysis.ComplexityScore,
            analysis.EstimatedDurationDays,
            analysis.RiskLevel,
            analysis.Recommendations
        );
    }
}
```

### 5. Add AI Endpoints

```csharp
// TE4IT.API/Controllers/AIController.cs
[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AIController(IMediator mediator) : ControllerBase
{
    [HttpPost("projects/{id:guid}/analyze")]
    public async Task<IActionResult> AnalyzeProject(Guid id, CancellationToken ct)
    {
        var command = new AnalyzeProjectCommand(id);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("tasks/{id:guid}/estimate-time")]
    public async Task<IActionResult> EstimateTaskTime(Guid id, [FromBody] EstimateTaskTimeRequest request, CancellationToken ct)
    {
        var command = new EstimateTaskTimeCommand(id, request.Description, request.Complexity);
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }
}
```

## Frontend Integration

### 1. Add AI Service

```typescript
// frontend/src/services/aiService.ts
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:5001';

export interface ProjectAnalysis {
  complexityScore: number;
  estimatedDurationDays: number;
  riskLevel: string;
  recommendations: string[];
}

export interface TaskEstimation {
  estimatedHours: number;
  confidenceScore: number;
  complexityAssessment: string;
}

export const aiService = {
  async analyzeProject(projectId: string): Promise<ProjectAnalysis> {
    const response = await axios.post(`${API_BASE_URL}/api/v1/ai/projects/${projectId}/analyze`);
    return response.data;
  },

  async estimateTaskTime(taskId: string, description: string, complexity: string): Promise<TaskEstimation> {
    const response = await axios.post(`${API_BASE_URL}/api/v1/ai/tasks/${taskId}/estimate-time`, {
      description,
      complexity
    });
    return response.data;
  }
};
```

### 2. Add AI Components

```typescript
// frontend/src/components/ProjectAnalysis.tsx
import React, { useState } from 'react';
import { aiService } from '../services/aiService';

interface ProjectAnalysisProps {
  projectId: string;
}

export const ProjectAnalysis: React.FC<ProjectAnalysisProps> = ({ projectId }) => {
  const [analysis, setAnalysis] = useState<ProjectAnalysis | null>(null);
  const [loading, setLoading] = useState(false);

  const handleAnalyze = async () => {
    setLoading(true);
    try {
      const result = await aiService.analyzeProject(projectId);
      setAnalysis(result);
    } catch (error) {
      console.error('Analysis failed:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 border rounded-lg">
      <h3 className="text-lg font-semibold mb-4">AI Project Analysis</h3>
      
      <button
        onClick={handleAnalyze}
        disabled={loading}
        className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 disabled:opacity-50"
      >
        {loading ? 'Analyzing...' : 'Analyze Project'}
      </button>

      {analysis && (
        <div className="mt-4 space-y-2">
          <p><strong>Complexity Score:</strong> {(analysis.complexityScore * 100).toFixed(1)}%</p>
          <p><strong>Estimated Duration:</strong> {analysis.estimatedDurationDays} days</p>
          <p><strong>Risk Level:</strong> {analysis.riskLevel}</p>
          <div>
            <strong>Recommendations:</strong>
            <ul className="list-disc list-inside mt-1">
              {analysis.recommendations.map((rec, index) => (
                <li key={index}>{rec}</li>
              ))}
            </ul>
          </div>
        </div>
      )}
    </div>
  );
};
```

## Mobile Integration

### 1. Add AI Service

```typescript
// mobile/src/services/aiService.ts
import AsyncStorage from '@react-native-async-storage/async-storage';

const API_BASE_URL = 'https://localhost:5001';

export const aiService = {
  async analyzeProject(projectId: string, token: string) {
    const response = await fetch(`${API_BASE_URL}/api/v1/ai/projects/${projectId}/analyze`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });
    
    if (!response.ok) {
      throw new Error('Analysis failed');
    }
    
    return response.json();
  },

  async estimateTaskTime(taskId: string, description: string, complexity: string, token: string) {
    const response = await fetch(`${API_BASE_URL}/api/v1/ai/tasks/${taskId}/estimate-time`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ description, complexity }),
    });
    
    if (!response.ok) {
      throw new Error('Estimation failed');
    }
    
    return response.json();
  }
};
```

## Testing

### 1. Backend Tests

```csharp
[Test]
public async Task AnalyzeProject_ShouldReturnAnalysis()
{
    // Arrange
    var projectId = Guid.NewGuid();
    var command = new AnalyzeProjectCommand(projectId);
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.That(result.ComplexityScore, Is.InRange(0, 1));
    Assert.That(result.EstimatedDurationDays, Is.GreaterThan(0));
    Assert.That(result.RiskLevel, Is.OneOf("low", "medium", "high"));
    Assert.That(result.Recommendations, Is.Not.Empty);
}
```

### 2. Frontend Tests

```typescript
// frontend/src/services/__tests__/aiService.test.ts
import { aiService } from '../aiService';
import axios from 'axios';

jest.mock('axios');
const mockedAxios = axios as jest.Mocked<typeof axios>;

describe('aiService', () => {
  it('should analyze project successfully', async () => {
    const mockResponse = {
      complexityScore: 0.7,
      estimatedDurationDays: 30,
      riskLevel: 'medium',
      recommendations: ['Break down tasks', 'Add buffer time']
    };

    mockedAxios.post.mockResolvedValue({ data: mockResponse });

    const result = await aiService.analyzeProject('project-1');

    expect(result).toEqual(mockResponse);
    expect(mockedAxios.post).toHaveBeenCalledWith(
      'https://localhost:5001/api/v1/ai/projects/project-1/analyze'
    );
  });
});
```

## Deployment

### 1. Docker Configuration

```dockerfile
# ai-service/Dockerfile
FROM python:3.9-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install -r requirements.txt

COPY . .

EXPOSE 8000

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
```

### 2. Docker Compose

```yaml
# docker-compose.yml
services:
  ai-service:
    build: ./ai-service
    ports:
      - "8000:8000"
    environment:
      - BACKEND_API_URL=http://backend:5001
      - BACKEND_API_KEY=${AI_SERVICE_API_KEY}
    depends_on:
      - backend
```

## Monitoring and Logging

### 1. Health Checks

```python
# ai-service/main.py
@app.get("/health")
async def health_check():
    return {
        "status": "healthy",
        "timestamp": datetime.now(),
        "models": {
            "project_analysis": "ready",
            "task_estimation": "ready",
            "risk_assessment": "ready"
        }
    }
```

### 2. Logging

```python
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

@app.post("/analyze/project")
async def analyze_project(request: ProjectAnalysisRequest):
    logger.info(f"Analyzing project {request.project_id}")
    # ... analysis logic
    logger.info(f"Analysis completed for project {request.project_id}")
```

## Security Considerations

1. **API Authentication**: Use JWT tokens for AI service authentication
2. **Rate Limiting**: Implement rate limiting to prevent abuse
3. **Input Validation**: Validate all inputs to prevent injection attacks
4. **Data Privacy**: Ensure sensitive project data is handled securely
5. **Model Security**: Protect ML models from unauthorized access

## Performance Optimization

1. **Caching**: Cache analysis results for similar projects
2. **Async Processing**: Use background tasks for heavy analysis
3. **Model Optimization**: Optimize ML models for faster inference
4. **Resource Management**: Monitor memory and CPU usage
5. **Load Balancing**: Use multiple AI service instances for scalability
