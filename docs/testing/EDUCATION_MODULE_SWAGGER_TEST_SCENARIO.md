# Eğitim Modülü - Swagger Test Senaryosu

**Versiyon:** 1.0  
**Tarih:** 2025-01-27  
**Hazırlayan:** TE4IT Development Team  
**Amaç:** Eğitim modülünün tüm fonksiyonlarını Swagger üzerinden test etmek

---

## 📋 İçindekiler

1. [Test Senaryosu Genel Bakış](#1-test-senaryosu-genel-bakış)
2. [Ön Hazırlık](#2-ön-hazırlık)
3. [Admin Test Senaryosu](#3-admin-test-senaryosu)
4. [Kullanıcı Test Senaryosu](#4-kullanıcı-test-senaryosu)
5. [Test Verileri Referansı](#5-test-verileri-referansı)

---

## 1. Test Senaryosu Genel Bakış

Bu test senaryosu, eğitim modülünün tüm fonksiyonlarını kapsamlı bir şekilde test etmek için hazırlanmıştır. İki farklı kullanıcı tipi ile test yapılacaktır:

### ⚠️ Önemli Not: MongoDB Mapping

Eğer roadmap oluştururken `Steps` array'i MongoDB'ye kaydedilmiyorsa, `EducationMongoMappings.cs` dosyasında `RegisterCourseRoadmapMapping()` ve `RegisterRoadmapStepMapping()` metodlarında explicit mapping'lerin eklenmiş olduğundan emin olun. Read-only collection property'leri (`Steps`, `Contents`) explicit olarak map edilmelidir.

1. **Admin Kullanıcı**: Kurs oluşturma, roadmap yönetimi, kurs güncelleme
2. **Normal Kullanıcı**: Kurs görüntüleme, kayıt olma, ilerleme takibi, kurs tamamlama

### Test Kapsamı

- ✅ Kurs oluşturma ve yönetimi
- ✅ Roadmap oluşturma ve güncelleme
- ✅ Kurs listeleme ve detay görüntüleme
- ✅ Kursa kayıt olma
- ✅ İçerik erişimi ve tamamlama
- ✅ İlerleme takibi
- ✅ Dashboard görüntüleme
- ✅ Video ilerleme takibi
- ✅ Kurs tamamlama

---

## 2. Ön Hazırlık

### 2.1 Gerekli Kullanıcılar

Test için iki farklı kullanıcı hesabı gereklidir:

1. **Admin Kullanıcı**
   - Email: `admin@te4it.com`
   - Rol: `Administrator`
   - Yetkiler: Tüm eğitim modülü yetkileri

2. **Normal Kullanıcı**
   - Email: `user@te4it.com`
   - Rol: `Employee` (veya herhangi bir kullanıcı rolü)
   - Yetkiler: Kurs görüntüleme, kayıt olma, ilerleme takibi

### 2.2 Swagger Erişimi

1. Uygulamayı başlatın
2. Swagger UI'ya gidin: `https://localhost:5001/swagger` (veya uygulamanızın Swagger URL'i)
3. Authentication endpoint'ini kullanarak JWT token alın
4. Swagger UI'da "Authorize" butonuna tıklayın
5. Token'ı `Bearer {token}` formatında girin

### 2.3 Test Sırası

Test senaryosu sırayla takip edilmelidir:
1. Önce Admin kullanıcı ile tüm admin işlemleri yapılır
2. Sonra Normal kullanıcı ile tüm kullanıcı işlemleri yapılır

---

## 3. Admin Test Senaryosu

### 3.1 Admin ile Giriş Yapma

**Endpoint:** `POST /api/v1/auth/login` (veya mevcut auth endpoint'iniz)

**Request Body:**
```json
{
  "email": "admin@te4it.com",
  "password": "Admin123!"
}
```

**Response'dan token'ı alın ve Swagger'da "Authorize" butonuna tıklayarak token'ı girin.**

---

### 3.2 Kurs Oluşturma

**Endpoint:** `POST /api/v1/education/courses`

**Request Body:**
```json
{
  "title": "C# Temelleri ve İleri Seviye Programlama",
  "description": "Bu kurs, C# programlama dilinin temel kavramlarından başlayarak ileri seviye konulara kadar kapsamlı bir öğrenme deneyimi sunar. Nesne yönelimli programlama, LINQ, async/await, Entity Framework gibi modern C# özelliklerini öğreneceksiniz. Kurs sonunda gerçek dünya projeleri geliştirebilecek seviyeye geleceksiniz.",
  "thumbnailUrl": "https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=800&h=600&fit=crop"
}
```

**Beklenen Response:**
- Status: `201 Created`
- Response body'de `id` (Guid) değerini not edin. Bu ID'yi sonraki adımlarda kullanacaksınız.

**Not:** Response'dan dönen `courseId` değerini kaydedin (örnek: `550e8400-e29b-41d4-a716-446655440000`)

---

### 3.3 Roadmap Oluşturma

**Endpoint:** `POST /api/v1/education/courses/{courseId}/roadmap`

**Not:** `{courseId}` yerine 3.2 adımında aldığınız kurs ID'sini kullanın.

**Request Body:**
```json
{
  "title": "C# Temelleri Yolu - Sıfırdan Uzmanlığa",
  "description": "Bu roadmap, C# programlama dilini sıfırdan öğrenmek isteyenler için tasarlanmıştır. Adım adım ilerleyerek temel kavramlardan başlayıp ileri seviye konulara kadar uzanacaksınız.",
  "estimatedDurationMinutes": 720,
  "steps": [
    {
      "title": "Adım 1: C# Programlama Dili ile Tanışma",
      "description": "C# programlama dilinin temelleri, tarihçesi ve .NET ekosistemi hakkında bilgi edinin.",
      "order": 1,
      "isRequired": true,
      "estimatedDurationMinutes": 90,
      "contents": [
        {
          "type": 1,
          "title": "C# Nedir? - Kapsamlı Giriş Yazısı",
          "description": "C# programlama diline giriş ve temel kavramlar",
          "order": 1,
          "isRequired": true,
          "content": "<h1>C# Programlama Dili Nedir?</h1><p>C# (C-Sharp), Microsoft tarafından 2000 yılında geliştirilen modern, nesne yönelimli ve tip güvenli bir programlama dilidir. C# dili, .NET Framework ve .NET Core platformları üzerinde çalışır ve güçlü bir derleyici, zengin bir kütüphane ekosistemi ve geniş bir topluluk desteği sunar.</p><h2>C#'ın Özellikleri</h2><ul><li><strong>Nesne Yönelimli:</strong> Sınıflar, kalıtım, polimorfizm ve encapsulation gibi OOP prensiplerini destekler.</li><li><strong>Tip Güvenli:</strong> Derleme zamanında tip kontrolü yapılır, runtime hataları azaltılır.</li><li><strong>Modern Özellikler:</strong> LINQ, async/await, pattern matching, nullable reference types gibi modern özellikler içerir.</li><li><strong>Cross-Platform:</strong> .NET Core ile Windows, Linux ve macOS'ta çalışır.</li></ul><h2>Neden C# Öğrenmeliyim?</h2><p>C# öğrenmek size şu avantajları sağlar:</p><ul><li>Web uygulamaları (ASP.NET Core)</li><li>Mobil uygulamalar (Xamarin, .NET MAUI)</li><li>Oyun geliştirme (Unity)</li><li>Masaüstü uygulamaları (WPF, WinForms)</li><li>Mikroservisler ve cloud uygulamaları</li></ul><pre><code>// İlk C# Programınız\nusing System;\n\nclass Program\n{\n    static void Main()\n    {\n        Console.WriteLine(\"Merhaba, C# Dünyası!\");\n    }\n}</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Programlama Dili Tanıtım Videosu",
          "description": "C# dilinin temel özelliklerini anlatan kapsamlı video",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=GhQdlIFylQ8"
        },
        {
          "type": 4,
          "title": "Microsoft C# Dokümantasyonu",
          "description": "Resmi Microsoft C# dokümantasyonu",
          "order": 3,
          "isRequired": false,
          "content": null,
          "linkUrl": "https://learn.microsoft.com/dotnet/csharp/"
        }
      ]
    },
    {
      "title": "Adım 2: Değişkenler, Veri Tipleri ve Operatörler",
      "description": "C# dilinde değişken tanımlama, veri tipleri ve operatörler hakkında detaylı bilgi.",
      "order": 2,
      "isRequired": true,
      "estimatedDurationMinutes": 120,
      "contents": [
        {
          "type": 1,
          "title": "Değişkenler ve Veri Tipleri - Detaylı Rehber",
          "description": "C# dilinde değişken tanımlama ve veri tipleri",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Değişkenler ve Veri Tipleri</h1><p>C# dilinde değişkenler, belirli bir veri tipinde değer saklamak için kullanılır. C# güçlü bir tip sistemi kullanır, yani her değişkenin bir tipi olmalıdır.</p><h2>Değişken Tanımlama</h2><pre><code>// Değişken tanımlama\nint sayi = 10;\nstring metin = \"Merhaba\";\nbool dogruMu = true;\ndouble ondalik = 3.14;</code></pre><h2>Veri Tipleri</h2><h3>Değer Tipleri (Value Types)</h3><ul><li><strong>int:</strong> 32-bit tam sayı (-2,147,483,648 ile 2,147,483,647 arası)</li><li><strong>long:</strong> 64-bit tam sayı</li><li><strong>float:</strong> 32-bit ondalıklı sayı</li><li><strong>double:</strong> 64-bit ondalıklı sayı</li><li><strong>bool:</strong> true veya false</li><li><strong>char:</strong> Tek karakter</li></ul><h3>Referans Tipleri (Reference Types)</h3><ul><li><strong>string:</strong> Metin dizisi</li><li><strong>object:</strong> Tüm tiplerin temel sınıfı</li><li><strong>Array:</strong> Dizi</li></ul><h2>Var ve Dynamic</h2><pre><code>// var kullanımı (tip çıkarımı)\nvar sayi = 10; // int olarak algılanır\nvar metin = \"Test\"; // string olarak algılanır\n\n// dynamic kullanımı\ndynamic degisken = 10;\ndegisken = \"Şimdi string\"; // Runtime'da tip değişebilir</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Değişkenler ve Veri Tipleri Eğitim Videosu",
          "description": "Pratik örneklerle değişkenler ve veri tipleri",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=YrtFtdTTfv0"
        },
        {
          "type": 3,
          "title": "C# Veri Tipleri Referans Dokümanı (PDF)",
          "description": "Tüm C# veri tiplerinin detaylı listesi",
          "order": 3,
          "isRequired": false,
          "content": null,
          "linkUrl": "https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types"
        }
      ]
    },
    {
      "title": "Adım 3: Kontrol Yapıları ve Döngüler",
      "description": "if-else, switch-case, for, while gibi kontrol yapılarını öğrenin.",
      "order": 3,
      "isRequired": true,
      "estimatedDurationMinutes": 100,
      "contents": [
        {
          "type": 1,
          "title": "Kontrol Yapıları - if, else, switch",
          "description": "Koşullu ifadeler ve kontrol yapıları",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Kontrol Yapıları</h1><p>C# dilinde program akışını kontrol etmek için çeşitli yapılar kullanılır.</p><h2>if-else Yapısı</h2><pre><code>int yas = 18;\n\nif (yas >= 18)\n{\n    Console.WriteLine(\"Reşit\");\n}\nelse\n{\n    Console.WriteLine(\"Reşit değil\");\n}\n\n// Ternary operator\nstring durum = yas >= 18 ? \"Reşit\" : \"Reşit değil\";</code></pre><h2>switch-case Yapısı</h2><pre><code>int gun = 3;\n\nswitch (gun)\n{\n    case 1:\n        Console.WriteLine(\"Pazartesi\");\n        break;\n    case 2:\n        Console.WriteLine(\"Salı\");\n        break;\n    case 3:\n        Console.WriteLine(\"Çarşamba\");\n        break;\n    default:\n        Console.WriteLine(\"Geçersiz gün\");\n        break;\n}</code></pre><h2>Pattern Matching (C# 7.0+)</h2><pre><code>object obj = \"Test\";\n\nswitch (obj)\n{\n    case string s:\n        Console.WriteLine($\"String: {s}\");\n        break;\n    case int i:\n        Console.WriteLine($\"Integer: {i}\");\n        break;\n    default:\n        Console.WriteLine(\"Bilinmeyen tip\");\n        break;\n}</code></pre>",
          "linkUrl": null
        },
        {
          "type": 1,
          "title": "Döngüler - for, while, foreach",
          "description": "Döngü yapıları ve kullanım örnekleri",
          "order": 2,
          "isRequired": true,
          "content": "<h1>Döngüler</h1><p>C# dilinde tekrarlayan işlemler için döngüler kullanılır.</p><h2>for Döngüsü</h2><pre><code>// Klasik for döngüsü\nfor (int i = 0; i < 10; i++)\n{\n    Console.WriteLine(i);\n}\n\n// Sonsuz döngü (dikkatli kullanın!)\nfor (;;)\n{\n    // Kod\n    break; // Çıkış için\n}</code></pre><h2>while Döngüsü</h2><pre><code>int sayac = 0;\nwhile (sayac < 10)\n{\n    Console.WriteLine(sayac);\n    sayac++;\n}\n\n// do-while döngüsü\nint x = 0;\ndo\n{\n    Console.WriteLine(x);\n    x++;\n} while (x < 10);</code></pre><h2>foreach Döngüsü</h2><pre><code>string[] isimler = { \"Ali\", \"Ayşe\", \"Mehmet\" };\n\nforeach (string isim in isimler)\n{\n    Console.WriteLine(isim);\n}\n\n// LINQ ile birlikte\nvar sayilar = new List<int> { 1, 2, 3, 4, 5 };\nforeach (var sayi in sayilar.Where(x => x > 2))\n{\n    Console.WriteLine(sayi);\n}</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Kontrol Yapıları ve Döngüler Video Eğitimi",
          "description": "Pratik örneklerle kontrol yapıları",
          "order": 3,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=IF6k0u8p7-I"
        }
      ]
    },
    {
      "title": "Adım 4: Diziler, Koleksiyonlar ve LINQ",
      "description": "Diziler, List, Dictionary gibi koleksiyonlar ve LINQ sorguları.",
      "order": 4,
      "isRequired": true,
      "estimatedDurationMinutes": 150,
      "contents": [
        {
          "type": 1,
          "title": "Diziler ve Koleksiyonlar Rehberi",
          "description": "C# koleksiyon tipleri ve kullanımları",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Diziler ve Koleksiyonlar</h1><p>C# dilinde veri gruplarını saklamak için çeşitli yapılar kullanılır.</p><h2>Diziler (Arrays)</h2><pre><code>// Dizi tanımlama\nint[] sayilar = new int[5];\nsayilar[0] = 1;\nsayilar[1] = 2;\n\n// Dizi başlatma\nint[] sayilar2 = { 1, 2, 3, 4, 5 };\n\n// Çok boyutlu dizi\nint[,] matris = new int[3, 3];</code></pre><h2>List&lt;T&gt;</h2><pre><code>// List oluşturma\nList<string> isimler = new List<string>();\nisimler.Add(\"Ali\");\nisimler.Add(\"Ayşe\");\n\n// List başlatma\nvar sayilar = new List<int> { 1, 2, 3, 4, 5 };\n\n// List işlemleri\nsayilar.Remove(3);\nsayilar.Contains(2);\nsayilar.Count;</code></pre><h2>Dictionary&lt;TKey, TValue&gt;</h2><pre><code>// Dictionary oluşturma\nDictionary<string, int> yaslar = new Dictionary<string, int>();\nyaslar[\"Ali\"] = 25;\nyaslar[\"Ayşe\"] = 30;\n\n// Dictionary başlatma\nvar yaslar2 = new Dictionary<string, int>\n{\n    { \"Ali\", 25 },\n    { \"Ayşe\", 30 }\n};</code></pre><h2>HashSet ve Queue</h2><pre><code>// HashSet (benzersiz değerler)\nHashSet<int> benzersizSayilar = new HashSet<int> { 1, 2, 3, 2, 1 };\n// Sonuç: { 1, 2, 3 }\n\n// Queue (FIFO)\nQueue<string> kuyruk = new Queue<string>();\nkuyruk.Enqueue(\"İlk\");\nkuyruk.Enqueue(\"İkinci\");\nstring ilk = kuyruk.Dequeue(); // \"İlk\"</code></pre>",
          "linkUrl": null
        },
        {
          "type": 1,
          "title": "LINQ (Language Integrated Query) Temelleri",
          "description": "LINQ sorguları ve kullanım örnekleri",
          "order": 2,
          "isRequired": true,
          "content": "<h1>LINQ (Language Integrated Query)</h1><p>LINQ, C# dilinde veri sorgulama için güçlü bir araçtır.</p><h2>LINQ Method Syntax</h2><pre><code>var sayilar = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };\n\n// Filtreleme\nvar ciftSayilar = sayilar.Where(x => x % 2 == 0);\n\n// Dönüştürme\nvar kareler = sayilar.Select(x => x * x);\n\n// Sıralama\nvar sirali = sayilar.OrderBy(x => x);\nvar tersSirali = sayilar.OrderByDescending(x => x);\n\n// Toplama\nvar toplam = sayilar.Sum();\nvar ortalama = sayilar.Average();\nvar maksimum = sayilar.Max();\nvar minimum = sayilar.Min();\n\n// Gruplama\nvar gruplu = sayilar.GroupBy(x => x % 2 == 0 ? \"Çift\" : \"Tek\");</code></pre><h2>LINQ Query Syntax</h2><pre><code>var sayilar = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };\n\nvar sonuc = from sayi in sayilar\n            where sayi % 2 == 0\n            orderby sayi descending\n            select sayi * 2;</code></pre><h2>Pratik Örnek</h2><pre><code>public class Kisi\n{\n    public string Ad { get; set; }\n    public int Yas { get; set; }\n    public string Sehir { get; set; }\n}\n\nvar kisiler = new List<Kisi>\n{\n    new Kisi { Ad = \"Ali\", Yas = 25, Sehir = \"İstanbul\" },\n    new Kisi { Ad = \"Ayşe\", Yas = 30, Sehir = \"Ankara\" },\n    new Kisi { Ad = \"Mehmet\", Yas = 25, Sehir = \"İstanbul\" }\n};\n\n// İstanbul'da yaşayan 25 yaş üstü kişiler\nvar sonuc = kisiler\n    .Where(k => k.Sehir == \"İstanbul\" && k.Yas >= 25)\n    .OrderBy(k => k.Ad)\n    .Select(k => k.Ad)\n    .ToList();</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Koleksiyonlar ve LINQ Video Eğitimi",
          "description": "Koleksiyonlar ve LINQ kullanımı",
          "order": 3,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=z3Pow9pMu2I"
        },
        {
          "type": 4,
          "title": "Microsoft LINQ Dokümantasyonu",
          "description": "Resmi LINQ dokümantasyonu",
          "order": 4,
          "isRequired": false,
          "content": null,
          "linkUrl": "https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/"
        }
      ]
    },
    {
      "title": "Adım 5: Nesne Yönelimli Programlama (OOP)",
      "description": "Sınıflar, nesneler, kalıtım, polimorfizm ve encapsulation.",
      "order": 5,
      "isRequired": true,
      "estimatedDurationMinutes": 180,
      "contents": [
        {
          "type": 1,
          "title": "OOP Temelleri - Sınıflar ve Nesneler",
          "description": "Nesne yönelimli programlamanın temelleri",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Nesne Yönelimli Programlama (OOP)</h1><p>OOP, yazılım geliştirmede kod organizasyonu ve yeniden kullanılabilirlik için güçlü bir paradigmdır.</p><h2>Sınıf Tanımlama</h2><pre><code>public class Araba\n{\n    // Özellikler (Properties)\n    public string Marka { get; set; }\n    public string Model { get; set; }\n    public int Yil { get; set; }\n    \n    // Metodlar (Methods)\n    public void Calistir()\n    {\n        Console.WriteLine($\"{Marka} {Model} çalıştırıldı.\");\n    }\n    \n    public void Dur()\n    {\n        Console.WriteLine($\"{Marka} {Model} durdu.\");\n    }\n}\n\n// Kullanım\nvar araba = new Araba\n{\n    Marka = \"Toyota\",\n    Model = \"Corolla\",\n    Yil = 2023\n};\n\naraba.Calistir();</code></pre><h2>Constructor (Yapıcı Metod)</h2><pre><code>public class Kisi\n{\n    public string Ad { get; set; }\n    public int Yas { get; set; }\n    \n    // Default constructor\n    public Kisi() { }\n    \n    // Parametreli constructor\n    public Kisi(string ad, int yas)\n    {\n        Ad = ad;\n        Yas = yas;\n    }\n}\n\n// Kullanım\nvar kisi1 = new Kisi();\nvar kisi2 = new Kisi(\"Ali\", 25);</code></pre><h2>Encapsulation (Kapsülleme)</h2><pre><code>public class BankaHesabi\n{\n    private decimal bakiye; // Private field\n    \n    public decimal Bakiye\n    {\n        get { return bakiye; }\n        private set { bakiye = value; } // Sadece sınıf içinden set edilebilir\n    }\n    \n    public void ParaYatir(decimal tutar)\n    {\n        if (tutar > 0)\n        {\n            bakiye += tutar;\n        }\n    }\n    \n    public bool ParaCek(decimal tutar)\n    {\n        if (tutar > 0 && bakiye >= tutar)\n        {\n            bakiye -= tutar;\n            return true;\n        }\n        return false;\n    }\n}</code></pre>",
          "linkUrl": null
        },
        {
          "type": 1,
          "title": "Kalıtım ve Polimorfizm",
          "description": "OOP'nin ileri seviye konuları",
          "order": 2,
          "isRequired": true,
          "content": "<h1>Kalıtım ve Polimorfizm</h1><h2>Kalıtım (Inheritance)</h2><pre><code>// Base class (Temel sınıf)\npublic class Hayvan\n{\n    public string Ad { get; set; }\n    \n    public virtual void SesCikar()\n    {\n        Console.WriteLine(\"Hayvan ses çıkarıyor...\");\n    }\n}\n\n// Derived class (Türetilmiş sınıf)\npublic class Kedi : Hayvan\n{\n    public override void SesCikar()\n    {\n        Console.WriteLine(\"Miyav!\");\n    }\n}\n\npublic class Kopek : Hayvan\n{\n    public override void SesCikar()\n    {\n        Console.WriteLine(\"Hav hav!\");\n    }\n}\n\n// Kullanım\nHayvan hayvan1 = new Kedi { Ad = \"Pamuk\" };\nHayvan hayvan2 = new Kopek { Ad = \"Karabaş\" };\n\nhayvan1.SesCikar(); // \"Miyav!\"\nhayvan2.SesCikar(); // \"Hav hav!\"</code></pre><h2>Abstract Class ve Interface</h2><pre><code>// Abstract class\npublic abstract class Sekil\n{\n    public abstract double AlanHesapla();\n    public abstract double CevreHesapla();\n}\n\npublic class Dikdortgen : Sekil\n{\n    public double Genislik { get; set; }\n    public double Yukseklik { get; set; }\n    \n    public override double AlanHesapla()\n    {\n        return Genislik * Yukseklik;\n    }\n    \n    public override double CevreHesapla()\n    {\n        return 2 * (Genislik + Yukseklik);\n    }\n}\n\n// Interface\npublic interface ICalisan\n{\n    void Calis();\n    decimal MaasAl();\n}\n\npublic class Yazilimci : ICalisan\n{\n    public void Calis()\n    {\n        Console.WriteLine(\"Kod yazıyor...\");\n    }\n    \n    public decimal MaasAl()\n    {\n        return 50000;\n    }\n}</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# OOP Kapsamlı Video Eğitimi",
          "description": "Nesne yönelimli programlama detayları",
          "order": 3,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=Zq3aR2yOoX0"
        },
        {
          "type": 3,
          "title": "C# OOP Best Practices Dokümanı",
          "description": "OOP tasarım prensipleri ve best practices",
          "order": 4,
          "isRequired": false,
          "content": null,
          "linkUrl": "https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/"
        }
      ]
    },
    {
      "title": "Adım 6: Async/Await ve Asenkron Programlama",
      "description": "Modern C# asenkron programlama teknikleri.",
      "order": 6,
      "isRequired": false,
      "estimatedDurationMinutes": 80,
      "contents": [
        {
          "type": 1,
          "title": "Async/Await Temelleri",
          "description": "Asenkron programlama kavramları",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Async/Await ile Asenkron Programlama</h1><p>Async/await, C# dilinde asenkron işlemleri kolaylaştıran modern bir özelliktir.</p><h2>Async Metod Tanımlama</h2><pre><code>// Asenkron metod\npublic async Task<string> VeriGetirAsync()\n{\n    await Task.Delay(2000); // 2 saniye bekle\n    return \"Veri geldi!\";\n}\n\n// Kullanım\nvar sonuc = await VeriGetirAsync();\nConsole.WriteLine(sonuc);</code></pre><h2>Task ve Task&lt;T&gt;</h2><pre><code>// Task (değer döndürmeyen)\npublic async Task IslemYapAsync()\n{\n    await Task.Delay(1000);\n    Console.WriteLine(\"İşlem tamamlandı\");\n}\n\n// Task&lt;T&gt; (değer döndüren)\npublic async Task<int> ToplaAsync(int a, int b)\n{\n    await Task.Delay(500);\n    return a + b;\n}\n\n// Kullanım\nvar toplam = await ToplaAsync(5, 3);\nConsole.WriteLine(toplam); // 8</code></pre><h2>Paralel İşlemler</h2><pre><code>// Birden fazla asenkron işlemi paralel çalıştırma\nvar gorev1 = VeriGetirAsync();\nvar gorev2 = BaskaVeriGetirAsync();\n\nawait Task.WhenAll(gorev1, gorev2);\n\nvar sonuc1 = await gorev1;\nvar sonuc2 = await gorev2;</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Async/Await Video Eğitimi",
          "description": "Pratik örneklerle async/await",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=2moh18sh5p4"
        }
      ]
    }
  ]
}
```

**Beklenen Response:**
- Status: `201 Created`
- Response body'de roadmap bilgilerini kontrol edin.

**Not:** Bu roadmap'te 6 adım var. İlk 5 adım zorunlu, son adım (Async/Await) opsiyonel. Her adımda farklı içerik tipleri (Text, VideoLink, DocumentLink, ExternalLink) kullanıldı.

---

### 3.4 Kurs Detayını Görüntüleme (Admin)

**Endpoint:** `GET /api/v1/education/courses/{courseId}`

**Beklenen Response:**
- Status: `200 OK`
- Response'da kurs bilgileri, roadmap ve adımlar görünmelidir.

---

### 3.5 Roadmap Detayını Görüntüleme

**Endpoint:** `GET /api/v1/education/courses/{courseId}/roadmap`

**Beklenen Response:**
- Status: `200 OK`
- Tüm roadmap bilgileri, adımlar ve içerikler görünmelidir.

---

### 3.6 Roadmap Adımlarını Görüntüleme

**Endpoint:** `GET /api/v1/education/courses/{courseId}/roadmap/steps`

**Beklenen Response:**
- Status: `200 OK`
- Sadece adımlar listesi görünmelidir.

---

### 3.7 Kurs Güncelleme

**Endpoint:** `PUT /api/v1/education/courses/{courseId}`

**Request Body:**
```json
{
  "title": "C# Temelleri ve İleri Seviye Programlama - Güncellenmiş Versiyon",
  "description": "Bu kurs, C# programlama dilinin temel kavramlarından başlayarak ileri seviye konulara kadar kapsamlı bir öğrenme deneyimi sunar. Nesne yönelimli programlama, LINQ, async/await, Entity Framework gibi modern C# özelliklerini öğreneceksiniz. Kurs sonunda gerçek dünya projeleri geliştirebilecek seviyeye geleceksiniz. **YENİ:** Bu versiyonda Entity Framework Core ve ASP.NET Core konuları da eklendi!",
  "thumbnailUrl": "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=600&fit=crop"
}
```

**Beklenen Response:**
- Status: `204 No Content`

---

### 3.8 Roadmap Güncelleme

**Endpoint:** `PUT /api/v1/education/courses/{courseId}/roadmap`

**ÖNEMLİ NOTLAR:**
1. **Tüm Adımları Göndermelisiniz:** Roadmap güncelleme endpoint'i tüm adımları bekler. Sadece bir adımı güncellemek istiyorsanız bile, önce mevcut roadmap'i GET edip, sonra tüm adımları göndermelisiniz.
2. **JSON Escape:** HTML content içinde çift tırnak (`"`) karakterleri varsa, bunları `\"` şeklinde escape etmelisiniz. Swagger UI genellikle bunu otomatik yapar, ancak manuel JSON yapıştırırken dikkat edin.
3. **CourseId:** Request body'de `courseId` field'ı **GÖNDERMEYİN**. CourseId route parameter'ından alınır.

**Adım 1: Mevcut Roadmap'i GET Edin**

Önce mevcut roadmap'i alın:
```
GET /api/v1/education/courses/{courseId}/roadmap
```

Response'dan tüm adımları kopyalayın.

**Adım 2: Roadmap'i Güncelleyin**

**Request Body:**
```json
{
  "title": "C# Temelleri Yolu - Sıfırdan Uzmanlığa (Güncellenmiş)",
  "description": "Bu roadmap, C# programlama dilini sıfırdan öğrenmek isteyenler için tasarlanmıştır. Adım adım ilerleyerek temel kavramlardan başlayıp ileri seviye konulara kadar uzanacaksınız. **GÜNCELLEME:** Async/Await adımı artık zorunlu hale getirildi!",
  "estimatedDurationMinutes": 800,
  "steps": [
    {
      "title": "Adım 1: C# Programlama Dili ile Tanışma",
      "description": "C# programlama dilinin temelleri, tarihçesi ve .NET ekosistemi hakkında bilgi edinin.",
      "order": 1,
      "isRequired": true,
      "estimatedDurationMinutes": 90,
      "contents": [
        {
          "type": 1,
          "title": "C# Nedir? - Kapsamlı Giriş Yazısı",
          "description": "C# programlama diline giriş ve temel kavramlar",
          "order": 1,
          "isRequired": true,
          "content": "<h1>C# Programlama Dili Nedir?</h1><p>C# (C-Sharp), Microsoft tarafından 2000 yılında geliştirilen modern, nesne yönelimli ve tip güvenli bir programlama dilidir.</p>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Programlama Dili Tanıtım Videosu",
          "description": "C# dilinin temel özelliklerini anlatan kapsamlı video",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=GhQdlIFylQ8"
        }
      ]
    },
    {
      "title": "Adım 2: Değişkenler, Veri Tipleri ve Operatörler",
      "description": "C# dilinde değişken tanımlama, veri tipleri ve operatörler hakkında detaylı bilgi.",
      "order": 2,
      "isRequired": true,
      "estimatedDurationMinutes": 120,
      "contents": [
        {
          "type": 1,
          "title": "Değişkenler ve Veri Tipleri - Detaylı Rehber",
          "description": "C# dilinde değişken tanımlama ve veri tipleri",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Değişkenler ve Veri Tipleri</h1><p>C# dilinde değişkenler, belirli bir veri tipinde değer saklamak için kullanılır. C# güçlü bir tip sistemi kullanır, yani her değişkenin bir tipi olmalıdır.</p><h2>Değişken Tanımlama</h2><pre><code>// Değişken tanımlama\\nint sayi = 10;\\nstring metin = \\\"Merhaba\\\";\\nbool dogruMu = true;\\ndouble ondalik = 3.14;</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Değişkenler ve Veri Tipleri Eğitim Videosu",
          "description": "Pratik örneklerle değişkenler ve veri tipleri",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=YrtFtdTTfv0"
        }
      ]
    },
    {
      "title": "Adım 3: Kontrol Yapıları ve Döngüler",
      "description": "if-else, switch-case, for, while gibi kontrol yapılarını öğrenin.",
      "order": 3,
      "isRequired": true,
      "estimatedDurationMinutes": 100,
      "contents": [
        {
          "type": 1,
          "title": "Kontrol Yapıları - if, else, switch",
          "description": "Koşullu ifadeler ve kontrol yapıları",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Kontrol Yapıları</h1><p>C# dilinde program akışını kontrol etmek için çeşitli yapılar kullanılır.</p><h2>if-else Yapısı</h2><pre><code>int yas = 18;\\n\\nif (yas >= 18)\\n{\\n    Console.WriteLine(\\\"Reşit\\\");\\n}\\nelse\\n{\\n    Console.WriteLine(\\\"Reşit değil\\\");\\n}</code></pre>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Kontrol Yapıları ve Döngüler Video Eğitimi",
          "description": "Pratik örneklerle kontrol yapıları",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=IF6k0u8p7-I"
        }
      ]
    },
    {
      "title": "Adım 4: Diziler, Koleksiyonlar ve LINQ",
      "description": "Diziler, List, Dictionary gibi koleksiyonlar ve LINQ sorguları.",
      "order": 4,
      "isRequired": true,
      "estimatedDurationMinutes": 150,
      "contents": [
        {
          "type": 1,
          "title": "Diziler ve Koleksiyonlar Rehberi",
          "description": "C# koleksiyon tipleri ve kullanımları",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Diziler ve Koleksiyonlar</h1><p>C# dilinde veri gruplarını saklamak için çeşitli yapılar kullanılır.</p>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Koleksiyonlar ve LINQ Video Eğitimi",
          "description": "Koleksiyonlar ve LINQ kullanımı",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=z3Pow9pMu2I"
        }
      ]
    },
    {
      "title": "Adım 5: Nesne Yönelimli Programlama (OOP)",
      "description": "Sınıflar, nesneler, kalıtım, polimorfizm ve encapsulation.",
      "order": 5,
      "isRequired": true,
      "estimatedDurationMinutes": 180,
      "contents": [
        {
          "type": 1,
          "title": "OOP Temelleri - Sınıflar ve Nesneler",
          "description": "Nesne yönelimli programlamanın temelleri",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Nesne Yönelimli Programlama (OOP)</h1><p>OOP, yazılım geliştirmede kod organizasyonu ve yeniden kullanılabilirlik için güçlü bir paradigmdır.</p>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# OOP Kapsamlı Video Eğitimi",
          "description": "Nesne yönelimli programlama detayları",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=Zq3aR2yOoX0"
        }
      ]
    },
    {
      "title": "Adım 6: Async/Await ve Asenkron Programlama",
      "description": "Modern C# asenkron programlama teknikleri.",
      "order": 6,
      "isRequired": true,
      "estimatedDurationMinutes": 80,
      "contents": [
        {
          "type": 1,
          "title": "Async/Await Temelleri",
          "description": "Asenkron programlama kavramları",
          "order": 1,
          "isRequired": true,
          "content": "<h1>Async/Await ile Asenkron Programlama</h1><p>Async/await, C# dilinde asenkron işlemleri kolaylaştıran modern bir özelliktir.</p>",
          "linkUrl": null
        },
        {
          "type": 2,
          "title": "C# Async/Await Video Eğitimi",
          "description": "Pratik örneklerle async/await",
          "order": 2,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://www.youtube.com/watch?v=2moh18sh5p4"
        }
      ]
    }
  ]
}
```

**ÖNEMLİ:** Bu örnekte tüm 6 adım gösterildi. Gerçek testte, önce mevcut roadmap'i GET edip, sonra güncellemek istediğiniz adımları değiştirip tüm roadmap'i göndermelisiniz.

**Beklenen Response:**
- Status: `204 No Content`

---

### 3.9 Kurs Listeleme (Admin)

**Endpoint:** `GET /api/v1/education/courses?page=1&pageSize=10`

**Beklenen Response:**
- Status: `200 OK`
- Oluşturduğunuz kurs listede görünmelidir.

---

### 3.10 Kurs Arama

**Endpoint:** `GET /api/v1/education/courses?search=C#&page=1&pageSize=10`

**Beklenen Response:**
- Status: `200 OK`
- Arama sonuçlarında "C#" içeren kurslar görünmelidir.

---

**Admin test senaryosu tamamlandı. Şimdi normal kullanıcı ile devam ediyoruz.**

---

## 4. Kullanıcı Test Senaryosu

### 4.1 Normal Kullanıcı ile Giriş Yapma

**ÖNEMLİ:** Swagger'da "Authorize" butonuna tıklayın ve admin token'ını kaldırın, sonra normal kullanıcı token'ını girin.

**Endpoint:** `POST /api/v1/auth/login` (veya mevcut auth endpoint'iniz)

**Request Body:**
```json
{
  "email": "user@te4it.com",
  "password": "User123!"
}
```

**Response'dan token'ı alın ve Swagger'da "Authorize" butonuna tıklayarak yeni token'ı girin.**

---

### 4.2 Kursları Listeleme

**Endpoint:** `GET /api/v1/education/courses?page=1&pageSize=10`

**Beklenen Response:**
- Status: `200 OK`
- Admin'in oluşturduğu kurs listede görünmelidir.

---

### 4.3 Kurs Detayını Görüntüleme

**Endpoint:** `GET /api/v1/education/courses/{courseId}`

**Not:** `{courseId}` yerine admin'in oluşturduğu kurs ID'sini kullanın.

**Beklenen Response:**
- Status: `200 OK`
- Kurs bilgileri, roadmap ve adımlar görünmelidir.
- `userEnrollment` alanı `null` veya `isEnrolled: false` olmalıdır (henüz kayıt olunmadı).

---

### 4.4 Roadmap Detayını Görüntüleme

**Endpoint:** `GET /api/v1/education/courses/{courseId}/roadmap`

**Beklenen Response:**
- Status: `200 OK`
- Tüm roadmap bilgileri görünmelidir.

---

### 4.5 Kursa Kayıt Olma

**Endpoint:** `POST /api/v1/education/courses/{courseId}/enroll`

**Beklenen Response:**
- Status: `201 Created`
- Response'da enrollment bilgileri görünmelidir.
- `enrolledAt` set edilmiş olmalıdır.
- `startedAt` başlangıçta `null` olmalıdır.

**Not:** Response'dan `enrollmentId` değerini kaydedin (ilerleme takibi için gerekli olabilir).

---

### 4.6 Kurs Detayını Tekrar Görüntüleme (Kayıt Sonrası)

**Endpoint:** `GET /api/v1/education/courses/{courseId}`

**Beklenen Response:**
- Status: `200 OK`
- `userEnrollment` alanında kayıt bilgileri görünmelidir.
- `isEnrolled: true` olmalıdır.
- `enrolledAt` değeri set edilmiş olmalıdır.

---

### 4.7 Kullanıcı Kayıtlarını Listeleme

**Endpoint:** `GET /api/v1/education/enrollments?status=all`

**Beklenen Response:**
- Status: `200 OK`
- Kayıt olduğunuz kurs listede görünmelidir.

---

### 4.8 İlerleme Dashboard'unu Görüntüleme (Başlangıç)

**Endpoint:** `GET /api/v1/education/progress/dashboard`

**Beklenen Response:**
- Status: `200 OK`
- `totalCourses: 1`
- `completedCourses: 0`
- `activeCourses: 1`
- `progressPercentage: 0` (henüz içerik tamamlanmadı)

---

### 4.9 Kurs İlerlemesini Görüntüleme (Başlangıç)

**Endpoint:** `GET /api/v1/education/courses/{courseId}/progress`

**Beklenen Response:**
- Status: `200 OK`
- `progressPercentage: 0`
- `completedSteps: 0`
- `totalSteps: 6` (veya roadmap'teki toplam adım sayısı)
- Tüm adımlar `isCompleted: false` olmalıdır.

---

### 4.10 Adım 1 - İçerik 1: Text İçeriği Tamamlama

**Endpoint:** `POST /api/v1/education/contents/{contentId}/complete`

**Not:** `{contentId}` yerine Adım 1'deki ilk Text içeriğinin ID'sini kullanın. Bu ID'yi kurs detayından alabilirsiniz.

**Request Body:**
```json
{
  "courseId": "550e8400-e29b-41d4-a716-446655440000",
  "timeSpentMinutes": 15,
  "watchedPercentage": null
}
```

**Beklenen Response:**
- Status: `200 OK`
- İçerik tamamlandı olarak işaretlenmelidir.

---

### 4.11 Adım 1 - İçerik 2: Video İçeriği İlerleme Takibi

**Endpoint:** `POST /api/v1/education/contents/{contentId}/video-progress`

**Not:** `{contentId}` yerine Adım 1'deki Video içeriğinin ID'sini kullanın.

**Request Body (Video %50 izlendi):**
```json
{
  "courseId": "550e8400-e29b-41d4-a716-446655440000",
  "watchedPercentage": 50,
  "timeSpentSeconds": 300,
  "isCompleted": false
}
```

**Beklenen Response:**
- Status: `200 OK`
- Video ilerlemesi kaydedilmelidir.

---

### 4.12 Adım 1 - İçerik 2: Video İçeriği Tamamlama

**Endpoint:** `POST /api/v1/education/contents/{contentId}/video-progress`

**Request Body (Video %100 izlendi ve tamamlandı):**
```json
{
  "courseId": "550e8400-e29b-41d4-a716-446655440000",
  "watchedPercentage": 100,
  "timeSpentSeconds": 600,
  "isCompleted": true
}
```

**Beklenen Response:**
- Status: `200 OK`
- Video tamamlandı olarak işaretlenmelidir.

---

### 4.13 Adım 1 - İçerik 3: External Link İçeriği Tamamlama (Opsiyonel)

**Not:** Bu içerik opsiyonel olduğu için tamamlanmasa da adım tamamlanabilir. Ancak test için tamamlayalım.

**Endpoint:** `POST /api/v1/education/contents/{contentId}/complete`

**Request Body:**
```json
{
  "courseId": "550e8400-e29b-41d4-a716-446655440000",
  "timeSpentMinutes": 10,
  "watchedPercentage": null
}
```

**Beklenen Response:**
- Status: `200 OK`

---

### 4.14 Kurs İlerlemesini Kontrol Etme (Adım 1 Tamamlandı)

**Endpoint:** `GET /api/v1/education/courses/{courseId}/progress`

**Beklenen Response:**
- Status: `200 OK`
- Adım 1'in tüm zorunlu içerikleri tamamlandığı için `isCompleted: true` olmalıdır.
- `progressPercentage` artmış olmalıdır (yaklaşık %16-17, çünkü 6 adımdan 1'i tamamlandı).
- Adım 2 artık erişilebilir olmalıdır (kilit kalkmış olmalı).

---

### 4.15 Adım 2 - Tüm İçerikleri Tamamlama

**Adım 2'deki tüm zorunlu içerikleri tamamlayın:**

1. **Text İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 20
   }
   ```

2. **Video İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/video-progress
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "watchedPercentage": 100,
     "timeSpentSeconds": 900,
     "isCompleted": true
   }
   ```

3. **Document Link (Opsiyonel - İsteğe bağlı):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 15
   }
   ```

---

### 4.16 Adım 3 - Tüm İçerikleri Tamamlama

**Adım 3'teki tüm zorunlu içerikleri tamamlayın:**

1. **Text İçeriği 1 (Kontrol Yapıları):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 25
   }
   ```

2. **Text İçeriği 2 (Döngüler):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 20
   }
   ```

3. **Video İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/video-progress
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "watchedPercentage": 100,
     "timeSpentSeconds": 1200,
     "isCompleted": true
   }
   ```

---

### 4.17 Adım 4 - Tüm İçerikleri Tamamlama

**Adım 4'teki tüm zorunlu içerikleri tamamlayın:**

1. **Text İçeriği 1 (Diziler ve Koleksiyonlar):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 30
   }
   ```

2. **Text İçeriği 2 (LINQ):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 35
   }
   ```

3. **Video İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/video-progress
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "watchedPercentage": 100,
     "timeSpentSeconds": 1800,
     "isCompleted": true
   }
   ```

---

### 4.18 Adım 5 - Tüm İçerikleri Tamamlama

**Adım 5'teki tüm zorunlu içerikleri tamamlayın:**

1. **Text İçeriği 1 (OOP Temelleri):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 40
   }
   ```

2. **Text İçeriği 2 (Kalıtım ve Polimorfizm):**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 35
   }
   ```

3. **Video İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/video-progress
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "watchedPercentage": 100,
     "timeSpentSeconds": 2400,
     "isCompleted": true
   }
   ```

---

### 4.19 Adım 6 - Tüm İçerikleri Tamamlama (Opsiyonel Adım)

**Not:** Adım 6 opsiyonel olduğu için tamamlanmasa da kurs tamamlanabilir. Ancak test için tamamlayalım.

**Adım 6'daki tüm zorunlu içerikleri tamamlayın:**

1. **Text İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/complete
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "timeSpentMinutes": 25
   }
   ```

2. **Video İçeriği:**
   ```json
   POST /api/v1/education/contents/{contentId}/video-progress
   {
     "courseId": "550e8400-e29b-41d4-a716-446655440000",
     "watchedPercentage": 100,
     "timeSpentSeconds": 1200,
     "isCompleted": true
   }
   ```

---

### 4.20 Kurs İlerlemesini Kontrol Etme (Tüm Adımlar Tamamlandı)

**Endpoint:** `GET /api/v1/education/courses/{courseId}/progress`

**Beklenen Response:**
- Status: `200 OK`
- `progressPercentage: 100` (veya yakın bir değer)
- `completedSteps: 6` (veya tüm zorunlu adımlar)
- `totalSteps: 6`
- Tüm zorunlu adımlar `isCompleted: true` olmalıdır.
- `timeSpentMinutes` toplam süre görünmelidir.

---

### 4.21 Kurs Detayını Kontrol Etme (Kurs Tamamlandı)

**Endpoint:** `GET /api/v1/education/courses/{courseId}`

**Beklenen Response:**
- Status: `200 OK`
- `userEnrollment.completedAt` set edilmiş olmalıdır (kurs tamamlandığında).
- `userEnrollment.progressPercentage: 100` olmalıdır.

---

### 4.22 İlerleme Dashboard'unu Görüntüleme (Kurs Tamamlandı)

**Endpoint:** `GET /api/v1/education/progress/dashboard`

**Beklenen Response:**
- Status: `200 OK`
- `totalCourses: 1`
- `completedCourses: 1`
- `activeCourses: 0`
- `totalTimeSpentMinutes` toplam süre görünmelidir.
- Kurslar listesinde kurs `status: "completed"` olarak görünmelidir.

---

### 4.23 Kullanıcı Kayıtlarını Listeleme (Tamamlanan Kurs)

**Endpoint:** `GET /api/v1/education/enrollments?status=completed`

**Beklenen Response:**
- Status: `200 OK`
- Tamamlanan kurs listede görünmelidir.
- `completedAt` değeri set edilmiş olmalıdır.

---

### 4.24 Kurs Listeleme (Tekrar)

**Endpoint:** `GET /api/v1/education/courses?page=1&pageSize=10`

**Beklenen Response:**
- Status: `200 OK`
- Kurs hala listede görünmelidir (soft delete yapılmadığı sürece).

---

## 5. Test Verileri Referansı

### 5.1 Gerçek Video Linkleri

- **C# Tanıtım Videosu:** `https://www.youtube.com/watch?v=GhQdlIFylQ8`
- **C# Değişkenler ve Veri Tipleri:** `https://www.youtube.com/watch?v=YrtFtdTTfv0`
- **C# Kontrol Yapıları:** `https://www.youtube.com/watch?v=IF6k0u8p7-I`
- **C# Koleksiyonlar ve LINQ:** `https://www.youtube.com/watch?v=z3Pow9pMu2I`
- **C# OOP:** `https://www.youtube.com/watch?v=Zq3aR2yOoX0`
- **C# Async/Await:** `https://www.youtube.com/watch?v=2moh18sh5p4`

### 5.2 Gerçek Görsel Linkleri

- **Kurs Thumbnail 1:** `https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=800&h=600&fit=crop`
- **Kurs Thumbnail 2:** `https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=600&fit=crop`

### 5.3 Gerçek Doküman Linkleri

- **Microsoft C# Dokümantasyonu:** `https://learn.microsoft.com/dotnet/csharp/`
- **Microsoft LINQ Dokümantasyonu:** `https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/`
- **Microsoft OOP Dokümantasyonu:** `https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/`
- **C# Veri Tipleri:** `https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types`

### 5.4 İçerik Tipleri (ContentType Enum)

- `1` = Text (Zengin metin içeriği)
- `2` = VideoLink (Video linki)
- `3` = DocumentLink (Doküman linki)
- `4` = ExternalLink (Dış link)

### 5.5 Test Senaryosu Özeti

**Admin İşlemleri:**
1. ✅ Kurs oluşturma
2. ✅ Roadmap oluşturma (6 adım, çeşitli içerik tipleri)
3. ✅ Kurs detay görüntüleme
4. ✅ Roadmap detay görüntüleme
5. ✅ Kurs güncelleme
6. ✅ Roadmap güncelleme
7. ✅ Kurs listeleme ve arama

**Kullanıcı İşlemleri:**
1. ✅ Kurs listeleme
2. ✅ Kurs detay görüntüleme
3. ✅ Kursa kayıt olma
4. ✅ İçerik tamamlama (Text, Video, Document, External Link)
5. ✅ Video ilerleme takibi
6. ✅ İlerleme görüntüleme
7. ✅ Dashboard görüntüleme
8. ✅ Tüm adımları tamamlama
9. ✅ Kurs tamamlama kontrolü

---

## 6. Hata Senaryoları (Opsiyonel - Test Edilebilir)

### 6.1 Yetkisiz Erişim

- Normal kullanıcı ile kurs oluşturma denemesi → `403 Forbidden` beklenir
- Normal kullanıcı ile roadmap oluşturma denemesi → `403 Forbidden` beklenir

### 6.2 Geçersiz Veri

- Geçersiz kurs ID ile detay görüntüleme → `404 Not Found` beklenir
- Geçersiz içerik ID ile tamamlama → `404 Not Found` beklenir

### 6.3 Tekrar Kayıt

- Aynı kursa tekrar kayıt olma denemesi → `400 Bad Request` veya uygun hata mesajı beklenir

### 6.4 Kilitli Adım Erişimi

- Önceki adım tamamlanmadan sonraki adıma erişim → İş kurallarına göre kontrol edilir

---

## 7. Test Sonuçları

Test tamamlandıktan sonra aşağıdaki kontrol listesini doldurun:

### Admin Testleri
- [ ] Kurs oluşturma başarılı
- [ ] Roadmap oluşturma başarılı
- [ ] Kurs güncelleme başarılı
- [ ] Roadmap güncelleme başarılı
- [ ] Kurs listeleme çalışıyor
- [ ] Kurs arama çalışıyor

### Kullanıcı Testleri
- [ ] Kurs listeleme çalışıyor
- [ ] Kurs detay görüntüleme çalışıyor
- [ ] Kursa kayıt olma başarılı
- [ ] Text içerik tamamlama çalışıyor
- [ ] Video içerik tamamlama çalışıyor
- [ ] Video ilerleme takibi çalışıyor
- [ ] İlerleme görüntüleme çalışıyor
- [ ] Dashboard görüntüleme çalışıyor
- [ ] Tüm adımlar tamamlandı
- [ ] Kurs tamamlandı olarak işaretlendi

---

**Test Dokümanı Versiyonu:** 1.0  
**Son Güncelleme:** 2025-01-27  
**Hazırlayan:** TE4IT Development Team

