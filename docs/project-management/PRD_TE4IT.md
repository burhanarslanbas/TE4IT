PRD — TE4IT (Task and Education for IT)

Sürüm: 1.0  |  Tarih: 2025-10-13  |  Sahip: Ürün Ekibi

1. Amaç, Hedefler ve Başarı Ölçütleri
1.1 Amaç
Projeleri modüller ve kullanım senaryoları ile yönetmek, görev akışları ve bağımlılıklarını takip etmek, eğitim içerikleriyle ekip yetkinliğini artırmak ve yapay zekâ ile araştırma/öneri desteği sağlamaktır.

1.2 Hedefler
- Tek noktadan görünürlük ve hızlı planlama/yürütme
- Ekip içi iş birliği ve izlenebilir sorumluluk
- Eğitim ilerlemesiyle yetkinlik artışı
- AI destekli öngörü ve önerilerle verimlilik artışı

1.3 Başarı Ölçütleri (KPI hedefleri)
- Görev akışlarında başarılı işlem oranı ≥ %99 (HTTP 2xx)
- Ortalama API yanıt süresi ≤ 300 ms (P95 ≤ 800 ms)
- İlk sürümde 0 kritik güvenlik açığı (SAST/DAST)
- AI öneri kartlarının en az %30 kullanım oranı

2. Kapsam
2.1 Kapsam
- Proje yönetimi, görev yönetimi, kullanım senaryosu/modül yönetimi
- Eğitim içeriği okuma ve basit ilerleme takibi
- Kimlik doğrulama, kullanıcı/rol/izin yönetimi
- Raporlama ve AI destekli analiz/tahmin

2.2 Kapsam Dışı (bu sürüm)
- Gelişmiş içerik üretimi/editör, gerçek zamanlı sohbet/bildirim
- Kurumsal entegrasyonlar (Jira/Slack/LDAP) ve ileri ML modelleri

3. Paydaşlar ve Roller
- Öğretim Görevlisi (Ürün Sahibi)
- Scrum Master
- Backend Geliştirici
- Frontend Geliştirici (Web)
- Mobil Geliştirici (Android)
- Yapay Zekâ Geliştiricisi
- Son Kullanıcılar (Yönetici, Ekip Üyesi, Öğrenci)

4. Personalar ve Ana Kullanım Senaryoları
4.1 Yönetici
- Proje oluşturur, ekibi atar, ilerlemeyi ve riskleri takip eder, raporları okur.

4.2 Ekip Üyesi
- Kendine atanan görevleri yürütür, durum değiştirir, bağımlılıkları yönetir.

4.3 Öğrenci
- Kurs/dersleri görüntüler, içerikleri inceler, ilerlemesini takip eder.

4.4 Ürün Sahibi
- Kabul kriterlerini doğrular, geri bildirim verir, yol haritasını önceliklendirir.

5. Fonksiyonel Gereksinimler (User Story + Kabul Kriterleri)
5.1 Kimlik Doğrulama ve Oturum
US-Auth-01: Kullanıcı olarak kayıt olmak isterim.
- Kabul: Geçerli e-posta/parola ile 201 döner; doğrulama e-postası opsiyonel.
US-Auth-02: Kullanıcı olarak giriş yapmak isterim.
- Kabul: Doğru kimlik bilgileriyle 200 ve JWT/Refresh döner; yanlışta 401.
US-Auth-03: Oturumu yenilemek isterim.
- Kabul: Geçerli refresh ile 200 ve yeni access token.
US-Auth-04: Parolamı sıfırlamak/değiştirmek isterim.
- Kabul: Kısıtlar (min uzunluk, karmaşıklık), başarılı işlem 200.
US-Auth-05: Oturumu sonlandırmak isterim.
- Kabul: Aktif refresh token iptal edilir, 204.

5.2 Kullanıcı ve Rol Yönetimi
US-User-01: Yetkili olarak kullanıcıları listelemek isterim.
- Kabul: Sayfalı sonuç, filtreleme (rol, durum), 200.
US-User-02: Rol atamak/kaldırmak isterim.
- Kabul: İşlem sonrası 200; denetim izi kaydı tutulur.
US-User-03: İzin politikalarıyla işlemleri kısıtlamak isterim.
- Kabul: Rol/izin ihlallerinde 403 ve standart hata gövdesi.

5.3 Proje Yönetimi
US-Proj-01: Proje oluşturmak/listemek/detaya bakmak isterim.
- Kabul: Zorunlu alanlar doğrulanır, arama/filtreleme/sıralama desteklenir.
US-Proj-02: Proje durumunu (aktif/arşiv) değiştirmek isterim.
- Kabul: 200; arşivde yeni görev ataması yapılamaz.
US-Proj-03: Modül ve kullanım senaryosu yönetmek isterim.
- Kabul: Hiyerarşi doğrulanır; silme bağımlılık kontrolü yapılır.

5.4 Görev Yönetimi
US-Task-01: Görev oluştur/ata/güncelle.
- Kabul: Alan doğrulama; 201/200; denetim izi.
US-Task-02: Durum akışı (başlat, tamamla, iptal, geri al).
- Kabul: Geçiş kuralları uygulanır; uygunsuz geçişte 409.
US-Task-03: Bitiş tarihi ve gecikme hesabı.
- Kabul: Gecikme gün sayısı otomatik hesaplanır.
US-Task-04: Görev bağımlılık/ilişki (bloklayan/bağımlı) ekle/çıkar.
- Kabul: Döngüsel bağımlılık engellenir; 400.
US-Task-05: Tamamlanabilirlik kontrolü.
- Kabul: Tüm engeller kalktıysa ve durum uygunsa tamamlanabilir.
US-Task-06: Listeleme/sayfalama/sıralama/filtreleme.
- Kabul: Paged sonuç, P95 ≤ 800 ms.

5.5 Eğitim Modülü
US-Learn-01: Kurs/ders listelerini görüntüle.
- Kabul: Sade liste, arama/filtreleme.
US-Learn-02: İlerleme yüzdelerinin hesaplanması.
- Kabul: Tamamlanan ders / toplam ders x 100.
US-Learn-03: İçerik açıklama/doküman/görsel görüntüleme.
- Kabul: Okuma odaklı; erişim kontrolü.

5.6 Yapay Zekâ Destekli Özellikler
US-AI-01: Proje için kısa analiz/öneri/risk kartları.
- Kabul: 5 sn zaman aşımı, 2 yeniden deneme, önbellek 10 dk.
US-AI-02: Görev süre tahmini.
- Kabul: 80% güven aralığı ile yaklaşık değer; belirsizlik metni.
US-AI-03: İnternet araştırması özetleri.
- Kabul: Kaynak linkleri ile 3-5 maddelik özet.

5.7 Raporlama ve İzleme
US-Rep-01: Geciken görevler listesi ve özet sayılar.
US-Rep-02: Tamamlanabilir görevler listesi.
US-Rep-03: Proje ilerleme yüzdesi (tamamlanan/toplam).
US-Rep-04: Kullanıcı bazlı görev dağılımı.
US-Rep-05: Eğitim ilerleme özetleri.

5.8 Bildirimler ve İletişim
- Sistem içi bildirim altyapısı (temel); e-posta/mobil bildirim bir sonraki sürüm.

6. Fonksiyonel Olmayan Gereksinimler (NFR)
- Güvenilirlik: API hata gövdesi standart (problem+json), P99 hata oranı ≤ %1.
- Kullanılabilirlik: Temel akışlar 3 adımı aşmamalı; WCAG 2.1 AA.
- Performans: P95 yanıt ≤ 800 ms; sayfalı listelerde ≤ 1 sn.
- Ölçeklenebilirlik: Katmanlı mimari, yatay ölçeklenebilir servisler.
- Güvenlik: JWT kısa ömür (≤15 dk), refresh rotation, RBAC, rate-limit.
- Gözlemlenebilirlik: Yapılandırılmış log, hata izleme, temel metrikler (RPS, lat, err%).
- Bakım yapılabilirlik: Modüler, DI, test edilebilir (unit+integration ≥ %60).
- Uyumluluk: Lisans ve KVKK/GDPR prensiplerine uygunluk.

7. Veri Modeli ve Kavramsal Varlıklar
- Proje: id, createdBy, title, description, status, startDate
- Modül: id, projectId, title, status
- Kullanım Senaryosu: id, moduleId, title, description, notes, status
- Görev: id, useCaseId, title, description, notes, type, state, startDate, dueDate, assigneeId
- Görev İlişkisi: id, sourceTaskId, targetTaskId, relationType
- Eğitim: Course(id, title), Lesson(id, courseId, title), Enrollment(userId, courseId), Progress(userId, lessonId, percentage)
- Kimlik: User(id, email, name), Role(id, name), RefreshToken(id, userId, expiresAt)

8. Mimari ve Teknoloji Yığını
- Backend: .NET 9, Clean Architecture, CQRS, EF Core, PostgreSQL
- Kimlik: JWT + Refresh, Policy-based Authorization
- Web: React + TypeScript
- Mobil: Kotlin (Android)
- AI Servisi: FastAPI (özet, öneri, süre tahmini), web-araştırma odaklı
- Dokümantasyon: Swagger/OpenAPI

9. Güvenlik ve Uyum
- Token ömrü kısa, refresh ile yönetim; HTTPS zorunlu
- Rol/izin politikaları; girdi doğrulama ve merkezi hata yönetimi
- Log/iz kayıtları yetkisiz erişime karşı korunur
- Duyarlı veriler için maskeleme ve en az ayrıcalık ilkesi

10. UX Bilgi Mimarisi ve Akışlar
- Giriş/Kayıt → Pano (atanan görevler, gecikenler, hızlı bağlantılar)
- Proje Listesi → Proje Detay → Modül → Kullanım Senaryosu → Görev
- Görev ekranı: durum değişimi, atama, tarih güncelleme
- Eğitim: Kurs Listesi → Ders Listesi → İçerik → İlerleme
- AI Paneli: Proje analizi, görev süresi tahmini, araştırma özet kartları
- Ayarlar/Profil: parola değişimi, tercihler

11. API Genel Bakış (read-only seviye; detay Swagger’da)
- Auth: POST /auth/register, POST /auth/login, POST /auth/refresh, POST /auth/logout, POST /auth/password/reset
- Users: GET /users, GET /users/{id}, POST /users/{id}/roles, DELETE /users/{id}/roles/{role}
- Projects: POST/GET /projects, GET /projects/{id}, PATCH /projects/{id}/status
- Modules/UseCases: CRUD /projects/{id}/modules, /modules/{id}/use-cases
- Tasks: CRUD /use-cases/{id}/tasks; POST /tasks/{id}/state, POST /tasks/{id}/relations
- Reports: GET /reports/overdue, /reports/completable, /reports/progress
- Learning: GET /courses, /courses/{id}/lessons, /users/{id}/learning/progress
- AI: POST /ai/analyze-project, POST /ai/estimate-task, POST /ai/research

12. Rol ve İzin Matrisi (özet)
- Yönetici: Tüm proje/görev CRUD, rol atama, rapor görüntüleme
- Ekip Üyesi: Kendi görevlerini görüntüle/güncelle, durum değişimi, not/tarih
- Öğrenci: Eğitim içeriklerini görüntüleme, ilerleme görüntüleme
- Ürün Sahibi: Raporlar/analizler, kabul işaretleme (okuma/ağ.

13. Raporlama, Metrikler ve Analitik
- Proje ilerleme yüzdesi, görev tamamlama oranı
- Geciken görev sayısı/oranı, kullanıcı başına açık görev
- Eğitim ilerleme ortalaması, tamamlanan ders sayısı
- AI öneri görüntülenme/kullanım ve dönüşüm oranı
- Sistem metrikleri: RPS, latency (P50/P95), hata oranı

14. Yol Haritası ve Sürümler
Sürüm 1: Kimlik, temel proje/görev CRUD, eğitim okuma, AI sağlık kontrolü
- Hedef: Çekirdek akışların uçtan uca çalışması
Sürüm 2: Görev ilişkileri, raporlar, gelişmiş görev akış ekranları, süre tahmini
Sürüm 3: Eğitim ilerleme/özetleri, UI/UX parlatma, mobil okuma genişletme
Sürüm 4: Güvenlik politikaları genişletme, log/izleme, gelişmiş filtreleme
Sürüm 5: Stabilizasyon, dokümantasyon, final sunum

15. Riskler, Varsayımlar, Bağımlılıklar
- Zaman kutulaması: Haftalık çalışır çıktı; kapsam küçük parçalara bölünür
- Web araştırması bağımlılığı: Zaman aşımı, retry, önbellek ile dayanıklılık
- Önce okuma akışları ve kritik CRUD; yetki ilk etapta basit RBAC
- Yerel geliştirme: PostgreSQL ve FastAPI konteyner ile

16. Kabul Kriterleri (Örnek, E2E)
- Auth: Kayıt/giriş/yenileme/çıkış akışları 2xx ve hatalarda anlamlı sorun nesnesi
- Proje: Oluşturma→Liste→Detay→Arşiv akışı; arşivde görev ataması engellenir
- Görev: Başlat→Tamamla; uygunsuz geçişlerde 409
- İlişki: Ekle/Kaldır; döngüsel bağımlılık 400 ile engellenir
- Eğitim: Kurs/ders listeleri ve ilerleme yüzdesi doğru hesaplanır
- AI: Sağlık kontrolü 200; en az bir analiz veya tahmin döner (≤ 5 sn)

17. Sözlük ve Ekler
- Görev İlişkisi: İki görev arasındaki mantıksal bağ (bloklayan/bağımlı)
- Tamamlanabilirlik: Durum ve ilişkilerin tamamlamaya uygunluğu
- İlerleme Yüzdesi: Tamamlanan/Toplam oranı
- Ekler: Geliştirme kılavuzu, AI entegrasyon notları, başlangıç komutları, ekranlar