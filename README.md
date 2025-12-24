# 📅 Rezervasyon ve Randevu Yönetim Platformu

Hizmet sağlayıcıları ve müşterileri bir araya getiren modern bir randevu yönetim sistemi.

## 🚀 Özellikler

### Müşteri Paneli
- ✅ Hizmet sağlayıcı arama ve seçme
- ✅ Dinamik müsait saat görüntüleme
- ✅ Kolay randevu oluşturma (wizard)
- ✅ Randevu görüntüleme ve iptal etme

### Hizmet Sağlayıcı Paneli  
- ✅ Hizmet ekleme, düzenleme, silme
- ✅ Gelen randevuları yönetme
- ✅ Randevu onaylama/tamamlama/iptal

### Genel Özellikler
- ✅ JWT tabanlı kimlik doğrulama
- ✅ Rol bazlı yetkilendirme (Müşteri/Provider)
- ✅ E-posta bildirimleri (kayıt, randevu, iptal)
- ✅ Responsive tasarım (Angular Material)

## 🛠️ Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Frontend | Angular 19, Angular Material, TypeScript |
| Backend | .NET 8, ASP.NET Core Web API |
| Veritabanı | SQLite + Entity Framework Core |
| Kimlik Doğrulama | JWT Bearer Token |

## 📦 Kurulum

### Gereksinimler
- Node.js 18+
- .NET 8 SDK
- Angular CLI

### Backend
```bash
cd backend
dotnet restore
dotnet run
```
Backend: http://localhost:5160

### Frontend
```bash
cd frontend
npm install
ng serve
```
Frontend: http://localhost:4200

## 🧪 Test Kullanıcıları

| Rol | E-posta | Şifre |
|-----|---------|-------|
| Müşteri | ahmet.test@gmail.com | Test123! |
| Provider | mehmet.provider@gmail.com | Test123! |

## 📁 Proje Yapısı

```
├── backend/
│   ├── Controllers/     # API endpoint'leri
│   ├── Models/          # Veritabanı modelleri
│   ├── Services/        # İş mantığı (Email vb.)
│   └── Data/            # DbContext
│
└── frontend/
    └── src/app/
        ├── features/    # Sayfa bileşenleri
        ├── services/    # API servisleri
        └── shared/      # Ortak bileşenler
```

## 📸 Ekran Görüntüleri

### Giriş Sayfası
Modern ve kullanıcı dostu arayüz

### Müşteri Dashboard
Randevuları görüntüleme ve yeni randevu oluşturma

### Provider Dashboard
Hizmet yönetimi ve gelen randevuları takip etme

## 👨‍💻 Geliştirici

Bitirme Projesi - 2024/2025

## 📄 Lisans

MIT License
