@echo off
chcp 65001 >nul
echo ========================================
echo 三国策略游戏服务器启动脚本 (开发模式)
echo ========================================
echo.

REM 进入服务器目录
cd /d "%~dp0server"

REM 设置开发环境变量
set GIN_MODE=debug
set GO_ENV=development

REM 启动服务器 (带自动重载)
echo 启动游戏服务器 (开发模式)...
echo 提示: 修改代码后会自动重新编译
echo ========================================
echo.

REM 检查是否安装了 air (热重载工具)
where air >nul 2>&1
if %errorlevel% == 0 (
    echo 使用 Air 进行热重载...
    air
) else (
    echo Air 未安装，使用普通模式启动
    echo 提示: 安装 Air 可以实现代码热重载
    echo 运行: go install github.com/cosmtrek/air@latest
    echo.
    go run cmd/server/main.go
)

echo.
echo ========================================
echo 服务器已停止
pause

