# 三国志：地形策略版 (Sanguo Strategy: Terrain Edition)

> 融合《怪谈事务所》机制与《三国杀》玩法的创新策略卡牌游戏

[![Unity](https://img.shields.io/badge/Unity-2021.3_LTS-black.svg)](https://unity.com/)
[![Go](https://img.shields.io/badge/Go-1.21+-00ADD8.svg)](https://golang.org/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1.svg)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/License-学习演示-green.svg)](LICENSE)

## 📖 目录

- [项目简介](#项目简介)
- [核心特色](#核心特色)
- [快速开始](#快速开始)
- [技术架构](#技术架构)
- [游戏功能](#游戏功能)
- [开发指南](#开发指南)
- [部署说明](#部署说明)
- [常见问题](#常见问题)

---

## 🎮 项目简介

三国志：地形策略版是一款融合《怪谈事务所》机制与《三国杀》玩法的创新策略卡牌游戏，支持 2-6 人在线对战。游戏特色为**动态地形系统**与**随机武将联动机制**，打破传统三国杀的固定套路，提供全新的策略体验。

### 为什么选择本项目？

- ✅ **完整的全栈游戏框架**：从服务器到客户端，从开发到部署
- ✅ **创新的游戏机制**：地形系统 + 随机选将 + 历史羁绊
- ✅ **现代化技术栈**：Go + Unity + MySQL + WebSocket
- ✅ **详尽的开发文档**：快速上手，易于扩展
- ✅ **Steam 就绪**：集成 Steamworks，可直接发布

---

## 🌟 核心特色

### 🎲 创新玩法

#### 1. 动态地形系统
- **12回合制战斗**：每回合分为探索、部署、战斗三个阶段
- **多样地形类型**：山地、平原、河流、森林、祭坛、烽火台等 15+ 种地形
- **地形联动**：武将技能与地形深度结合，创造独特战术
- **动态生成**：根据队伍配置智能调整地形分布

#### 2. 强制随机选将
- **打破套路**：每局随机武将池，告别固定阵容
- **职能平衡**：输出/控制/辅助/特殊四类分类，强制平衡队伍
- **策略深度**：迫使玩家开发非常规战术和组合

#### 3. 历史羁绊机制
- **典故重现**：特定武将组合激活隐藏技能
- **跨阵营协作**：魏蜀吴武将混编触发特殊效果
- **IF线剧情**：创造非史实组合的全新叙事

#### 4. 多人协作模式
- **2-6人组队**：支持灵活的组队配置
- **多角色控制**：每位玩家控制 2-n 个武将
- **合击技系统**：团队协作触发强力组合技能
- **快节奏体验**：单局15-20分钟，适合休闲玩家

---

## 🚀 快速开始

### 环境要求

**服务器端：**
- Go 1.21+
- MySQL 8.0+
- Redis 7+
- Docker & Docker Compose（可选）

**客户端：**
- Unity 2021.3 LTS+
- .NET Framework 4.8+
- Visual Studio 2022 或 JetBrains Rider

### 30秒快速启动

#### 1. 启动服务器

```bash
# 克隆项目
git clone <repository-url>
cd sgsa

# 启动服务器（方式1：使用批处理文件）
.\start_server.bat

# 或方式2：使用Docker
cd server
docker-compose up -d
go run cmd/server/main.go
```

服务器启动成功后访问：http://localhost:8080/health

#### 2. 打开 Unity 客户端

```bash
# 使用批处理文件一键打开Unity
.\open_unity_hub.bat

# 或手动打开
# 1. 打开 Unity Hub
# 2. 添加项目：选择 client 文件夹
# 3. 打开项目
```

#### 3. 创建登录场景（首次运行）

在 Unity 编辑器中：
```
菜单：三国策略 → 场景管理 → 创建登录场景
```

#### 4. 运行游戏

点击 Unity 的 **Play** 按钮（▶），开始体验！

### 功能测试

1. **注册账号**：在登录界面点击"注册"，创建新账号
2. **登录游戏**：使用注册的账号登录
3. **快速匹配**：在主菜单点击"快速匹配"
4. **房间系统**：创建或加入房间
5. **选将对战**：选择武将开始游戏

---

## 🏗️ 技术架构

### 系统架构图

```
┌─────────────────┐     WebSocket/HTTP      ┌─────────────────┐
│                 │ ←────────────────────→  │                 │
│  Unity Client   │                         │   Go Server     │
│  (C# .NET 4.8)  │                         │   (Gin + GORM)  │
│                 │                         │                 │
└─────────────────┘                         └────────┬────────┘
                                                     │
                                            ┌────────┴────────┐
                                            │                 │
                                         ┌──▼──┐         ┌───▼───┐
                                         │MySQL│         │ Redis │
                                         └─────┘         └───────┘
```

### 后端技术栈

```
server/
├── cmd/server/           # 主程序入口
├── internal/
│   ├── handler/          # HTTP/WebSocket 处理器
│   ├── game/            # 游戏核心逻辑
│   ├── database/        # 数据库操作
│   ├── models/          # 数据模型
│   └── router/          # 路由和中间件
├── pkg/
│   └── logger/          # 日志工具
├── config/              # 配置文件
└── scripts/             # SQL初始化脚本

技术：
- 语言：Go 1.21+
- Web框架：Gin
- ORM：GORM
- 数据库：MySQL 8.0
- 缓存：Redis 7
- WebSocket：gorilla/websocket
- 认证：JWT (golang-jwt/jwt)
- 加密：bcrypt
```

### 前端技术栈

```
client/
├── Assets/
│   ├── Scenes/           # 游戏场景
│   │   ├── Login.unity
│   │   ├── MainMenu.unity
│   │   ├── RoomList.unity
│   │   ├── HeroSelection.unity
│   │   └── GameScene.unity
│   ├── Scripts/
│   │   ├── UI/           # UI控制器
│   │   ├── Network/      # 网络通信
│   │   ├── Game/         # 游戏逻辑
│   │   └── Editor/       # 编辑器工具
│   ├── Prefabs/          # 预制体
│   ├── Resources/        # 资源文件
│   └── Fonts/            # 中文字体

技术：
- 引擎：Unity 2021.3 LTS
- 语言：C# .NET 4.8
- 网络：WebSocketSharp
- JSON：Newtonsoft.Json
- Steam：Steamworks.NET
```

### 数据库设计

**核心数据表：**

| 表名 | 说明 | 主要字段 |
|------|------|----------|
| players | 玩家账号 | id, username, email, password_hash, level, coins |
| player_stats | 玩家统计 | player_id, total_games, wins, losses, win_rate |
| generals | 武将配置 | id, name, faction, role, skills |
| terrains | 地形配置 | id, name, type, effects, duration |
| rooms | 游戏房间 | id, name, status, players, config |
| game_histories | 游戏记录 | id, room_id, winner, duration, stats |

---

## 🎯 游戏功能

### ✅ 已实现功能

#### 账号系统
- [x] 用户注册（用户名 + 邮箱 + 密码）
- [x] 用户登录（JWT Token认证）
- [x] 自动登录（Token持久化）
- [x] 密码加密（bcrypt）
- [x] 会话管理（7天有效期）

#### 房间系统
- [x] 创建房间
- [x] 加入房间
- [x] 房间列表
- [x] 快速匹配
- [x] 房间配置（人数、地形规则）

#### 网络通信
- [x] HTTP API（RESTful）
- [x] WebSocket 实时通信
- [x] 状态同步
- [x] 错误处理

#### UI系统
- [x] 登录/注册界面
- [x] 主菜单
- [x] 房间列表
- [x] 选将界面
- [x] 游戏场景
- [x] 中文字体支持

#### 编辑器工具
- [x] 场景自动创建工具
- [x] 中文字体修复工具
- [x] 网络设置向导
- [x] UI自动修复器

### 📋 待开发功能

#### 游戏逻辑
- [ ] 完整的回合制流程
- [ ] 卡牌效果实现（杀、闪、桃等）
- [ ] 武将技能完整实现（20+ 武将）
- [ ] 地形交互逻辑（15+ 地形）
- [ ] 羁绊系统触发
- [ ] 胜利条件判定

#### 系统功能
- [ ] 商城系统
- [ ] 任务系统
- [ ] 排行榜
- [ ] 好友系统
- [ ] 聊天系统
- [ ] 观战系统
- [ ] 回放系统

#### Steam 集成
- [ ] 成就系统
- [ ] 云存档
- [ ] 交易卡牌
- [ ] 工坊支持
- [ ] 好友邀请

#### 美术资源
- [ ] 武将/地形 3D 模型
- [ ] 动画系统
- [ ] 音效和背景音乐
- [ ] 粒子特效
- [ ] UI美术资源

---

## 💻 开发指南

### 服务器开发

#### 配置数据库

编辑 `server/config/config.yaml`：

```yaml
database:
  host: "localhost"
  port: 3306
  user: "root"
  password: "your-password"
  dbname: "sanguo_strategy"

jwt:
  secret_key: "your-secret-key-change-this"
  token_expire_hours: 168

redis:
  host: "localhost"
  port: 6379
```

#### 初始化数据库

```bash
cd server
# 使用 Docker（推荐）
docker-compose up -d

# 或手动创建
mysql -u root -p -e "CREATE DATABASE sanguo_strategy CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
mysql -u root -p sanguo_strategy < scripts/create_tables.sql
mysql -u root -p sanguo_strategy < scripts/init_data.sql
```

#### 运行开发服务器

```bash
# 热重载开发模式（需要安装 air）
go install github.com/cosmtrek/air@latest
air

# 或直接运行
go run cmd/server/main.go
```

#### API 测试

```bash
# 健康检查
curl http://localhost:8080/health

# 注册
curl -X POST http://localhost:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","email":"test@example.com","password":"123456"}'

# 登录
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"123456"}'
```

### Unity 客户端开发

#### 场景管理

使用编辑器工具自动创建场景：
```
菜单：三国策略 → 场景设置向导 → 一键设置所有场景
```

或手动创建：
1. 创建新场景：`Assets → Create → Scene`
2. 添加 Canvas 和 EventSystem
3. 创建 UI 元素
4. 添加控制器脚本

#### 中文字体修复

如果中文显示为方块：
```
菜单：三国策略 → 修复中文字体显示
```

#### 网络配置

编辑 `Assets/Scripts/Network/ApiClient.cs`：

```csharp
[Header("Network")]
[SerializeField] private string apiBaseUrl = "http://localhost:8080/api/v1";
[SerializeField] private string wsUrl = "ws://localhost:8080/ws";
```

#### 构建设置

```
File → Build Settings
1. 添加所有场景（Login → MainMenu → RoomList → HeroSelection → GameScene）
2. 选择平台（PC, Mac & Linux Standalone）
3. Player Settings → Company Name / Product Name
4. Build
```

### 代码规范

**Go 代码：**
- 使用 `gofmt` 格式化代码
- 提交前运行 `go test ./...`
- 遵循 [Uber Go Style Guide](https://github.com/uber-go/guide)

**C# 代码：**
- 使用 PascalCase 命名类和方法
- 使用 camelCase 命名变量
- 遵循 [Unity C# 代码规范](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity)

### Git 提交规范

```
[模块] 简要说明

详细说明（可选）

示例：
[Server] 添加JWT认证中间件
[Client] 修复登录界面中文显示问题
[Database] 优化玩家数据查询性能
```

---

## 🚢 部署说明

### Docker 部署（推荐）

```bash
# 1. 克隆项目
git clone <repository-url>
cd sgsa/server

# 2. 配置环境变量
cp config/config.example.yaml config/config.yaml
# 编辑 config.yaml 填入实际配置

# 3. 启动所有服务
docker-compose up -d

# 4. 查看日志
docker-compose logs -f

# 5. 停止服务
docker-compose down
```

### 手动部署

#### 服务器部署

```bash
# 编译
cd server
make build

# 运行
./bin/server

# 或使用 systemd
sudo cp server.service /etc/systemd/system/
sudo systemctl enable server
sudo systemctl start server
```

#### 客户端发布

**Windows：**
```
Target Platform: PC, Mac & Linux Standalone
Target OS: Windows
Architecture: x86_64
```

**macOS：**
```
Target Platform: PC, Mac & Linux Standalone
Target OS: macOS
Architecture: x86_64 + ARM64 (Apple Silicon)
```

**Linux：**
```
Target Platform: PC, Mac & Linux Standalone
Target OS: Linux
Architecture: x86_64
```

### Steam 发布

详细步骤请参考：[DEPLOYMENT.md](DEPLOYMENT.md)

1. 在 [Steamworks Partner](https://partner.steamgames.com/) 注册应用
2. 下载 Steamworks SDK
3. 配置 `steam_appid.txt`
4. 构建并上传到 Steam
5. 配置商店页面
6. 提交审核

---

## ❓ 常见问题

### 服务器相关

**Q: 数据库连接失败？**

A: 检查以下几点：
1. MySQL 是否正常运行：`docker ps` 或 `systemctl status mysql`
2. `config.yaml` 中的数据库配置是否正确
3. 数据库是否已创建：`mysql -u root -p -e "SHOW DATABASES;"`
4. 防火墙是否开放 3306 端口

**Q: Redis 连接失败？**

A: 
1. 检查 Redis 是否运行：`docker ps` 或 `redis-cli ping`
2. 确认 Redis 端口 6379 未被占用
3. 检查 `config.yaml` 中的 Redis 配置

**Q: WebSocket 连接断开？**

A:
1. 检查服务器日志
2. 确认防火墙开放 8080 端口
3. 检查网络代理设置

### Unity 客户端相关

**Q: 场景是空的，只有摄像机？**

A: 运行场景设置向导重新生成场景：
```
三国策略 → 场景设置向导 → 一键设置所有场景
```

**Q: 中文显示为方块 □？**

A: 运行中文字体修复工具：
```
三国策略 → 修复中文字体显示
```

**Q: 按钮点击没反应？**

A: 检查场景中是否有 EventSystem：
```
Hierarchy → 右键 → UI → Event System
```

**Q: 无法连接服务器？**

A:
1. 确认服务器已启动：`curl http://localhost:8080/health`
2. 检查 `ApiClient.cs` 中的 `apiBaseUrl` 配置
3. 查看 Unity Console 的错误日志

**Q: Build 后无法运行？**

A:
1. 检查 Build Settings 中场景顺序（Login → MainMenu → ...）
2. 确认所有依赖的 DLL 已包含
3. 检查目标平台设置

### Steam 集成相关

**Q: Steam API 初始化失败？**

A:
1. 确认 `steam_appid.txt` 文件存在
2. 检查 Steam 客户端是否正在运行
3. 使用测试 App ID (480) 进行开发测试

**Q: 成就无法解锁？**

A:
1. 确认成就已在 Steamworks 后台配置
2. 检查 Steam API 调用是否成功
3. 查看 Steam 客户端的日志

---

## 📚 文档索引

### 核心文档
- [README.md](README.md) - 项目总览（本文档）
- [QUICK_START.md](QUICK_START.md) - 5分钟快速开始指南
- [DEPLOYMENT.md](DEPLOYMENT.md) - 部署和 Steam 发布指南

### 游戏设计
- [game_design_utf8.txt](game_design_utf8.txt) - 完整的游戏设计文档
- [三国玩法设计.docx](三国玩法设计.docx) - 游戏玩法详细设计

### 开发文档
- [登录注册系统README.md](登录注册系统README.md) - 账号系统开发文档
- [Unity场景使用说明.md](Unity场景使用说明.md) - Unity 场景管理指南
- [client/三国策略使用手册.md](client/三国策略使用手册.md) - 客户端开发手册

### 批处理工具
- [start_server.bat](start_server.bat) - 一键启动服务器
- [open_unity_hub.bat](open_unity_hub.bat) - 打开 Unity Hub
- [test_connection.bat](test_connection.bat) - 测试服务器连接
- [测试登录注册.bat](测试登录注册.bat) - API 功能测试

---

## 🎯 开发路线图

### Alpha 版本（当前）
- [x] 基础服务器框架
- [x] 核心数据模型
- [x] 登录注册系统
- [x] 房间管理系统
- [x] 基础 UI 界面
- [ ] 基础战斗逻辑

### Beta 版本（计划中）
- [ ] 完整武将系统（20+ 武将）
- [ ] 完整地形系统（15+ 地形）
- [ ] 匹配系统优化
- [ ] 好友系统
- [ ] 任务系统

### 正式版本（未来）
- [ ] 50+ 武将
- [ ] 30+ 地形
- [ ] 排行榜
- [ ] 回放系统
- [ ] Steam 成就
- [ ] 商城系统

---

## 💰 商业化设计

### 定价策略

**基础版：** $9.99 USD
- 包含基础武将和地形
- 完整游戏体验
- 所有核心功能

**豪华版：** $19.99 USD
- 包含所有初始内容
- 专属武将皮肤
- 战斗特效
- 早期访问新内容

### 内容扩展

**地形主题包：** $4.99 - $7.99
- "赤壁火攻"主题
- "五丈原星象"主题
- "官渡之战"主题

**武将扩展包：** $4.99 - $7.99
- 新武将（5-10名）
- 专属皮肤
- 特殊技能

**战令系统：** $9.99/季
- 赛季专属奖励
- 限定皮肤
- 经验加成

### 公平竞技原则

✅ **核心玩法完全免费**
✅ **无付费优势（Pay-to-Win）**
✅ **所有武将和地形可通过游戏内货币解锁**
✅ **付费内容仅限外观和便利性**

---

## 🤝 贡献指南

欢迎提交 Issue 和 Pull Request！

### 如何贡献

1. Fork 本项目
2. 创建特性分支：`git checkout -b feature/AmazingFeature`
3. 提交更改：`git commit -m '[Feature] Add some AmazingFeature'`
4. 推送分支：`git push origin feature/AmazingFeature`
5. 提交 Pull Request

### 贡献类型

- 🐛 Bug 修复
- ✨ 新功能
- 📝 文档改进
- 🎨 UI/UX 优化
- ⚡ 性能优化
- ♻️ 代码重构

---

## 📄 许可证

本项目仅用于**学习和演示目的**。

**注意**：本游戏是独立创作，与《三国杀》官方无关。所有三国历史人物均为公共领域内容。

---

## 📞 联系方式

- **项目主页**：[GitHub Repository]
- **开发者邮箱**：dev@sanguo-strategy.com
- **Discord 社区**：[待建立]
- **官方网站**：[待建立]

---

## 🎉 致谢

感谢以下开源项目和资源：

- [Unity](https://unity.com/) - 游戏引擎
- [Gin](https://gin-gonic.com/) - Go Web 框架
- [GORM](https://gorm.io/) - Go ORM
- [Steamworks.NET](https://steamworks.github.io/) - Steam SDK
- 所有贡献者和测试玩家

---

## ⭐ Star History

如果这个项目对你有帮助，请给个 Star ⭐！

---

<div align="center">

**立即开始**：查看 [QUICK_START.md](QUICK_START.md) 5分钟快速上手！

Made with ❤️ by the Sanguo Strategy Team

</div>
