# Self-Hosted Agent Kurulum Rehberi

## 1. Azure DevOps'ta Agent Pool Oluşturma
1. Azure DevOps → **Proje Ayarları**
2. **Agent Pools** → **Add pool**
3. **Self-hosted** seçin
4. Pool adı: `Local-Agent`

## 2. Agent İndirme ve Kurma
```bash
# Windows için
mkdir C:\azp-agent
cd C:\azp-agent

# Agent'ı indir ve çalıştır
Invoke-WebRequest -Uri "https://vstsagentpackage.azureedge.net/agent/3.220.5/vsts-agent-win-x64-3.220.5.zip" -OutFile "agent.zip"
Expand-Archive -Path "agent.zip" -DestinationPath "C:\azp-agent"

# Agent'ı yapılandır
.\config.cmd
```

## 3. Pipeline'ı Self-Hosted Agent'a Yönlendirme
```yaml
pool:
  name: 'Local-Agent'  # Self-hosted pool adı
```

## 4. Agent'ı Başlatma
```bash
.\run.cmd
```
