# 📅 Rezervasyon ve Randevu Yönetim Platformu

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-19-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white)

**Modern, ölçeklenebilir ve kullanıcı dostu bir randevu yönetim sistemi**

[Özellikler](#-özellikler) • [Kurulum](#-kurulum) • [Kullanım](#-kullanım) • [API Dokümantasyonu](#-api-dokümantasyonu) • [Mimari](#️-mimari)

</div>

---

## 📖 Proje Hakkında

Bu proje, hizmet sağlayıcıları (berber, kuaför, doktor, danışman vb.) ile müşterileri dijital ortamda bir araya getiren kapsamlı bir **Rezervasyon ve Randevu Yönetim Platformu**dur.

### 🎯 Projenin Amacı

- Hizmet sektöründeki randevu karmaşasını ortadan kaldırmak
- Müşterilere 7/24 online randevu alma imkanı sunmak
- Hizmet sağlayıcılara iş planlaması ve müşteri yönetimi kolaylığı sağlamak
- Telefon trafiğini azaltarak operasyonel verimliliği artırmak

### 👥 Hedef Kitle

| Kullanıcı Tipi | Kullanım Senaryosu |
|----------------|-------------------|
| **Müşteriler** | Hizmet arama, randevu oluşturma, geçmiş randevuları görüntüleme |
| **Hizmet Sağlayıcılar** | Hizmet tanımlama, randevu yönetimi, müşteri takibi |

---

## ✨ Özellikler

### 🔐 Kimlik Doğrulama ve Yetkilendirme
- JWT (JSON Web Token) tabanlı güvenli kimlik doğrulama
- Rol bazlı erişim kontrolü (Müşteri / Hizmet Sağlayıcı)
- Şifre hash'leme ile güvenli depolama
- Token yenileme mekanizması

### 👤 Müşteri Paneli
- ✅ Hizmet sağlayıcı listeleme ve arama
- ✅ Dinamik müsait saat görüntüleme
- ✅ Adım adım randevu oluşturma (Wizard)
- ✅ Aktif ve geçmiş randevuları görüntüleme
- ✅ Randevu iptal etme

### 💼 Hizmet Sağlayıcı Paneli
- ✅ Hizmet ekleme, düzenleme ve silme
- ✅ Fiyat ve süre belirleme
- ✅ Gelen randevuları görüntüleme ve yönetme
- ✅ Randevu onaylama, tamamlama, iptal etme
- ✅ İstatistik dashboard'u

### 📧 Bildirim Sistemi
- Kayıt sonrası hoş geldin e-postası
- Randevu oluşturulduğunda hem müşteriye hem sağlayıcıya bildirim
- Randevu iptalinde bilgilendirme e-postası
- HTML formatında profesyonel e-posta şablonları

### 🎨 Kullanıcı Deneyimi
- Modern ve responsive tasarım
- Angular Material ile tutarlı UI bileşenleri
- Türkçe dil desteği
- Telefon numarası formatlaması (0XXX XXX XX XX)
- Form validasyonları ve kullanıcı geri bildirimleri

---

## 🛠️ Teknoloji Yığını

### Backend
| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| .NET | 8.0 | Modern, yüksek performanslı framework |
| ASP.NET Core | 8.0 | RESTful Web API geliştirme |
| Entity Framework Core | 8.0 | ORM - Veritabanı işlemleri |
| SQL Server LocalDB | - | Visual Studio ile gelen hafif SQL Server |
| JWT Bearer | - | Token tabanlı kimlik doğrulama |

### Frontend
| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| Angular | 19.0 | Component tabanlı SPA framework |
| Angular Material | 19.0 | Google Material Design UI kiti |
| TypeScript | 5.6 | Tip güvenli JavaScript |
| RxJS | 7.x | Reaktif programlama |

---

## 📦 Kurulum

### Ön Gereksinimler

```bash
# Node.js (18.x veya üzeri)
node --version

# .NET SDK (8.0)
dotnet --version

# Angular CLI
npm install -g @angular/cli
```

### Adım 1: Projeyi Klonlayın

```bash
git clone https://github.com/utkumertgecgel/rezervasyon-ve-randevu-yonetim-platformu.git
cd rezervasyon-ve-randevu-yonetim-platformu
```

### Adım 2: Backend'i Başlatın

```bash
cd backend
dotnet restore
dotnet run
```

Backend varsayılan olarak `http://localhost:5160` adresinde çalışır.

### Adım 3: Frontend'i Başlatın

Yeni bir terminal açın:

```bash
cd frontend
npm install
ng serve
```

Frontend `http://localhost:4200` adresinde çalışır.

### Adım 4: Uygulamayı Açın

Tarayıcınızda `http://localhost:4200` adresine gidin.

---

## 🚀 Kullanım

### Test Kullanıcıları

Sistemi test etmek için aşağıdaki hazır kullanıcıları kullanabilirsiniz:

| Rol | E-posta | Şifre |
|-----|---------|-------|
| Müşteri | ahmet.test@gmail.com | Test123! |
| Hizmet Sağlayıcı | mehmet.provider@gmail.com | Test123! |

### Yeni Hesap Oluşturma

1. Ana sayfada "Kayıt Ol" butonuna tıklayın
2. Müşteri veya Hizmet Sağlayıcı rolünü seçin
3. Gerekli bilgileri doldurun
4. Hesabınız otomatik olarak aktif olacaktır

### Randevu Alma (Müşteri)

1. Giriş yapın
2. "Hizmet Ara" butonuna tıklayın
3. Hizmet sağlayıcı seçin → İleri
4. Hizmet seçin → İleri
5. Tarih ve saat seçin → İleri
6. Randevuyu onaylayın

### Hizmet Ekleme (Sağlayıcı)

1. Sağlayıcı olarak giriş yapın
2. Sol menüden "Hizmetlerim"e tıklayın
3. "Yeni Hizmet Ekle" butonuna tıklayın
4. Hizmet adı, açıklama, fiyat ve süre girin
5. Kaydedin

---

## 📡 API Dokümantasyonu

### Kimlik Doğrulama Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | `/api/auth/register` | Yeni kullanıcı kaydı |
| POST | `/api/auth/login` | Kullanıcı girişi |

### Randevu Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/appointments/my` | Kullanıcının randevuları |
| POST | `/api/appointments` | Yeni randevu oluştur |
| PUT | `/api/appointments/{id}/cancel` | Randevu iptal et |
| PUT | `/api/appointments/{id}/complete` | Randevu tamamla |

### Hizmet Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/services` | Tüm hizmetler |
| GET | `/api/services/my` | Sağlayıcının hizmetleri |
| POST | `/api/services` | Yeni hizmet ekle |
| PUT | `/api/services/{id}` | Hizmet güncelle |
| DELETE | `/api/services/{id}` | Hizmet sil |

### Müsaitlik Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/availability/slots` | Müsait saatleri getir |

---

## 🏗️ Mimari

### Proje Yapısı

```
rezervasyon-ve-randevu-yonetim-platformu/
│
├── backend/                          # .NET Web API
│   ├── Controllers/                  # API Controller'ları
│   │   ├── AuthController.cs         # Kimlik doğrulama
│   │   ├── AppointmentsController.cs # Randevu işlemleri
│   │   ├── ServicesController.cs     # Hizmet yönetimi
│   │   ├── AvailabilityController.cs # Müsaitlik hesaplama
│   │   └── ProvidersController.cs    # Sağlayıcı listeleme
│   │
│   ├── Models/                       # Veritabanı modelleri
│   │   ├── ApplicationUser.cs        # Kullanıcı modeli
│   │   ├── Appointment.cs            # Randevu modeli
│   │   ├── Service.cs                # Hizmet modeli
│   │   └── WorkSchedule.cs           # Çalışma takvimi
│   │
│   ├── Services/                     # İş mantığı servisleri
│   │   └── EmailService.cs           # E-posta gönderimi
│   │
│   ├── Data/                         # Veritabanı context
│   │   └── AppDbContext.cs
│   │
│   └── appsettings.json              # Konfigürasyon
│
├── frontend/                         # Angular Uygulaması
│   └── src/
│       └── app/
│           ├── features/             # Sayfa bileşenleri
│           │   ├── auth/             # Giriş/Kayıt sayfaları
│           │   ├── dashboard/        # Panel sayfaları
│           │   └── appointment/      # Randevu wizard
│           │
│           ├── services/             # API servisleri
│           │   ├── auth.service.ts
│           │   ├── appointment.service.ts
│           │   └── provider.service.ts
│           │
│           └── shared/               # Ortak bileşenler
│               └── directives/       # Direktifler
│
├── README.md                         # Bu dosya
└── .gitignore                        # Git ignore kuralları
```

### Veritabanı Şeması

```
┌─────────────────┐       ┌─────────────────┐
│  ApplicationUser│       │     Service     │
├─────────────────┤       ├─────────────────┤
│ Id              │       │ Id              │
│ Email           │       │ Name            │
│ PasswordHash    │       │ Description     │
│ FirstName       │       │ Price           │
│ LastName        │       │ DurationMinutes │
│ PhoneNumber     │◄──────│ ServiceProviderId│
│ Role            │       └─────────────────┘
│ BusinessName    │
│ Address         │       ┌─────────────────┐
└─────────────────┘       │   Appointment   │
        │                 ├─────────────────┤
        │                 │ Id              │
        │                 │ StartTime       │
        │                 │ EndTime         │
        └────────────────►│ Status          │
                          │ CustomerId      │
                          │ ServiceProviderId│
                          │ ServiceId       │
                          └─────────────────┘
```

---

## 🔒 Güvenlik

- **Şifre Güvenliği**: BCrypt ile hash'leme
- **JWT Token**: Stateless authentication
- **CORS**: Cross-Origin güvenliği
- **Yetki Kontrolü**: `[Authorize]` attribute'ları
- **Input Validasyonu**: Model validasyon kuralları

---

## 🧪 Test

### Manuel Test Senaryoları

1. **Müşteri Kayıt Testi**
   - Geçersiz e-posta ile kayıt dene → Hata mesajı görünmeli
   - Kısa şifre ile kayıt dene → Hata mesajı görünmeli
   - Geçerli bilgilerle kayıt → Başarılı, giriş sayfasına yönlendir

2. **Randevu Oluşturma Testi**
   - Aynı saatte iki randevu almayı dene → İkinci randevu için saat görünmemeli
   - Geçmiş tarih seçmeyi dene → Seçilememeli
   - Geçerli randevu oluştur → Başarı mesajı, dashboard'da görünmeli

3. **Hizmet Yönetimi Testi**
   - Eksik bilgilerle hizmet eklemeyi dene → Kaydet butonu disabled
   - Geçerli hizmet ekle → Listede görünmeli
   - Hizmet sil → Listeden kalkmalı

---

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır.

---

## 👨‍💻 Geliştirici

**Bitirme Tezi** - 2025/2026 Akademik Yılı

---

<div align="center">



</div>
