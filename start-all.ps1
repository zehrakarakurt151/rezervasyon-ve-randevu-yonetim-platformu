# Rezervasyon ve Randevu Yonetim Platformu - Baslat Script
# Bu script hem backend hem frontend'i ayni anda baslatir

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Rezervasyon ve Randevu Yonetim Platformu" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Proje dizinini al
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# Backend'i baslat
Write-Host "[1/2] Backend baslatiliyor..." -ForegroundColor Yellow
$backendPath = Join-Path $ProjectRoot "backend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$backendPath'; Write-Host 'Backend (API) Sunucusu' -ForegroundColor Green; dotnet run" -WindowStyle Normal

# Biraz bekle
Start-Sleep -Seconds 3

# Frontend'i baslat
Write-Host "[2/2] Frontend baslatiliyor..." -ForegroundColor Yellow
$frontendPath = Join-Path $ProjectRoot "frontend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; Write-Host 'Frontend (Angular) Sunucusu' -ForegroundColor Green; npm start" -WindowStyle Normal

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Her iki sunucu da baslatildi!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Erisim Adresleri:" -ForegroundColor White
Write-Host "  Frontend:  http://localhost:4200" -ForegroundColor Cyan
Write-Host "  Backend:   http://localhost:5160" -ForegroundColor Cyan
Write-Host "  Swagger:   http://localhost:5160/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Durdurmak icin acilan terminal pencerelerini kapatin." -ForegroundColor Gray
