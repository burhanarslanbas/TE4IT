# AI Service Development

Bu klasör AI/ML servisi için ayrılmıştır.

## 📋 Görevler

- [ ] AI service projesi oluştur (Python/FastAPI)
- [ ] ML modelleri entegre et
- [ ] Backend API entegrasyonu
- [ ] Project analysis algoritmaları
- [ ] Task estimation modelleri
- [ ] Risk assessment sistemi
- [ ] Testing setup
- [ ] Model deployment

## 🚀 Başlangıç

### 1. Proje Oluşturma
```bash
# Python virtual environment
python -m venv venv
source venv/bin/activate  # Linux/Mac
# veya
venv\Scripts\activate     # Windows

# FastAPI projesi oluştur
pip install fastapi uvicorn
```

### 2. Temel Yapı
```bash
# Proje klasör yapısı
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

## 🔗 Backend API Entegrasyonu

Backend API şu adreste çalışıyor:
- **URL**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### Backend'den AI Service'e İstekler
Backend'den şu endpoint'lere istekler gelecek:

```python
# Backend'den gelecek istekler
POST /analyze/project
POST /estimate/time  
POST /assess/risks
GET /health
```

### Örnek Response Format
```python
# Project Analysis Response
{
    "project_id": "uuid",
    "complexity_score": 0.7,
    "estimated_duration_days": 30,
    "risk_level": "medium",
    "recommendations": ["Break down tasks", "Add buffer time"],
    "analyzed_at": "2024-01-01T00:00:00Z"
}

# Task Estimation Response  
{
    "task_id": "uuid",
    "estimated_hours": 8,
    "confidence_score": 0.8,
    "complexity_assessment": "medium",
    "estimated_at": "2024-01-01T00:00:00Z"
}
```

## 📚 Dokümantasyon

Detaylı entegrasyon rehberi için:
- [Ana README](../README.md)
- [AI Entegrasyon Rehberi](../docs/AI_INTEGRATION.md)
- [Geliştirme Rehberi](../docs/DEVELOPMENT.md)

## 🛠️ Önerilen Teknolojiler

- **Framework**: FastAPI
- **Language**: Python 3.9+
- **ML Libraries**: scikit-learn, pandas, numpy
- **Deep Learning**: PyTorch, TensorFlow (opsiyonel)
- **Database**: PostgreSQL, Redis
- **Testing**: pytest
- **Deployment**: Docker, uvicorn

## 🤖 AI/ML Özellikleri

### Project Analysis
- Proje karmaşıklığı analizi
- Süre tahmini
- Risk değerlendirmesi
- Öneriler

### Task Estimation
- Görev süre tahmini
- Karmaşıklık analizi
- Bağımlılık analizi

### Risk Assessment
- Proje risk faktörleri
- Mitigation önerileri
- Trend analizi

## 🔧 Development Setup

1. Python 3.9+ kurulumu
2. Virtual environment
3. Dependencies kurulumu
4. IDE setup (VS Code/PyCharm)

## 📦 Build & Deployment

### Local Development
```bash
uvicorn main:app --reload
```

### Docker
```bash
docker build -t ai-service .
docker run -p 8000:8000 ai-service
```

### Production
- Cloud deployment (AWS/GCP/Azure)
- Model serving
- Monitoring & logging

## 🧪 Testing

```bash
# Unit tests
pytest tests/

# API tests
pytest tests/api/

# Model tests
pytest tests/models/
```

## 📊 Model Management

- Model versioning
- A/B testing
- Performance monitoring
- Model retraining

## 🔒 Security

- API authentication
- Input validation
- Rate limiting
- Data privacy

## 📈 Performance

- Caching strategies
- Async processing
- Load balancing
- Resource optimization

İyi kodlamalar! 🚀