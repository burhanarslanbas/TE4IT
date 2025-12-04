---
trigger: glob
glob: *.gradle, *.gradle.kts
---
# 1. Android Proje Konfigürasyonu

Bu projedeki tüm Android mobil kodları, aşağıdaki SDK ve dil versiyonlarına sıkı sıkıya bağlı kalmalıdır. Bu kurallar `mobile/build.gradle.kts` (veya `.gradle`) dosyası için geçerlidir.

- **Compile Sdk:** 35
- **Min Sdk:** 24
- **Target Sdk:** 35
- **Java Versiyonu:** 11 (`JavaVersion.VERSION_11`)
- **Kotlin JVM Target:** "11"

