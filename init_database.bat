@echo off
chcp 65001 >nul
echo ========================================
echo 三国策略游戏 - 数据库初始化脚本
echo ========================================
echo.

REM 检查MySQL是否安装
where mysql >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ 错误: 未找到MySQL命令！
    echo 请确保MySQL已安装并添加到系统PATH中。
    echo.
    pause
    exit /b 1
)

echo ✓ 已找到MySQL
echo.

REM 从配置文件读取数据库信息
set DB_HOST=localhost
set DB_PORT=3306
set DB_USER=root
set DB_NAME=sanguo_strategy

REM 提示输入密码
set /p DB_PASSWORD=请输入MySQL root密码: 

echo.
echo 警告：此操作将删除现有数据库并重新创建！
echo.
set /p CONFIRM=确认要继续吗？(Y/N): 

if /i not "%CONFIRM%"=="Y" (
    echo.
    echo 操作已取消。
    echo.
    pause
    exit /b 0
)

echo.
echo 正在初始化数据库...
echo.

REM 执行SQL脚本
mysql -h%DB_HOST% -P%DB_PORT% -u%DB_USER% -p%DB_PASSWORD% < server/scripts/init_database.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo 数据库初始化成功！
    echo ========================================
    echo.
    echo 数据库名称: %DB_NAME%
    echo 主机地址: %DB_HOST%:%DB_PORT%
    echo.
    echo 已创建的表:
    echo   - players       ^(玩家表^)
    echo   - generals      ^(武将表^)
    echo   - terrains      ^(地形表^)
    echo   - rooms         ^(房间表^)
    echo   - game_histories^(游戏历史^)
    echo   - player_stats  ^(玩家统计^)
    echo.
    echo 测试账号:
    echo   用户名: test
    echo   密码: test123
    echo.
    echo 下一步: 运行 start_server.bat 启动服务器
    echo.
) else (
    echo.
    echo ========================================
    echo 数据库初始化失败！
    echo ========================================
    echo.
    echo 可能的原因:
    echo   1. MySQL密码错误
    echo   2. MySQL服务未启动
    echo   3. 权限不足或数据已存在
    echo.
    echo 如需重置数据库，请先删除现有数据库。
    echo 请检查错误信息后重试。
    echo.
)

pause

