# 项目结构说明

## 📁 完整目录结构

```
sgs/                                    # 项目根目录
├── README.md                           # 项目主文档
├── QUICKSTART.md                       # 快速启动指南
├── DEPLOYMENT.md                       # 部署文档
├── PROJECT_STRUCTURE.md                # 本文件
├── .gitignore                          # Git 忽略文件
├── game_design_utf8.txt                # 游戏设计文档
│
├── server/                             # Go 后端服务器
│   ├── go.mod                          # Go 模块定义
│   ├── go.sum                          # Go 依赖校验
│   ├── Makefile                        # 构建脚本
│   ├── Dockerfile                      # Docker 镜像配置
│   ├── docker-compose.yml              # Docker Compose 配置
│   ├── .gitignore                      # 服务器 Git 忽略
│   │
│   ├── cmd/                            # 程序入口
│   │   └── server/
│   │       └── main.go                 # 服务器主程序
│   │
│   ├── internal/                       # 内部业务逻辑
│   │   ├── config/                     # 配置管理
│   │   │   └── config.go               # 配置加载
│   │   │
│   │   ├── database/                   # 数据库
│   │   │   ├── database.go             # 数据库初始化
│   │   │   └── models.go               # 数据库模型
│   │   │
│   │   ├── models/                     # 业务模型
│   │   │   └── game_models.go          # 游戏数据模型
│   │   │
│   │   ├── game/                       # 游戏核心逻辑
│   │   │   ├── game_manager.go         # 游戏管理器
│   │   │   ├── room.go                 # 房间管理
│   │   │   ├── matchmaking.go          # 匹配系统
│   │   │   └── game_logic.go           # 游戏逻辑
│   │   │
│   │   ├── router/                     # 路由
│   │   │   └── router.go               # 路由配置
│   │   │
│   │   └── handler/                    # HTTP 处理器
│   │       ├── auth_handler.go         # 认证处理
│   │       ├── room_handler.go         # 房间处理
│   │       ├── player_handler.go       # 玩家处理
│   │       ├── match_handler.go        # 匹配处理
│   │       ├── data_handler.go         # 数据处理
│   │       └── websocket_handler.go    # WebSocket 处理
│   │
│   ├── pkg/                            # 公共包
│   │   └── logger/                     # 日志
│   │       └── logger.go               # 日志工具
│   │
│   ├── config/                         # 配置文件
│   │   └── config.example.yaml         # 配置模板
│   │
│   └── scripts/                        # 脚本
│       └── init_data.sql               # 数据库初始化脚本
│
└── client/                             # Unity 客户端
    ├── README.md                       # 客户端文档
    │
    └── Assets/                         # Unity 资源
        ├── Scripts/                    # C# 脚本
        │   ├── Network/                # 网络通信
        │   │   ├── NetworkManager.cs   # WebSocket 管理
        │   │   └── ApiClient.cs        # HTTP API 客户端
        │   │
        │   ├── Game/                   # 游戏逻辑
        │   │   ├── GameManager.cs      # 游戏管理器
        │   │   ├── BoardManager.cs     # 棋盘管理
        │   │   ├── CardManager.cs      # 卡牌管理
        │   │   ├── GeneralController.cs # 武将控制器
        │   │   └── TerrainController.cs # 地形控制器
        │   │
        │   ├── UI/                     # 用户界面
        │   │   └── UIManager.cs        # UI 管理器
        │   │
        │   └── Steam/                  # Steam 集成
        │       └── SteamManager.cs     # Steam 管理器
        │
        ├── Scenes/                     # 游戏场景
        │   ├── MainMenu.unity          # 主菜单场景
        │   ├── GameScene.unity         # 游戏场景
        │   └── RoomList.unity          # 房间列表场景
        │
        ├── Prefabs/                    # 预制体
        │   ├── General.prefab          # 武将预制体
        │   ├── Terrain.prefab          # 地形预制体
        │   ├── Card.prefab             # 卡牌预制体
        │   └── Tile.prefab             # 格子预制体
        │
        ├── Materials/                  # 材质
        ├── Textures/                   # 贴图
        ├── Plugins/                    # 插件
        │   └── Steamworks.NET/         # Steam API
        └── Resources/                  # 资源
            ├── Generals/               # 武将资源
            ├── Terrains/               # 地形资源
            └── Cards/                  # 卡牌资源
```

## 🏗️ 架构说明

### 服务器架构（Go）

```
┌─────────────────────────────────────────────────────────┐
│                      客户端 (Unity)                       │
└─────────────────────────────────────────────────────────┘
                          ▲
                          │ HTTP / WebSocket
                          ▼
┌─────────────────────────────────────────────────────────┐
│                   API Gateway (Gin)                      │
│  ┌──────────┬──────────┬──────────┬──────────┐         │
│  │  Auth    │  Room    │  Match   │  Data    │         │
│  │ Handler  │ Handler  │ Handler  │ Handler  │         │
│  └──────────┴──────────┴──────────┴──────────┘         │
└─────────────────────────────────────────────────────────┘
                          ▲
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                   Game Manager                           │
│  ┌──────────┬──────────┬──────────┬──────────┐         │
│  │  Room    │ Matching │  Game    │  Player  │         │
│  │ Manager  │  Queue   │  Logic   │ Manager  │         │
│  └──────────┴──────────┴──────────┴──────────┘         │
└─────────────────────────────────────────────────────────┘
                          ▲
                          │
            ┌─────────────┴─────────────┐
            ▼                           ▼
┌─────────────────────┐     ┌─────────────────────┐
│      MySQL          │     │      Redis          │
│  - 玩家数据         │     │  - 会话缓存         │
│  - 武将配置         │     │  - 匹配队列         │
│  - 地形配置         │     │  - 实时数据         │
│  - 游戏记录         │     │  - 排行榜           │
└─────────────────────┘     └─────────────────────┘
```

### 客户端架构（Unity）

```
┌─────────────────────────────────────────────────────────┐
│                     UI Layer                             │
│  ┌──────────┬──────────┬──────────┬──────────┐         │
│  │ MainMenu │  Game UI │ RoomList │ Settings │         │
│  └──────────┴──────────┴──────────┴──────────┘         │
└─────────────────────────────────────────────────────────┘
                          ▲
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                  Game Logic Layer                        │
│  ┌──────────┬──────────┬──────────┬──────────┐         │
│  │  Game    │  Board   │  Card    │  Input   │         │
│  │ Manager  │ Manager  │ Manager  │ Manager  │         │
│  └──────────┴──────────┴──────────┴──────────┘         │
└─────────────────────────────────────────────────────────┘
                          ▲
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                  Network Layer                           │
│  ┌──────────────────┬──────────────────┐               │
│  │  NetworkManager  │    ApiClient     │               │
│  │   (WebSocket)    │     (HTTP)       │               │
│  └──────────────────┴──────────────────┘               │
└─────────────────────────────────────────────────────────┘
                          ▲
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                      │
│  ┌──────────┬──────────┬──────────┬──────────┐         │
│  │ General  │ Terrain  │   Card   │   UI     │         │
│  │  Prefab  │  Prefab  │  Prefab  │ Element  │         │
│  └──────────┴──────────┴──────────┴──────────┘         │
└─────────────────────────────────────────────────────────┘
```

## 🔄 数据流

### 游戏操作流程

```
Client                    Server                    Database
  │                         │                          │
  │  1. Play Card          │                          │
  ├───────────────────────>│                          │
  │                         │                          │
  │                         │  2. Validate            │
  │                         │<───────Game Logic───────>│
  │                         │                          │
  │  3. Update Response    │                          │
  │<────────────────────────┤                          │
  │                         │                          │
  │  4. Broadcast Update   │                          │
  │<══════════════════════╗ │                          │
  │                       ║ │                          │
Other Clients           ║ │                          │
  │<══════════════════════╝ │                          │
  │                         │                          │
  │                         │  5. Save Game State     │
  │                         ├─────────────────────────>│
  │                         │                          │
```

### 认证流程

```
Client                    Server                    Database
  │                         │                          │
  │  1. Login Request      │                          │
  ├───────────────────────>│                          │
  │                         │                          │
  │                         │  2. Query User          │
  │                         ├─────────────────────────>│
  │                         │<─────────────────────────┤
  │                         │                          │
  │                         │  3. Verify Password     │
  │                         │     Generate JWT        │
  │                         │                          │
  │  4. JWT Token          │                          │
  │<────────────────────────┤                          │
  │                         │                          │
  │  5. Subsequent Requests │                          │
  │  (with JWT in header)   │                          │
  ├───────────────────────>│                          │
  │                         │                          │
```

## 💾 数据模型

### 核心数据表

```sql
-- 玩家表
players (
    id UUID PRIMARY KEY,
    username VARCHAR,
    email VARCHAR,
    password_hash VARCHAR,
    level INT,
    coins INT,
    rank INT
)

-- 武将表
generals (
    id UUID PRIMARY KEY,
    name VARCHAR,
    faction VARCHAR,  -- 魏/蜀/吴/群
    role VARCHAR,     -- 输出/控制/辅助
    hp INT,
    skills_json JSONB
)

-- 地形表
terrains (
    id UUID PRIMARY KEY,
    name VARCHAR,
    type VARCHAR,      -- 基础/事件/交互
    category VARCHAR,  -- 山地/平原/河流
    effects_json JSONB
)

-- 房间表
rooms (
    id UUID PRIMARY KEY,
    host_id UUID,
    status VARCHAR,    -- waiting/playing/finished
    config_json JSONB
)

-- 游戏记录表
game_histories (
    id UUID PRIMARY KEY,
    room_id UUID,
    winner_team VARCHAR,
    duration INT,
    players_json JSONB
)
```

## 🔌 API 接口

### RESTful API

```
认证
POST   /api/v1/auth/register      # 注册
POST   /api/v1/auth/login         # 登录
POST   /api/v1/auth/steam-auth    # Steam认证

玩家
GET    /api/v1/player/profile     # 获取资料
PUT    /api/v1/player/profile     # 更新资料
GET    /api/v1/player/stats       # 获取统计

房间
GET    /api/v1/room/list          # 房间列表
POST   /api/v1/room/create        # 创建房间
POST   /api/v1/room/:id/join      # 加入房间
POST   /api/v1/room/:id/leave     # 离开房间
POST   /api/v1/room/:id/start     # 开始游戏

匹配
POST   /api/v1/match/join         # 加入匹配
POST   /api/v1/match/leave        # 离开匹配

数据
GET    /api/v1/data/generals      # 武将数据
GET    /api/v1/data/terrains      # 地形数据
```

### WebSocket 消息

```javascript
// 客户端 -> 服务器
{
  "type": "game_action",
  "room_id": "uuid",
  "data": {
    "action_type": "play_card",
    "card_id": "uuid",
    "target_ids": ["uuid"]
  }
}

// 服务器 -> 客户端
{
  "type": "game_state_update",
  "room_id": "uuid",
  "data": {
    "round": 3,
    "phase": "combat",
    "players": [...],
    "terrains": {...}
  }
}
```

## 🎨 资源命名规范

### 服务器端

- 文件名：`snake_case.go`
- 包名：`package`
- 类型：`PascalCase`
- 函数：`PascalCase` (公开) / `camelCase` (私有)
- 常量：`UPPER_SNAKE_CASE`

### 客户端

- 文件名：`PascalCase.cs`
- 类名：`PascalCase`
- 方法：`PascalCase` (公开) / `PascalCase` (私有)
- 变量：`camelCase`
- 常量：`PascalCase`

### Unity 资源

- 场景：`PascalCase.unity`
- 预制体：`PascalCase.prefab`
- 材质：`Material_Name.mat`
- 贴图：`texture_name.png`

## 📦 部署结构

### Docker 容器

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│              │     │              │     │              │
│    MySQL     │────▶│    Server    │◀────│    Redis     │
│  Container   │     │  Container   │     │  Container   │
│              │     │              │     │              │
└──────────────┘     └──────────────┘     └──────────────┘
      :3306               :8080                :6379
```

### Steam 发布结构

```
SanguoStrategy/
├── Windows/
│   ├── SanguoStrategy.exe
│   ├── steam_api64.dll
│   └── ...
├── macOS/
│   └── SanguoStrategy.app
└── Linux/
    ├── SanguoStrategy.x86_64
    ├── libsteam_api.so
    └── ...
```

## 🔐 安全考虑

1. **密码加密**：使用 bcrypt
2. **JWT 认证**：有效期 7 天
3. **WebSocket 认证**：通过 token 验证
4. **SQL 注入防护**：使用参数化查询
5. **XSS 防护**：输入验证和输出转义
6. **CORS 配置**：限制允许的源

## 📚 技术栈总结

### 后端
- **语言**：Go 1.21
- **框架**：Gin
- **数据库**：MySQL 8.0
- **缓存**：Redis 7
- **WebSocket**：gorilla/websocket
- **ORM**：GORM

### 前端
- **引擎**：Unity 2021.3 LTS
- **语言**：C# .NET 4.8
- **网络**：WebSocketSharp
- **JSON**：Newtonsoft.Json
- **Steam**：Steamworks.NET

### 部署
- **容器**：Docker & Docker Compose
- **CI/CD**：GitHub Actions (待添加)
- **监控**：Prometheus + Grafana (待添加)

---

*项目持续更新中...*

