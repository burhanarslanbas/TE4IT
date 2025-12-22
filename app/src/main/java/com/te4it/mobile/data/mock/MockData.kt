package com.te4it.mobile.data.mock

import com.te4it.mobile.data.network.dto.TaskState
import com.te4it.mobile.data.network.dto.TaskType
import com.te4it.mobile.domain.model.Module
import com.te4it.mobile.domain.model.Project
import com.te4it.mobile.domain.model.Task
import com.te4it.mobile.domain.model.UseCase
import java.util.UUID

object MockData {

    val projects = listOf(
        Project(
            id = "1",
            title = "E-Ticaret SuperApp",
            description = "Giyim, elektronik ve market alışverişini tek çatı altında toplayan, yapay zeka destekli öneri sistemine sahip kapsamlı mobil uygulama.",
            isActive = true,
            startedDate = "2024-01-15T09:00:00Z",
            memberCount = 12,
            moduleCount = 5
        ),
        Project(
            id = "2",
            title = "Fintech Cüzdan",
            description = "Kripto para ve geleneksel bankacılık işlemlerini birleştiren, QR ile ödeme ve uluslararası para transferi sağlayan dijital cüzdan.",
            isActive = true,
            startedDate = "2024-02-01T10:30:00Z",
            memberCount = 8,
            moduleCount = 4
        ),
        Project(
            id = "3",
            title = "Sağlık Asistanı",
            description = "Hastaların ilaç takibini yapabileceği, doktor randevularını yönetebileceği ve giyilebilir cihazlarla entegre çalışan sağlık uygulaması.",
            isActive = true,
            startedDate = "2023-11-20T08:00:00Z",
            memberCount = 6,
            moduleCount = 3
        ),
        Project(
            id = "4",
            title = "Akıllı Ev Otomasyonu",
            description = "IoT cihazlarını tek merkezden yöneten, enerji tasarrufu senaryoları sunan ve sesli asistan desteği olan ev yönetim paneli.",
            isActive = true,
            startedDate = "2024-03-10T14:00:00Z",
            memberCount = 5,
            moduleCount = 2
        ),
        Project(
            id = "5",
            title = "Kurumsal CRM & ERP",
            description = "Büyük ölçekli şirketler için müşteri ilişkileri, stok yönetimi, faturalama ve İK süreçlerini kapsayan bütünleşik yönetim sistemi.",
            isActive = true,
            startedDate = "2023-09-01T09:00:00Z",
            memberCount = 20,
            moduleCount = 8
        ),
        Project(
            id = "6",
            title = "Online Eğitim Platformu",
            description = "Canlı dersler, video içerikler, sınav modülleri ve öğrenci performans takibi içeren kapsamlı uzaktan eğitim sistemi.",
            isActive = false,
            startedDate = "2023-12-05T11:00:00Z",
            memberCount = 15,
            moduleCount = 6
        ),
        Project(
            id = "7",
            title = "Lojistik Takip Sistemi",
            description = "Araç filolarının gerçek zamanlı takibi, rota optimizasyonu ve teslimat yönetimi sağlayan harita tabanlı operasyon paneli.",
            isActive = true,
            startedDate = "2024-04-01T08:30:00Z",
            memberCount = 7,
            moduleCount = 3
        ),
        Project(
            id = "8",
            title = "Sosyal Medya Analitiği",
            description = "Markalar için sosyal medya etkileşimlerini analiz eden, rakip analizi ve içerik önerileri sunan veri madenciliği aracı.",
            isActive = true,
            startedDate = "2024-02-20T13:00:00Z",
            memberCount = 4,
            moduleCount = 2
        )
    )

    val modules = listOf(
        Module("m1", "1", "Kullanıcı Yönetimi", "Auth ve Profil işlemleri", true, "2024-01-02T09:00:00Z", 5),
        Module("m2", "1", "Ürün Kataloğu", "Ürün listeleme ve detay", true, "2024-01-10T09:00:00Z", 4),
        Module("m3", "1", "Sepet & Sipariş", "Satın alma süreçleri", true, "2024-01-20T09:00:00Z", 3),
        Module("m4", "2", "Müşteri Veritabanı", "Müşteri kayıtları", true, "2024-02-16T09:00:00Z", 3),
        Module("m5", "2", "Satış Raporları", "Grafiksel raporlar", true, "2024-02-25T09:00:00Z", 2)
    )

    val useCases = listOf(
        UseCase("uc1", "m1", "Login", "Kullanıcı girişi", "Güvenlik önemli", true, "2024-01-05T09:00:00Z"),
        UseCase("uc2", "m1", "Register", "Kullanıcı kaydı", "Email doğrulama şart", true, "2024-01-06T09:00:00Z"),
        UseCase("uc3", "m2", "Ürün Arama", "Filtreli arama", null, true, "2024-01-15T09:00:00Z"),
        UseCase("uc4", "m3", "Sepete Ekle", "Stok kontrolü ile ekleme", "Stok yoksa uyarı ver", true, "2024-01-25T09:00:00Z")
    )

    val tasks = mutableListOf(
        Task(
            id = "t1",
            useCaseId = "uc1",
            creatorId = "user1",
            assigneeId = "user1",
            assigneeName = "Samet Yeşil",
            title = "Login Ekranı Tasarımı",
            description = "Figma tasarımına uygun login ekranı kodlanacak.",
            importantNotes = "Material 3 kullanılmalı.",
            startedDate = "2024-12-01T09:00:00Z",
            dueDate = "2024-12-05T17:00:00Z",
            taskType = TaskType.FEATURE,
            taskState = TaskState.COMPLETED
        ),
        Task(
            id = "t2",
            useCaseId = "uc1",
            creatorId = "user1",
            assigneeId = "user1",
            assigneeName = "Samet Yeşil",
            title = "JWT Entegrasyonu",
            description = "Backend'den gelen token DataStore'a kaydedilecek.",
            importantNotes = "Security best practices'e dikkat.",
            startedDate = "2024-12-06T10:00:00Z",
            dueDate = "2024-12-08T18:00:00Z",
            taskType = TaskType.FEATURE,
            taskState = TaskState.IN_PROGRESS
        ),
        Task(
            id = "t3",
            useCaseId = "uc2",
            creatorId = "user2",
            assigneeId = "user1",
            assigneeName = "Samet Yeşil",
            title = "Register Validasyonları",
            description = "Email ve şifre kuralları kontrol edilecek.",
            importantNotes = null,
            startedDate = null,
            dueDate = "2024-12-15T17:00:00Z",
            taskType = TaskType.BUG,
            taskState = TaskState.NOT_STARTED
        ),
        Task(
            id = "t4",
            useCaseId = "uc3",
            creatorId = "user1",
            assigneeId = "user1",
            assigneeName = "Samet Yeşil",
            title = "Ürün Detay Sayfası",
            description = "Resim galerisi ve özellikler tablosu.",
            importantNotes = "Coil kütüphanesi kullanılacak.",
            startedDate = "2024-11-20T09:00:00Z",
            dueDate = "2024-11-25T17:00:00Z", // Gecikmiş
            taskType = TaskType.FEATURE,
            taskState = TaskState.IN_PROGRESS
        ),
         Task(
            id = "t5",
            useCaseId = "uc4",
            creatorId = "user1",
            assigneeId = "user1",
            assigneeName = "Samet Yeşil",
            title = "Unit Testlerin Yazılması",
            description = "ViewModel katmanı için testler.",
            importantNotes = "Coverage %80 olmalı.",
            startedDate = null,
            dueDate = "2024-12-20T17:00:00Z",
            taskType = TaskType.TEST,
            taskState = TaskState.NOT_STARTED
        )
    )
}
