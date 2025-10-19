# Azure DevOps Deployment - Hızlı Başlangıç

## 🚀 5 Dakikada Deployment

### 1. Azure DevOps Setup (2 dakika)
1. [dev.azure.com](https://dev.azure.com) → **New Project**
2. **Import repository**: `https://github.com/burhanarslanbas/TE4IT.git`

### 2. Azure App Service (2 dakika)
1. [portal.azure.com](https://portal.azure.com) → **App Services** → **Create**
2. **Runtime**: `.NET 9` | **OS**: `Windows` | **Region**: `West Europe`
3. **Create**

### 3. Pipeline Setup (1 dakika)
1. Azure DevOps → **Pipelines** → **New pipeline**
2. **Azure Repos Git** → Repository seçin
3. **Existing Azure Pipelines YAML file** → `azure-pipelines.yml`
4. **Run**

## 🔐 Environment Variables

App Service → **Configuration** → **Application settings**:

```
CONNECTION_STRING=User Id=postgres.chrisnrtblexlktxwetq;Password=h+Hgg6sq@@c7#+W;Server=aws-1-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Command Timeout=120;Timeout=120

JWT_ISSUER=https://[your-app-name].azurewebsites.net
JWT_AUDIENCE=te4it-api
JWT_SIGNING_KEY=ToAyJiE3OPBfbv7GN0elgLNxxEwmXtd+7EyC76yxiss=

EMAIL_USERNAME=infoarslanbas@gmail.com
EMAIL_PASSWORD=rzowjgsyptscottc
```

## 📱 API URL

Deployment sonrası:
- **API**: `https://[your-app-name].azurewebsites.net`
- **Swagger**: `https://[your-app-name].azurewebsites.net/swagger`

## ✅ Avantajlar

- ✅ .NET 9 tam desteği
- ✅ Microsoft ekosistemi
- ✅ Ücretsiz tier mevcut
- ✅ Kolay scaling
- ✅ Built-in monitoring

## 🔄 Otomatik Deployment

Her GitHub push'unda otomatik deploy olur!
