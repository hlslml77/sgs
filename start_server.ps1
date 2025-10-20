# 三国策略游戏服务器启动脚本 (PowerShell)
# 设置编码为 UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "三国策略游戏服务器启动脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Redis 是否运行
Write-Host "[1/4] 检查 Redis 服务..." -ForegroundColor Yellow
$redisProcess = Get-Process -Name "redis-server" -ErrorAction SilentlyContinue
if ($redisProcess) {
    Write-Host "✓ Redis 服务正在运行" -ForegroundColor Green
} else {
    Write-Host "✗ Redis 服务未运行，正在启动..." -ForegroundColor Red
    try {
        Start-Process "redis-server" -WindowStyle Minimized
        Start-Sleep -Seconds 2
        Write-Host "✓ Redis 服务已启动" -ForegroundColor Green
    } catch {
        Write-Host "✗ 无法启动 Redis，请手动启动或检查是否已安装" -ForegroundColor Red
    }
}
Write-Host ""

# 检查 MySQL 是否运行
Write-Host "[2/4] 检查 MySQL 服务..." -ForegroundColor Yellow
$mysqlService = Get-Service | Where-Object {($_.Name -like '*mysql*' -or $_.DisplayName -like '*mysql*') -and $_.Status -eq 'Running'}
if ($mysqlService) {
    Write-Host "✓ MySQL 服务正在运行: $($mysqlService.Name)" -ForegroundColor Green
} else {
    Write-Host "✗ MySQL 服务未运行，请先启动 MySQL" -ForegroundColor Red
    $allMysqlServices = Get-Service | Where-Object {$_.Name -like '*mysql*' -or $_.DisplayName -like '*mysql*'}
    if ($allMysqlServices) {
        Write-Host "可用的 MySQL 服务: $($allMysqlServices.Name -join ', ')" -ForegroundColor Yellow
        Write-Host "可以运行: net start $($allMysqlServices[0].Name)" -ForegroundColor Yellow
    } else {
        Write-Host "未找到 MySQL 服务，请检查 MySQL 是否已安装" -ForegroundColor Red
    }
    Read-Host "按任意键退出"
    exit 1
}
Write-Host ""

# 进入服务器目录
Write-Host "[3/4] 切换到服务器目录..." -ForegroundColor Yellow
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$scriptPath\server"

if (-not (Test-Path "cmd\server\main.go")) {
    Write-Host "✗ 找不到服务器主文件" -ForegroundColor Red
    Read-Host "按任意键退出"
    exit 1
}
Write-Host "✓ 服务器目录确认成功" -ForegroundColor Green
Write-Host ""

# 检查配置文件
if (-not (Test-Path "config\config.yaml")) {
    Write-Host "⚠ 配置文件不存在，从示例文件创建..." -ForegroundColor Yellow
    if (Test-Path "config\config.example.yaml") {
        Copy-Item "config\config.example.yaml" "config\config.yaml"
        Write-Host "✓ 配置文件已创建，请检查配置" -ForegroundColor Green
    }
}

# 启动服务器
Write-Host "[4/4] 启动游戏服务器..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    go run cmd/server/main.go
} catch {
    Write-Host ""
    Write-Host "✗ 服务器启动失败: $_" -ForegroundColor Red
}

# 如果服务器异常退出
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "服务器已停止" -ForegroundColor Yellow
Read-Host "按任意键退出"

