PRD — TE4IT (Task and Education for IT)

Sürüm: 1.0  |  Tarih: 2025-10-13  |  Sahip: Ürün Ekibi

0. Yürütücü Özeti
- TE4IT, yazılım ekipleri ve öğrenciler için proje/görev yönetimini, eğitim içeriklerini ve AI destekli analiz/tahminleri tek çatı altında birleştirir.
- İlk sürüm hedefi: Kimlik, temel proje/görev yönetimi, eğitim okuma, AI sağlık kontrolü ile uçtan uca çalışır bir sistem.

1. Genel Bakış
1.1 Amaç ve Kapsam
- Problemler: Parçalı araçlar, izlenemeyen bağımlılıklar, eğitim ilerlemesinin görünmezliği.
- Çözüm: TE4IT ile projeler→modüller→kullanım senaryoları→görevler hiyerarşisi, eğitim ilerleme görünürlüğü ve AI destekli karar.
- Bu PRD; fonksiyonel/fonksiyonel olmayan gereksinimler, veri modeli, API görünümü, UX, güvenlik, metrikler ve yol haritasını kapsar.

1.2 Hedef Kitle ve Personalar
- Yönetici: Proje portföyü ve risk görünürlüğü arayan lider.
- Ekip Üyesi: Atanan görevlerini verimli yönetmek isteyen geliştirici/analist.
- Öğrenci: Ders içeriklerini takip edip ilerlemesini görmek isteyen kullanıcı.
- Ürün Sahibi: Kabul kriterleri ve yol haritasını yöneten paydaş.

1.3 Varsayımlar ve Bağımlılıklar
- Varsayımlar: Web ve Android istemcileri aktif; temel RBAC yeterli; AI çıktıları özet/tahmin odaklı.
- Bağımlılıklar: PostgreSQL, FastAPI tabanlı AI servisi, konteynerize yerel ortam.

2. Ürün Tanımı ve Özellikler
2.1 Ana Özellikler (Öncelik: P1 zorunlu, P2 önemli, P3 sonraki)
- Kimlik ve Oturum (P1)
- Kullanıcı/Rol/İzin Yönetimi (P1)
- Proje/Modül/Kullanım Senaryosu Yönetimi (P1)
- Görev Yönetimi ve Durum Akışları (P1)
- Görev Bağımlılık/İlişki Yönetimi (P2)
- Eğitim İçeriklerini Görüntüleme ve İlerleme (P1)
- Raporlama (Gecikme, İlerleme, Dağılım) (P2)
- AI Analiz/Öneri/Tahmin (P2)
- Bildirim Altyapısı (temel) (P3)

2.2 Kullanıcı Hikayeleri (örneklerin tamamı değildir)
- US-Auth-01: "Kullanıcı olarak kayıt olmak isterim ki sisteme giriş yapabileyim."
- US-Task-02: "Ekip üyesi olarak görev durumunu başlat→tamamla akışıyla güncelleyebileyim."
- US-AI-01: "Yönetici olarak proje için risk/öneri kartları göreyim."
- US-Learn-02: "Öğrenci olarak kurs ilerlememi yüzde olarak göreyim."

2.3 Kabul Kriterleri (örnek)
- Hatalarda Problem Details (application/problem+json) döner.
- Sayfalı listelerde P95 ≤ 800 ms.
- Döngüsel görev bağımlılığı 400 ile engellenir.

3. Teknik Gereksinimler
3.1 Sistem Mimarisi ve Teknolojiler
- .NET 9, Clean Architecture, CQRS; EF Core + PostgreSQL
- Kimlik: JWT (≤15 dk) + Refresh rotation; Policy-based Authorization
- Web: React+TS; Mobil: Kotlin (Android - Okuma Odaklı)
- AI: FastAPI; web-araştırma, süre tahmini; 5 sn timeout, 2 retry, 10 dk cache

3.2 Performans ve Ölçeklenebilirlik
- API P95 ≤ 800 ms; rapor uçları ≤ 1 sn; RPS hedefi 50+
- Yatay ölçeklenebilirlik; veritabanında indeksleme ve N+1 önleme

3.3 Güvenlik
- RBAC, rate limiting, input validation, merkezi hata yönetimi
- Log/iz kayıtları; duyarlı veriler için maskeleme; HTTPS zorunlu

3.4 Uyumluluk ve Destek Matrisi
- Tarayıcı: Son 2 major Chrome/Firefox/Edge; Android 10+

4. Veri ve Entegrasyon
4.1 Kavramsal Veri Modeli (özet)
- Project(id, createdBy, title, description, status, startDate)
- Module(id, projectId, title, status)
- UseCase(id, moduleId, title, description, notes, status)
- Task(id, useCaseId, title, description, notes, type, state, startDate, dueDate, assigneeId)
- TaskRelation(id, sourceTaskId, targetTaskId, relationType)
- Course(id, title), Lesson(id, courseId, title)
- Enrollment(userId, courseId), Progress(userId, lessonId, percentage)
- User(id, email, name), Role(id, name), RefreshToken(id, userId, expiresAt)

4.2 API Genel Bakış
- Auth: register, login, refresh, logout, password reset
- Users: liste/detay, rol atama/kaldırma
- Projects/Modules/UseCases/Tasks: CRUD + durum/ilişki işlemleri (Web); Okuma API'leri (Mobil)
- Reports: overdue, completable, progress
- Learning: courses, lessons, progress
- AI: analyze-project, estimate-task, research

5. Kullanıcı Arayüzü ve Deneyimi
5.1 Navigasyon ve Bilgi Mimarisi
- Giriş/Kayıt → Pano → Projeler → Modül → Kullanım Senaryosu → Görev
- Eğitim: Kurslar → Dersler → İçerik → İlerleme
- AI Paneli: Analiz/Tahmin kartları

5.2 Kullanılabilirlik ve Erişilebilirlik
- WCAG 2.1 AA hedefi; klavye ile gezinilebilirlik; form doğrulama geri bildirimleri

6. Roller ve İzinler
- Yönetici: Proje/görev CRUD, rol atama, raporlar
- Ekip Üyesi: Kendi görevlerini görüntüle/güncelle, durum değişimi (Web); Sadece görüntüleme (Mobil)
- Öğrenci: Eğitim içeriklerini ve ilerlemeyi görüntüleme (Web/Mobil)
- Ürün Sahibi: Raporları görüntüleme, kabul işaretleme (okuma ağırlıklı)

7. Başarı Metrikleri ve Analitik
- Görev akışlarında başarılı oran ≥ %99; API P95 ≤ 800 ms
- AI kart kullanım oranı ≥ %30; geciken görev oranı ≤ %10
- Eğitim ilerleme ortalaması; kullanıcı başına açık görev sayısı

8. Test ve Doğrulama
- Birim+entegrasyon testleri ≥ %60 kapsam; e2e kritik akışlar
- Performans testleri (kısıtlı yük); güvenlik taramaları (SAST/DAST)

9. Zaman Çizelgesi ve Sürüm Planı
- S1: Kimlik, temel proje/görev, eğitim okuma, AI sağlık
- S2: Görev ilişkileri, raporlar, süre tahmini, görev akış ekranları
- S3: Eğitim ilerleme/özet zenginleştirme, mobil okuma ekranları, UI parlatma
- S4: Güvenlik politikaları genişletme, log/izleme, gelişmiş filtre
- S5: Stabilizasyon, dokümantasyon, sunum

10. Riskler ve Azaltma Stratejileri
- Zaman kısıtı → kapsamı dilimleme; haftalık çalışır çıktı
- Harici kaynaklara bağımlı AI → timeout/retry/cache
- Yetki kuralları → basit RBAC başlat, kademeli genişlet

11. Canlıya Alma Kriterleri
- E2E akışlar 2xx; problem detayları standardı; gözlemlenebilirlik aktif
- Güvenlik kritik açık 0; loglama ve temel metrikler mevcut

12. Ekler ve Sözlük
- Terimler: Tamamlanabilirlik, Görev İlişkisi, İlerleme Yüzdesi
- Referans: Swagger, mimari diyagramlar, kurulum komutları