# AI Roadmap API - Frontend Entegrasyon Dokümanı

Bu doküman, React veya diğer frontend framework'leri ile AI Roadmap API'sine nasıl istek atılacağını ve gelen hiyerarşik verinin nasıl işleneceğini açıklar.

## Temel Bilgiler

*   **Base URL:** `https://multi-source-search-api.onrender.com`
*   **Content-Type:** `application/json`

---

## 1. Yol Haritası Oluşturma (Roadmap Generation)

Kullanıcının proje fikrini alıp GitHub araması yapar ve Gemini AI ile hiyerarşik bir plan oluşturur.

*   **Endpoint:** `/plan/from-search`
*   **Method:** `POST`

### İstek Gövdesi (Request Body)

```json
{
  "title": "Proje Başlığı",
  "description": "Projenin detaylı açıklaması",
  "max_similar_projects": 3
}
```

### Yanıt Yapısı (Response Structure)

API, **Project -> Module -> UseCase -> Task** hiyerarşisinde bir veri döner.

```json
{
  "project_summary": "Projenin genel özeti",
  "tech_stack": ["React", "Node.js", "MongoDB"],
  "roadmap": [
    {
      "name": "Modül Adı (Örn: Kimlik Doğrulama)",
      "description": "Modülün ne işe yaradığı",
      "use_cases": [
        {
          "name": "Kullanım Durumu (Örn: Kullanıcı Kaydı)",
          "description": "Senaryo açıklaması",
          "tasks": [
            {
              "task": "Görev Adı",
              "priority": "high",
              "estimated_hours": 4,
              "description": "Görevin detaylı teknik açıklaması"
            }
          ]
        }
      ]
    }
  ],
  "similar_projects_found": 3,
  "key_insights": ["İpucu 1", "İpucu 2"]
}
```

---

## 2. React Entegrasyon Örneği (Axios)

Frontend tarafında veriyi çekmek ve state'e kaydetmek için örnek kod:

```javascript
import axios from 'axios';

const generateRoadmap = async (projectData) => {
  try {
    const response = await axios.post('https://multi-source-search-api.onrender.com/plan/from-search', {
      title: projectData.title,
      description: projectData.description,
      max_similar_projects: 5
    });

    const roadmap = response.data.roadmap;
    
    // Veriye erişim örneği:
    roadmap.forEach(module => {
      console.log("Modül:", module.name);
      module.use_cases.forEach(useCase => {
        console.log("  UseCase:", useCase.name);
        useCase.tasks.forEach(task => {
          console.log("    Görev:", task.task);
        });
      });
    });

    return response.data;
  } catch (error) {
    console.error("API Hatası:", error);
  }
};
```

---

## 3. Önemli Notlar

1.  **CORS:** API üzerinde CORS ayarları tüm origin'lere (`*`) izin verecek şekilde yapılandırılmıştır.
2.  **Zaman Aşımı (Timeout):** AI analizi ve GitHub araması nedeniyle istekler **20-40 saniye** sürebilir. Frontend tarafında mutlaka bir "Loading" (yükleniyor) animasyonu kullanılmalıdır.
3.  **Hiyerarşi:** Gelen veriyi render ederken iç içe `map` fonksiyonları kullanılması önerilir:
    *   `roadmap.map` (Modules)
    *   `module.use_cases.map` (UseCases)
    *   `useCase.tasks.map` (Tasks)
