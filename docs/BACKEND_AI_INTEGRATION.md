# Backend AI Integration Guide

Bu dokümantasyon backend'de AI service entegrasyonu için rehberdir.

## 🎯 Senin Görevin

Sen sadece backend'de AI service'i çağıracaksın. AI developer arkadaşın kendi servisini yazacak.

## 🔗 AI Service Entegrasyonu

### 1. HttpClient Ekle

`src/TE4IT.Infrastructure/AI/AIServiceClient.cs` oluştur:

```csharp
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

    // Diğer metodlar...
}
```

### 2. Response Models

`src/TE4IT.Application/Features/AI/Responses/` klasöründe:

```csharp
public record ProjectAnalysisResponse(
    string ProjectId,
    float ComplexityScore,
    int EstimatedDurationDays,
    string RiskLevel,
    List<string> Recommendations,
    DateTime AnalyzedAt
);

public record TaskEstimationResponse(
    string TaskId,
    int EstimatedHours,
    float ConfidenceScore,
    string ComplexityAssessment,
    DateTime EstimatedAt
);

public record RiskAssessmentResponse(
    string ProjectId,
    float RiskScore,
    List<string> RiskFactors,
    List<string> MitigationSuggestions,
    DateTime AssessedAt
);
```

### 3. Service Registration

`src/TE4IT.Infrastructure/DependencyInjection/ServiceRegistration.cs`:

```csharp
// AI Service HttpClient
services.AddHttpClient<IAIServiceClient, AIServiceClient>(client =>
{
    client.BaseAddress = new Uri(configuration["AIService:BaseUrl"] ?? "http://localhost:8000");
    client.DefaultRequestHeaders.Add("X-API-Key", configuration["AIService:ApiKey"] ?? "");
});
```

### 4. Configuration

`src/TE4IT.API/appsettings.json`:

```json
{
  "AIService": {
    "BaseUrl": "http://localhost:8000",
    "ApiKey": "your-api-key-here"
  }
}
```

### 5. Controller Endpoints

`src/TE4IT.API/Controllers/AIController.cs`:

```csharp
[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AIController(IMediator mediator) : ControllerBase
{
    [HttpPost("projects/{id:guid}/analyze")]
    public async Task<IActionResult> AnalyzeProject(Guid id, CancellationToken ct)
    {
        // Proje bilgilerini al
        var project = await _projectRepository.GetByIdAsync(id, ct);
        if (project is null) return NotFound();

        // AI service'i çağır
        var analysis = await _aiService.AnalyzeProjectAsync(
            id.ToString(),
            project.Description,
            ct
        );

        return Ok(analysis);
    }

    [HttpPost("tasks/{id:guid}/estimate-time")]
    public async Task<IActionResult> EstimateTaskTime(Guid id, [FromBody] EstimateTaskTimeRequest request, CancellationToken ct)
    {
        // AI service'i çağır
        var estimation = await _aiService.EstimateTaskTimeAsync(
            id.ToString(),
            request.Description,
            request.Complexity,
            ct
        );

        return Ok(estimation);
    }
}
```

## 🚀 Kullanım

### Frontend'den AI Özelliklerini Çağırma

```typescript
// Frontend'de AI özelliklerini kullanma
const analyzeProject = async (projectId: string) => {
  const response = await fetch(`/api/v1/ai/projects/${projectId}/analyze`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });
  
  return response.json();
};

const estimateTaskTime = async (taskId: string, description: string, complexity: string) => {
  const response = await fetch(`/api/v1/ai/tasks/${taskId}/estimate-time`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ description, complexity }),
  });
  
  return response.json();
};
```

## 🔧 Test Etme

### AI Service Çalışıyor mu?
```bash
# AI service'in çalıştığını kontrol et
curl http://localhost:8000/health
```

### Backend'den Test
```bash
# Backend'den AI endpoint'ini test et
curl -X POST https://localhost:5001/api/v1/ai/projects/{id}/analyze \
  -H "Authorization: Bearer {token}"
```

## 📝 Notlar

- AI service henüz geliştirilmediği için mock response döndürebilirsin
- AI developer arkadaşın servisi hazır olduğunda sadece URL'i değiştir
- Error handling eklemeyi unutma
- Logging ekle
- Rate limiting düşün

## 🆘 Sorun Giderme

### AI Service Erişilemiyor
- AI service çalışıyor mu kontrol et
- URL doğru mu?
- CORS ayarları yapıldı mı?

### Authentication Hatası
- API key doğru mu?
- Headers doğru gönderiliyor mu?

### Timeout Hatası
- AI işlemleri uzun sürebilir, timeout süresini artır
- Async/await kullan

Bu kadar! Sen sadece HTTP client ile AI service'i çağırıyorsun. AI developer arkadaşın gerisini halleder! 🚀
