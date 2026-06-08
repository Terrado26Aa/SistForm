@echo off
setlocal enabledelayedexpansion

set ROOT_DIR=%~dp0..

if "%1"=="" goto usage

if "%1"=="backend" goto backend
if "%1"=="android" goto android
if "%1"=="ios" goto ios
if "%1"=="windows" goto windows
if "%1"=="maccatalyst" goto maccatalyst
if "%1"=="linux-gtk4" goto linuxgtk4
if "%1"=="docker" goto docker
if "%1"=="stop" goto stop

goto usage

:backend
echo === Iniciando SistForm-API ===
cd /d "%ROOT_DIR%\SistForm-API"
start /B dotnet run
echo Backend iniciado.
wait
goto :eof

:android
call :backend
echo === Iniciando frontend MAUI (Android) ===
cd /d "%ROOT_DIR%\Forms"
dotnet build -f net10.0-android -t:Run -c Release
goto :eof

:ios
call :backend
echo === Iniciando frontend MAUI (iOS) ===
cd /d "%ROOT_DIR%\Forms"
dotnet build -f net10.0-ios -t:Run -c Release
goto :eof

:windows
call :backend
echo === Iniciando frontend MAUI (Windows) ===
cd /d "%ROOT_DIR%\Forms"
dotnet build -f net10.0-windows10.0.19041.0 -t:Run -c Release
goto :eof

:maccatalyst
call :backend
echo === Iniciando frontend MAUI (macCatalyst) ===
cd /d "%ROOT_DIR%\Forms"
dotnet run -f net10.0-maccatalyst -c Release
goto :eof

:linuxgtk4
echo Linux GTK4 no es compatible en Windows.
echo Ejecuta en Linux: ./script/start.sh linux-gtk4
goto :eof

:docker
echo === Iniciando contenedores Docker ===
cd /d "%ROOT_DIR%\script"
docker compose up -d
echo Docker compose iniciado.
echo   MySQL:  localhost:3307
echo   API:    http://localhost:5174
goto :eof

:stop
echo === Deteniendo contenedores Docker ===
cd /d "%ROOT_DIR%\script"
docker compose down
echo Contenedores detenidos.
goto :eof

:usage
echo Uso: %~nx0 {backend^|android^|ios^|windows^|maccatalyst^|docker^|stop}
echo.
echo Ejemplos:
echo   %~nx0 backend          Solo inicia la API
echo   %~nx0 android          API + app Android
echo   %~nx0 windows          API + app Windows
echo   %~nx0 docker           Inicia MySQL + API en Docker
echo   %~nx0 stop             Detiene Docker
goto :eof
