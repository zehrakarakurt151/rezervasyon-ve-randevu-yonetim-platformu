@echo off
echo ========================================
echo   Rezervasyon ve Randevu Yonetim Platformu
echo ========================================
echo.

echo [1/2] Backend baslatiliyor...
start "Backend API" cmd /k "cd /d %~dp0backend && dotnet run"

timeout /t 3 /nobreak > nul

echo [2/2] Frontend baslatiliyor...
start "Frontend Angular" cmd /k "cd /d %~dp0frontend && npm start"

echo.
echo ========================================
echo   Her iki sunucu da baslatildi!
echo ========================================
echo.
echo Erisim Adresleri:
echo   Frontend:  http://localhost:4200
echo   Backend:   http://localhost:5160
echo   Swagger:   http://localhost:5160/swagger
echo.
echo Durdurmak icin acilan terminal pencerelerini kapatin.
pause
