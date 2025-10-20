# 快速启动指南

## 🚀 5分钟快速体验

### 前置要求

- Docker & Docker Compose
- Go 1.21+ (可选，用于本地开发)
- Unity 2021.3 LTS+ (可选，用于客户端开发)

### 步骤 1：启动服务器（最简单方式）

```bash
# 克隆项目
git clone <repository-url>
cd sgs

# 使用 Docker Compose 一键启动
cd server
docker-compose up -d

# 查看日志
docker-compose logs -f server
```

服务器将在以下端口启动：
- HTTP API: `http://localhost:8080`
- WebSocket: `ws://localhost:8080/ws`
- MySQL: `localhost:3306`
- Redis: `localhost:6379`

### 步骤 2：测试 API

```bash
# 健康检查
curl http://localhost:8080/health

# 注册用户
curl -X POST http://localhost:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "player1",
    "email": "player1@example.com",
    "password": "password123"
  }'

# 登录
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "player1",
    "password": "password123"
  }'

# 获取武将列表
curl http://localhost:8080/api/v1/data/generals

# 获取地形列表
curl http://localhost:8080/api/v1/data/terrains

# 获取房间列表
curl http://localhost:8080/api/v1/room/list
```

### 步骤 3：在 Unity 中测试客户端

1. 打开 Unity Hub
2. 添加项目：`sgs/client`
3. 打开场景：`Assets/Scenes/MainMenu.unity`
4. 点击 Play 按钮
5. 测试功能：
   - 注册/登录
   - 快速匹配
   - 自定义房间

## 📦 手动部署（不使用 Docker）

### 服务器端

```bash
cd server

# 1. 安装依赖
go mod download

# 2. 配置数据库
# 确保 MySQL 和 Redis 已安装并运行
mysql -u root -p -e "CREATE DATABASE sanguo_strategy CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
mysql -u root -p sanguo_strategy < scripts/init_data.sql

# 3. 配置文件
cp config/config.example.yaml config/config.yaml
# 编辑 config.yaml，填入数据库信息

# 4. 启动服务器
go run cmd/server/main.go

# 或者编译后运行
make build
./bin/server
```

### 客户端

```bash
cd client

# 1. 在 Unity 中打开项目
# 2. 安装依赖插件（见 client/README.md）
# 3. 配置服务器地址
#    - NetworkManager.cs: serverUrl
#    - ApiClient.cs: apiBaseUrl
# 4. 运行或构建
```

## 🎮 游戏体验流程

### 1. 注册账号

- 打开客户端
- 点击"注册"
- 输入用户名、邮箱、密码
- 完成注册

### 2. 快速匹配

- 点击"快速匹配"
- 等待其他玩家
- 自动进入游戏

### 3. 自定义房间

- 点击"自定义房间"
- 创建房间或加入现有房间
- 设置游戏模式：
  - 固定地形
  - 随机地形
  - 地形包选择
- 等待所有玩家准备
- 开始游戏

### 4. 游戏流程

**准备阶段（2分钟）**
- 系统随机分配武将池
- 每人从每类（输出/控制/辅助）选1个武将
- 可选择地形规则

**部署阶段（1回合）**
- 轮流在初始区域放置地形
- 选择武将初始位置

**战斗阶段（15回合）**
- 回合流程：
  1. 地形效果生效
  2. 移动/交互地形
  3. 出牌阶段
  4. 地形结算

**结算**
- 根据胜利条件判定胜负
- 获得奖励和经验

## 🔧 开发模式

### 服务器热重载

```bash
# 安装 air（热重载工具）
go install github.com/cosmtrek/air@latest

# 启动开发模式
cd server
air
```

### 客户端调试

1. 在 Unity 中按 Play
2. 使用 Console 查看日志
3. 使用 Profiler 分析性能
4. 使用 Frame Debugger 调试渲染

## 📊 数据查看

### 数据库

```bash
# 连接数据库
mysql -h localhost -u sanguo_user -p sanguo_strategy

# 查看武将
SELECT name, faction, role, hp FROM generals;

# 查看地形
SELECT name, type, category FROM terrains;

# 查看玩家
SELECT username, level, rank, coins FROM players;

# 查看游戏记录
SELECT * FROM game_histories ORDER BY created_at DESC LIMIT 10;
```

### Redis 缓存

```bash
# 连接 Redis
redis-cli

# 查看所有键
KEYS *

# 查看在线玩家
SMEMBERS online_players

# 查看匹配队列
LRANGE matchmaking_queue 0 -1
```

## 🐛 常见问题排查

### 服务器无法启动

```bash
# 检查端口占用
lsof -i :8080
netstat -tuln | grep 8080

# 检查数据库连接
mysql -h localhost -u sanguo_user -p sanguo_strategy

# 检查 Redis 连接
redis-cli ping

# 查看日志
docker-compose logs server
tail -f logs/server.log
```

### 客户端连接失败

1. 检查服务器是否运行：`curl http://localhost:8080/health`
2. 检查防火墙设置
3. 检查客户端配置的服务器地址
4. 查看 Unity Console 的错误信息

### 数据库错误

```bash
# 重置数据库
mysql -u root -p -e "DROP DATABASE IF EXISTS sanguo_strategy;"
mysql -u root -p -e "CREATE DATABASE sanguo_strategy CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
mysql -u root -p sanguo_strategy < scripts/init_data.sql

# 或使用 Docker
docker-compose down -v
docker-compose up -d
```

## 📈 性能测试

### 压力测试

```bash
# 安装 k6
brew install k6  # macOS
# 或下载：https://k6.io/

# 运行测试
k6 run tests/load_test.js

# 100 并发用户测试
k6 run --vus 100 --duration 30s tests/load_test.js
```

### 性能监控

```bash
# 服务器指标
curl http://localhost:8080/metrics

# 数据库性能
mysql -u sanguo_user -p -e "SHOW PROCESSLIST;" sanguo_strategy

# Redis 性能
redis-cli info stats
```

## 📚 更多文档

- [完整部署指南](DEPLOYMENT.md)
- [服务器 README](server/README.md) - 待创建
- [客户端 README](client/README.md)
- [API 文档](docs/API.md) - 待创建
- [游戏设计文档](game_design_utf8.txt)

## 💡 下一步

1. **添加更多武将和地形**
   - 编辑 `server/scripts/init_data.sql`
   - 创建相应的预制体和资源

2. **实现完整的游戏逻辑**
   - 卡牌系统
   - 技能系统
   - 地形交互
   - 羁绊系统

3. **优化UI和用户体验**
   - 完善界面设计
   - 添加动画效果
   - 音效和背景音乐

4. **集成 Steam**
   - 配置 Steamworks
   - 实现成就系统
   - 添加好友功能

5. **准备发布**
   - 完善商店页面
   - 录制宣传视频
   - 准备营销材料

## 🎉 开始游戏！

现在你已经成功启动了游戏，可以开始体验三国志：地形策略版了！

祝你玩得愉快！ 🎮⚔️

---

有问题？查看 [GitHub Issues](链接) 或加入我们的 [Discord 社区](链接)

