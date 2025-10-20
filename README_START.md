# 三国策略游戏 - 服务器启动指南

## 快速启动

### Windows 环境

#### 方式一：使用批处理脚本 (推荐)
双击运行 `start_server.bat` 文件

#### 方式二：使用 PowerShell 脚本
```powershell
.\start_server.ps1
```

#### 方式三：开发模式 (带热重载)
双击运行 `start_server_dev.bat` 文件

---

## 启动前准备

### 1. 确保依赖服务运行

#### MySQL 数据库
```bash
# 启动 MySQL 服务
net start MySQL

# 或使用 MySQL Workbench 启动
```

#### Redis 服务
```bash
# 启动 Redis (如果已安装)
redis-server

# 或者脚本会自动启动
```

### 2. 配置文件检查

确保 `server/config/config.yaml` 文件存在并配置正确：

```yaml
server:
  host: 0.0.0.0
  port: 8080
  mode: debug

database:
  host: localhost
  port: 3306
  user: root
  password: 123456
  dbname: sanguo_strategy

redis:
  host: localhost
  port: 6379
  password: ""
  db: 0
```

### 3. 初始化数据库 (首次运行)

```bash
# 创建数据库
mysql -u root -p123456 -e "CREATE DATABASE IF NOT EXISTS sanguo_strategy CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"

# 创建表结构
mysql -u root -p123456 --default-character-set=utf8mb4 -e "source D:/sgsa/server/scripts/create_tables.sql"

# 导入初始数据
mysql -u root -p123456 --default-character-set=utf8mb4 sanguo_strategy -e "source D:/sgsa/server/scripts/init_data.sql"
```

---

## 手动启动 (不使用脚本)

### 1. 进入服务器目录
```bash
cd server
```

### 2. 运行服务器
```bash
# 直接运行
go run cmd/server/main.go

# 或编译后运行
go build -o sanguo_server.exe cmd/server/main.go
.\sanguo_server.exe
```

---

## 启动脚本说明

### start_server.bat
- 自动检查 Redis 和 MySQL 服务
- 自动启动 Redis (如果未运行)
- 启动游戏服务器
- 适合生产环境

### start_server.ps1
- PowerShell 版本，功能同上
- 更详细的状态输出
- 更好的错误处理

### start_server_dev.bat
- 开发模式启动
- 支持 Air 热重载 (需要先安装)
- 代码修改后自动重新编译

---

## 安装 Air 热重载工具 (可选)

```bash
# 安装 Air
go install github.com/cosmtrek/air@latest

# 在 server 目录创建 .air.toml 配置文件
# 然后运行 start_server_dev.bat 即可使用热重载
```

---

## 验证服务器运行

### 1. 检查服务器状态
打开浏览器访问：
```
http://localhost:8080/health
```

应该返回：
```json
{
  "status": "ok",
  "timestamp": "2025-10-20T12:00:00Z"
}
```

### 2. 查看 API 文档
```
http://localhost:8080/api/docs
```

### 3. WebSocket 连接测试
```
ws://localhost:8080/ws
```

---

## 常见问题

### 1. Redis 未启动
**错误**: `Failed to connect to Redis`

**解决**:
```bash
# 手动启动 Redis
redis-server

# 或下载 Windows 版 Redis
# https://github.com/tporadowski/redis/releases
```

### 2. MySQL 连接失败
**错误**: `Failed to initialize database`

**解决**:
- 检查 MySQL 服务是否运行
- 检查 config.yaml 中的数据库配置
- 确认数据库用户名和密码正确

### 3. 端口被占用
**错误**: `bind: address already in use`

**解决**:
```bash
# 查找占用端口的进程
netstat -ano | findstr :8080

# 结束进程
taskkill /PID <进程ID> /F

# 或修改 config.yaml 中的端口号
```

### 4. Go 依赖问题
**错误**: `cannot find package`

**解决**:
```bash
cd server
go mod tidy
go mod download
```

---

## 停止服务器

- **批处理/PowerShell**: 按 `Ctrl+C` 或关闭窗口
- **优雅关闭**: 服务器会自动保存状态并关闭连接

---

## 服务器日志

日志文件位置：
```
server/logs/
  - server.log      # 主服务器日志
  - error.log       # 错误日志
  - game.log        # 游戏逻辑日志
```

查看实时日志：
```bash
# PowerShell
Get-Content server/logs/server.log -Wait -Tail 50

# CMD
tail -f server/logs/server.log
```

---

## 技术支持

如有问题，请查看：
1. 项目文档: `PROJECT_STRUCTURE.md`
2. API 文档: `http://localhost:8080/api/docs`
3. 日志文件: `server/logs/`

---

**祝游戏开发顺利！** 🎮

