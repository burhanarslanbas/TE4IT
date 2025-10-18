# Proje Yönetimi Dokümantasyonu

Bu klasör TE4IT projesinin proje yönetimi, gereksinimler ve planlama dokümantasyonlarını içerir.

## 📋 İçindekiler

### 📄 Ürün Gereksinimleri Dokümanı (PRD)
- **[PRD_TE4IT_FINAL.md](./PRD_TE4IT_FINAL.md)** - Ana PRD dokümanı
  - Yürütücü Özeti
  - Ürün Genel Bakışı
  - Hedef Kullanıcılar ve Persona'lar
  - Fonksiyonel ve Fonksiyonel Olmayan Gereksinimler
  - Teknik Mimari
  - API Spesifikasyonları
  - Proje Zaman Çizelgesi ve Kilometre Taşları
  - Risk Değerlendirmesi

## 🎯 Proje Genel Bakış

### Proje Vizyonu
TE4IT, yazılım geliştirme ekipleri ve BT öğrencileri için proje yönetimi ile eğitimi tek platformda birleştiren kapsamlı bir sistemdir.

### Temel Değer Önerileri
- **Birleşik Platform**: Dağınık araçları tek sistemde toplar
- **Eğitim Entegrasyonu**: Proje çalışmasını öğrenme hedefleriyle harmanlar
- **AI Destekli İçgörüler**: Akıllı analiz ve öneriler sağlar

### İş Hedefleri
- Araç parçalanmasını %80 azaltmak
- Ekip verimliliğini %25 artırmak
- Proje teslim sürelerini %20 kısaltmak

## 👥 Hedef Kullanıcılar

### Ana Kullanıcı Tipleri
1. **Şirket Sahibi (Ahmet)** - Verimlilik ve maliyet optimizasyonu
2. **Yazılım Müşterisi (Zeynep)** - Proje takibi ve şeffaflık
3. **Proje Yöneticisi (Ayşe)** - Ekip koordinasyonu ve raporlama
4. **Yazılım Geliştirici (Mehmet)** - Görev yönetimi ve öğrenme
5. **BT Öğrencisi (Elif)** - Proje organizasyonu ve eğitim

## 🏗️ Teknik Mimari

### Platform Mimarisi
- **Backend**: .NET 9 Web API (Onion Architecture + CQRS)
- **Frontend**: React + TypeScript
- **Mobile**: Kotlin + Jetpack Compose (okuma odaklı)
- **AI Servisi**: FastAPI (Python)
- **Veritabanı**: PostgreSQL (Görevler) + MongoDB (Eğitim)

### Temel Özellikler
1. **Proje Yönetimi**: Hiyerarşik organizasyon (Proje → Modül → Kullanım Senaryosu → Görev)
2. **Görev Yönetimi**: Gelişmiş görev takibi ve bağımlılık yönetimi
3. **Eğitim İçeriği**: Kurs ve ders yönetimi ile ilerleme takibi
4. **AI Analitikleri**: Akıllı proje analizi ve görev tahminleri

## 📅 Proje Zaman Çizelgesi

### Sprint Planlaması (6 Sprint - 6 Hafta)

#### Sprint 1: Temel Altyapı (1 hafta)
- Backend: .NET 9 Web API kurulumu
- Frontend: React projesi kurulumu
- Mobile: Kotlin Android projesi kurulumu
- AI: FastAPI projesi kurulumu

#### Sprint 2: Proje Yönetimi (1 hafta)
- Backend: Proje CRUD API'leri
- Frontend: Proje listesi ve detay sayfaları
- Mobile: Proje listesi ekranı (okuma)
- AI: Temel proje analizi algoritması

#### Sprint 3: Görev Yönetimi (1 hafta)
- Backend: Görev CRUD API'leri
- Frontend: Görev listesi ve detay sayfaları
- Mobile: Görev listesi ekranı (okuma)
- AI: Görev tahmin algoritması

#### Sprint 4: Eğitim Modülü (1 hafta)
- Backend: MongoDB kurs API'leri
- Frontend: Kurs listesi ve detay sayfaları
- Mobile: Kurs listesi ekranı (okuma)
- AI: Öğrenme analizi algoritması

#### Sprint 5: Frontend ve Mobil (1 hafta)
- Frontend: Dashboard implementasyonu
- Mobile: Push notification entegrasyonu
- Backend: API optimizasyonları
- AI: AI servisi optimizasyonu

#### Sprint 6: AI ve Finalizasyon (1 hafta)
- AI: AI model fine-tuning
- Frontend: Final UI polish
- Mobile: Final testing
- Backend: Production hazırlığı

### Kilometre Taşları
- **MVP Release**: 4 hafta
- **Beta Test**: 5 hafta
- **Production Release**: 6 hafta

## 🎯 Başarı Kriterleri

### Ürün Hedefleri
- Kullanıcılar tek platformda çalışabilir
- Proje teslim süreleri kısalır
- Ekip koordinasyonu artar
- AI destekli içgörüler sağlanır

### Teknik Hedefler
- API yanıt süresi < 800ms
- Eşzamanlı kullanıcı desteği: 1000+
- Sistem uptime: %99.9
- Hata oranı: < 0.1%

## ⚠️ Risk Değerlendirmesi

### Teknik Riskler
- **AI Servisi Entegrasyon Karmaşıklığı**: Orta olasılık, Yüksek etki
- **Performans Yük Altında**: Orta olasılık, Yüksek etki

### İş Riskleri
- **Kapsam Genişlemesi**: Yüksek olasılık, Orta etki
- **Zaman Baskısı**: Orta olasılık, Yüksek etki

### Dış Riskler
- **Teknoloji Bağımlılıkları**: Düşük olasılık, Orta etki

## 🔗 Hızlı Linkler

- **[PRD Dokümanı](./PRD_TE4IT_FINAL.md)** - Detaylı gereksinimler
- **[Mimari Dokümantasyonu](../architecture/)** - Teknik tasarım
- **[API Dokümantasyonu](../api/)** - API kılavuzları
- **[Geliştirme Dokümantasyonu](../development/)** - Geliştirme süreçleri

## 📞 Destek

Proje yönetimi konularında sorun yaşarsanız:
1. PRD dokümanını kontrol edin
2. Mimari dokümantasyonunu inceleyin
3. GitHub Issues'da sorun bildirin
