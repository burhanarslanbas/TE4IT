import pytest
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_health_check():
    """Test health check endpoint"""
    response = client.get("/health")
    assert response.status_code == 200
    data = response.json()
    assert data["status"] == "healthy"
    assert "timestamp" in data

def test_analyze_project():
    """Test project analysis endpoint"""
    request_data = {
        "project_id": "test-project-1",
        "description": "A comprehensive project management system with AI integration",
        "requirements": "User authentication, project tracking, task management"
    }
    
    response = client.post("/analyze/project", json=request_data)
    assert response.status_code == 200
    
    data = response.json()
    assert data["project_id"] == "test-project-1"
    assert 0 <= data["complexity_score"] <= 1
    assert data["estimated_duration_days"] > 0
    assert data["risk_level"] in ["low", "medium", "high"]
    assert isinstance(data["recommendations"], list)
    assert "analyzed_at" in data

def test_estimate_task_time():
    """Test task time estimation endpoint"""
    request_data = {
        "task_id": "test-task-1",
        "description": "Implement user authentication system",
        "complexity": "medium",
        "dependencies": ["setup-database", "create-user-model"]
    }
    
    response = client.post("/estimate/time", json=request_data)
    assert response.status_code == 200
    
    data = response.json()
    assert data["task_id"] == "test-task-1"
    assert data["estimated_hours"] > 0
    assert 0 <= data["confidence_score"] <= 1
    assert data["complexity_assessment"] == "medium"
    assert "estimated_at" in data

def test_assess_risks():
    """Test risk assessment endpoint"""
    request_data = {
        "project_id": "test-project-1",
        "project_data": {
            "deadline": "2024-12-31",
            "team_size": 2,
            "complexity": "high"
        }
    }
    
    response = client.post("/assess/risks", json=request_data)
    assert response.status_code == 200
    
    data = response.json()
    assert data["project_id"] == "test-project-1"
    assert 0 <= data["risk_score"] <= 1
    assert isinstance(data["risk_factors"], list)
    assert isinstance(data["mitigation_suggestions"], list)
    assert "assessed_at" in data

def test_models_status():
    """Test models status endpoint"""
    response = client.get("/models/status")
    assert response.status_code == 200
    
    data = response.json()
    assert data["project_analysis_model"] == "ready"
    assert data["task_estimation_model"] == "ready"
    assert data["risk_assessment_model"] == "ready"
    assert "last_updated" in data

def test_invalid_project_analysis():
    """Test project analysis with invalid data"""
    request_data = {
        "project_id": "",  # Empty project ID
        "description": ""   # Empty description
    }
    
    response = client.post("/analyze/project", json=request_data)
    assert response.status_code == 422  # Validation error

def test_invalid_task_estimation():
    """Test task estimation with invalid complexity"""
    request_data = {
        "task_id": "test-task-1",
        "description": "Test task",
        "complexity": "invalid"  # Invalid complexity
    }
    
    response = client.post("/estimate/time", json=request_data)
    assert response.status_code == 200  # Should still work with default logic
