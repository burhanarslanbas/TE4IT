# 🚀 Azure DevOps Deployment Rehberi - TE4IT API

## 📋 Azure DevOps Avantajları

✅ **Railway'e Göre Avantajları:**
- .NET 9 tam desteği
- Microsoft ekosistemi entegrasyonu
- Daha güçlü CI/CD pipeline'ları
- Azure App Service ile kolay deployment
- Ücretsiz tier'da daha fazla kaynak

## 🔧 Azure DevOps Kurulum Adımları

### 1. Azure DevOps Hesabı Oluşturma
1. [dev.azure.com](https://dev.azure.com) adresine gidin
2. Microsoft hesabınızla giriş yapın
3. Yeni bir Organization oluşturun

### 2. Proje Oluşturma
1. **New Project** butonuna tıklayın
2. Proje adı: `TE4IT`
3. Visibility: **Private**
4. Version control: **Git**
5. Work item process: **Agile**

### 3. Repository Bağlama
1. **Repos** → **Files** sekmesine gidin
2. **Import a repository** seçin
3. Repository URL: `https://github.com/burhanarslanbas/TE4IT.git`
4. **Import** butonuna tıklayın

## 🏗️ Azure App Service Deployment

### 1. Azure Portal'da App Service Oluşturma
1. [portal.azure.com](https://portal.azure.com) → **App Services**
2. **Create** → **Web App**
3. **Resource Group**: Yeni oluşturun (`TE4IT-RG`)
4. **Name**: `te4it-api-[random]`
5. **Runtime stack**: `.NET 9`
6. **Operating System**: `Windows`
7. **Region**: `West Europe`

### 2. App Service Konfigürasyonu
1. **Configuration** → **Application settings**
2. Aşağıdaki environment variables'ları ekleyin:

```
CONNECTION_STRING=User Id=postgres.chrisnrtblexlktxwetq;Password=h+Hgg6sq@@c7#+W;Server=aws-1-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Command Timeout=120;Timeout=120

JWT_ISSUER=https://te4it-api-[random].azurewebsites.net
JWT_AUDIENCE=te4it-api
JWT_SIGNING_KEY=ToAyJiE3OPBfbv7GN0elgLNxxEwmXtd+7EyC76yxiss=

EMAIL_USERNAME=infoarslanbas@gmail.com
EMAIL_PASSWORD=rzowjgsyptscottc
```

## 🔄 CI/CD Pipeline Kurulumu

### 1. Azure DevOps Pipeline Oluşturma
1. **Pipelines** → **Pipelines** → **New pipeline**
2. **Azure Repos Git** seçin
3. Repository'nizi seçin
4. **Starter pipeline** seçin

### 2. YAML Pipeline Konfigürasyonu
```yaml
trigger:
- main

pool:
  vmImage: 'windows-latest'

variables:
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'
  buildConfiguration: 'Release'

steps:
- task: UseDotNet@2
  displayName: 'Use .NET 9 SDK'
  inputs:
    packageType: 'sdk'
    version: '9.0.x'

- task: DotNetCoreCLI@2
  displayName: 'Restore packages'
  inputs:
    command: 'restore'
    projects: '$(solution)'

- task: DotNetCoreCLI@2
  displayName: 'Build solution'
  inputs:
    command: 'build'
    projects: '$(solution)'
    arguments: '--configuration $(buildConfiguration)'

- task: DotNetCoreCLI@2
  displayName: 'Publish API'
  inputs:
    command: 'publish'
    publishWebProjects: false
    projects: 'src/TE4IT.API/TE4IT.API.csproj'
    arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'

- task: PublishBuildArtifacts@1
  displayName: 'Publish artifacts'
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: 'drop'

- task: AzureWebApp@1
  displayName: 'Deploy to Azure App Service'
  inputs:
    azureSubscription: 'Azure-Service-Connection'
    appType: 'webApp'
    appName: 'te4it-api-[your-app-name]'
    package: '$(Build.ArtifactStagingDirectory)/**/*.zip'
```

## 🔐 Service Connection Kurulumu

### 1. Azure Service Connection
1. **Project Settings** → **Service connections**
2. **New service connection** → **Azure Resource Manager**
3. **Service principal (automatic)** seçin
4. **Subscription** ve **Resource group** seçin
5. **Service connection name**: `Azure-Service-Connection`

## 📱 API Endpoints

Deployment sonrası:
- **Base URL**: `https://te4it-api-[random].azurewebsites.net`
- **Swagger UI**: `https://te4it-api-[random].azurewebsites.net/swagger`

## 🔍 Test ve Monitoring

### 1. Application Insights
1. Azure Portal'da **Application Insights** oluşturun
2. App Service'e bağlayın
3. Real-time monitoring ve logging

### 2. Health Checks
```bash
# Health Check
curl https://te4it-api-[random].azurewebsites.net/swagger

# API Test
curl -X POST https://te4it-api-[random].azurewebsites.net/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'
```

## 💰 Maliyet

### Azure DevOps
- **Public projects**: Ücretsiz
- **Private projects**: 5 kullanıcıya kadar ücretsiz

### Azure App Service
- **Free tier**: 1 GB RAM, 1 GB storage
- **Basic tier**: ~$13/ay (1 GB RAM)
- **Standard tier**: ~$50/ay (1 GB RAM)

## 🚀 Hızlı Başlangıç

1. **Azure DevOps'a git**: [dev.azure.com](https://dev.azure.com)
2. **Proje oluştur** ve repository'yi import et
3. **Azure Portal'da App Service oluştur**
4. **Pipeline oluştur** ve deploy et
5. **Environment variables ekle**

## 📞 Destek

- Azure DevOps Docs: [docs.microsoft.com/azure/devops](https://docs.microsoft.com/azure/devops)
- Azure App Service Docs: [docs.microsoft.com/azure/app-service](https://docs.microsoft.com/azure/app-service)
