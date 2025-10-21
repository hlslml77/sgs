@echo off
chcp 65001 >nul
echo ========================================
echo Client Environment Check
echo ========================================
echo.

REM Check if Unity Hub is installed
echo [1/5] Checking Unity Hub...
where unity-hub.exe >nul 2>&1
if %errorlevel% == 0 (
    echo [OK] Unity Hub found
) else (
    echo [WARN] Unity Hub not found in PATH
    echo Checking common installation paths...
    if exist "C:\Program Files\Unity Hub\Unity Hub.exe" (
        echo [OK] Unity Hub found at: C:\Program Files\Unity Hub\Unity Hub.exe
    ) else (
        echo [ERROR] Unity Hub not installed
        echo Please download from: https://unity.com/download
    )
)
echo.

REM Check if Unity is installed
echo [2/5] Checking Unity Editor...
set UNITY_FOUND=0
for /d %%i in ("C:\Program Files\Unity\Hub\Editor\*") do (
    echo [OK] Found Unity version: %%~nxi
    set UNITY_FOUND=1
)
if %UNITY_FOUND% == 0 (
    echo [WARN] No Unity Editor found
    echo Please install Unity 2021.3 LTS or higher via Unity Hub
)
echo.

REM Check if client directory exists
echo [3/5] Checking client directory...
if exist "%~dp0client" (
    echo [OK] Client directory found: %~dp0client
    cd /d "%~dp0client"
    
    REM Check for essential files
    if exist "Assets" (
        echo [OK] Assets folder exists
    ) else (
        echo [ERROR] Assets folder not found
    )
    
    if exist "ProjectSettings" (
        echo [OK] ProjectSettings folder exists
    ) else (
        echo [ERROR] ProjectSettings folder not found
    )
) else (
    echo [ERROR] Client directory not found
    echo Expected at: %~dp0client
)
echo.

REM Check server status
echo [4/5] Checking server status...
curl -s http://localhost:8080/health >nul 2>&1
if %errorlevel% == 0 (
    echo [OK] Server is running
) else (
    echo [WARN] Server not responding
    echo Please start server first: start_server.bat
)
echo.

REM Check dependencies
echo [5/5] Checking dependencies...
sc query MySQL92 2>nul | find "RUNNING" >nul
if %errorlevel% == 0 (
    echo [OK] MySQL is running
) else (
    echo [WARN] MySQL not running
)

tasklist /FI "IMAGENAME eq redis-server.exe" 2>NUL | find /I /N "redis-server.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [OK] Redis is running
) else (
    echo [WARN] Redis not running
)
echo.

echo ========================================
echo Check Complete
echo ========================================
echo.
echo Next Steps:
echo 1. If Unity Hub is installed, open it
echo 2. Add project: %~dp0client
echo 3. Open project with Unity 2021.3 LTS+
echo 4. Open scene: Assets/Scenes/MainMenu.unity
echo 5. Click Play button
echo.
echo For detailed instructions, see: START_CLIENT.md
echo.
pause

