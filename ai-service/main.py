from fastapi import FastAPI, HTTPException, Depends
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import httpx
import os
from datetime import datetime

app = FastAPI(
    title="TE4IT AI Service",
    description="AI/ML service for project management analysis",
    version="1.0.0"
)

# CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["https://localhost:5001", "http://localhost:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Configuration
BACKEND_API_URL = os.getenv("BACKEND_API_URL", "https://localhost:5001")
BACKEND_API_KEY = os.getenv("BACKEND_API_KEY", "")

# Pydantic models
class ProjectAnalysisRequest(BaseModel):
    project_id: str
    description: str
    requirements: Optional[str] = None

class ProjectAnalysisResponse(BaseModel):
    project_id: str
    complexity_score: float
    estimated_duration_days: int
    risk_level: str
    recommendations: List[str]
    analyzed_at: datetime

class TaskEstimationRequest(BaseModel):
    task_id: str
    description: str
    complexity: str  # low, medium, high
    dependencies: Optional[List[str]] = None

class TaskEstimationResponse(BaseModel):
    task_id: str
    estimated_hours: int
    confidence_score: float
    complexity_assessment: str
    estimated_at: datetime

class RiskAssessmentRequest(BaseModel):
    project_id: str
    project_data: dict

class RiskAssessmentResponse(BaseModel):
    project_id: str
    risk_score: float
    risk_factors: List[str]
    mitigation_suggestions: List[str]
    assessed_at: datetime

# Dependency to get HTTP client
async def get_http_client():
    async with httpx.AsyncClient() as client:
        yield client

@app.get("/health")
async def health_check():
    """Health check endpoint"""
    return {"status": "healthy", "timestamp": datetime.now()}

@app.post("/analyze/project", response_model=ProjectAnalysisResponse)
async def analyze_project(
    request: ProjectAnalysisRequest,
    client: httpx.AsyncClient = Depends(get_http_client)
):
    """Analyze project complexity and provide recommendations"""
    
    # Mock analysis logic (replace with actual ML model)
    complexity_score = min(1.0, len(request.description) / 1000)
    estimated_duration = max(7, int(complexity_score * 90))
    
    risk_level = "low"
    if complexity_score > 0.7:
        risk_level = "high"
    elif complexity_score > 0.4:
        risk_level = "medium"
    
    recommendations = [
        "Break down large tasks into smaller, manageable pieces",
        "Consider adding buffer time for complex features",
        "Regular progress reviews recommended"
    ]
    
    if complexity_score > 0.6:
        recommendations.append("Consider additional team members")
    
    return ProjectAnalysisResponse(
        project_id=request.project_id,
        complexity_score=complexity_score,
        estimated_duration_days=estimated_duration,
        risk_level=risk_level,
        recommendations=recommendations,
        analyzed_at=datetime.now()
    )

@app.post("/estimate/time", response_model=TaskEstimationResponse)
async def estimate_task_time(
    request: TaskEstimationRequest,
    client: httpx.AsyncClient = Depends(get_http_client)
):
    """Estimate task completion time based on complexity"""
    
    # Mock estimation logic (replace with actual ML model)
    base_hours = {"low": 2, "medium": 8, "high": 24}
    estimated_hours = base_hours.get(request.complexity, 8)
    
    # Adjust based on description length
    description_factor = min(2.0, len(request.description) / 200)
    estimated_hours = int(estimated_hours * description_factor)
    
    # Adjust for dependencies
    if request.dependencies:
        estimated_hours += len(request.dependencies) * 2
    
    confidence_score = 0.8 if request.complexity == "medium" else 0.7
    
    return TaskEstimationResponse(
        task_id=request.task_id,
        estimated_hours=estimated_hours,
        confidence_score=confidence_score,
        complexity_assessment=request.complexity,
        estimated_at=datetime.now()
    )

@app.post("/assess/risks", response_model=RiskAssessmentResponse)
async def assess_project_risks(
    request: RiskAssessmentRequest,
    client: httpx.AsyncClient = Depends(get_http_client)
):
    """Assess project risks and provide mitigation suggestions"""
    
    # Mock risk assessment (replace with actual ML model)
    risk_score = 0.3  # Base risk
    
    risk_factors = []
    mitigation_suggestions = []
    
    # Analyze project data for risk factors
    if "deadline" in request.project_data:
        risk_score += 0.2
        risk_factors.append("Tight deadline")
        mitigation_suggestions.append("Consider extending timeline or reducing scope")
    
    if "team_size" in request.project_data and request.project_data["team_size"] < 3:
        risk_score += 0.1
        risk_factors.append("Small team size")
        mitigation_suggestions.append("Consider adding more team members")
    
    if "complexity" in request.project_data and request.project_data["complexity"] == "high":
        risk_score += 0.3
        risk_factors.append("High complexity")
        mitigation_suggestions.append("Break down into smaller, manageable tasks")
    
    risk_score = min(1.0, risk_score)
    
    return RiskAssessmentResponse(
        project_id=request.project_id,
        risk_score=risk_score,
        risk_factors=risk_factors,
        mitigation_suggestions=mitigation_suggestions,
        assessed_at=datetime.now()
    )

@app.get("/models/status")
async def get_models_status():
    """Get status of ML models"""
    return {
        "project_analysis_model": "ready",
        "task_estimation_model": "ready", 
        "risk_assessment_model": "ready",
        "last_updated": datetime.now()
    }

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
