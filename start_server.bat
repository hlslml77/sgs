@echo off
chcp 65001 >nul
echo ========================================
echo Sanguo Strategy Game Server Launcher
echo ========================================
echo.

REM Check Redis
echo [1/4] Checking Redis service...
tasklist /FI "IMAGENAME eq redis-server.exe" 2>NUL | find /I /N "redis-server.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [OK] Redis is running
) else (
    echo [WARN] Redis not running, starting...
    start "Redis Server" redis-server
    timeout /t 2 /nobreak >nul
    echo [OK] Redis started
)
echo.

REM Check MySQL
echo [2/4] Checking MySQL service...
sc query MySQL92 2>nul | find "RUNNING" >nul
if %errorlevel% == 0 (
    echo [OK] MySQL is running
) else (
    sc query MySQL 2>nul | find "RUNNING" >nul
    if %errorlevel% == 0 (
        echo [OK] MySQL is running
    ) else (
        echo [ERROR] MySQL service not running
        echo Hint: Run 'net start MySQL92' or 'net start MySQL'
        pause
        exit /b 1
    )
)
echo.

REM Change to server directory
echo [3/4] Changing to server directory...
cd /d "%~dp0server"
if not exist "cmd\server\main.go" (
    echo [ERROR] Server main file not found
    pause
    exit /b 1
)
echo [OK] Server directory confirmed
echo.

REM Start server
echo [4/4] Starting game server...
echo ========================================
echo.
go run cmd/server/main.go

REM If server exits
echo.
echo ========================================
echo Server stopped
pause
