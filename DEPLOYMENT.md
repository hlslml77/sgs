# 三国志：地形策略版 - 部署指南

## Steam 集成部署指南

### 前置要求

1. **Steamworks 账号**
   - 在 [Steamworks Partner](https://partner.steamgames.com/) 注册开发者账号
   - 创建新的应用程序
   - 记录 App ID

2. **Steamworks SDK**
   - 下载 [Steamworks SDK](https://partner.steamgames.com/downloads)
   - 解压到项目目录
   - 将 `Steamworks.NET` 插件导入 Unity

### Steam 配置步骤

#### 1. 设置 App ID

在客户端根目录创建 `steam_appid.txt`：
```
480
```
*注意：480 是 Spacewar 测试 ID，正式发布时替换为实际 App ID*

#### 2. Unity 项目配置

1. 导入 `Steamworks.NET` 插件到 `Assets/Plugins/`
2. 在 `Build Settings` 中设置目标平台为 PC
3. 配置 Steam Manager 组件：
   - 将 `SteamManager.cs` 挂载到场景中的空物体
   - 设置为 DontDestroyOnLoad

#### 3. Steam 成就配置

在 Steamworks 后台配置成就：

| 成就ID | 名称 | 描述 |
|--------|------|------|
| ACH_FIRST_WIN | 初次胜利 | 赢得第一场游戏 |
| ACH_WIN_10 | 连胜达人 | 连续赢得10场游戏 |
| ACH_SYNERGY_MASTER | 羁绊大师 | 激活5次不同的羁绊 |
| ACH_TERRAIN_EXPERT | 地形专家 | 在一局游戏中部署10个地形 |
| ACH_LEGENDARY_GENERAL | 传说武将 | 解锁所有传说级武将 |

#### 4. Steam 统计数据

配置统计项：
- `total_games` - 总游戏场数
- `total_wins` - 总胜利次数
- `total_playtime` - 总游戏时长
- `favorite_general` - 最常用武将

### 构建发布版本

#### Windows 版本

```bash
# Unity 构建设置
- Target Platform: PC, Mac & Linux Standalone
- Target OS: Windows
- Architecture: x86_64

# 构建后目录结构
Build/
├── SanguoStrategy.exe
├── SanguoStrategy_Data/
├── steam_api64.dll
├── steam_appid.txt
└── UnityPlayer.dll
```

#### macOS 版本

```bash
# Unity 构建设置
- Target Platform: PC, Mac & Linux Standalone
- Target OS: macOS
- Architecture: x86_64 + ARM64 (Apple Silicon)

# 构建后目录结构
Build/
└── SanguoStrategy.app/
    └── Contents/
        ├── MacOS/
        ├── Resources/
        └── Plugins/
            └── libsteam_api.dylib
```

#### Linux 版本

```bash
# Unity 构建设置
- Target Platform: PC, Mac & Linux Standalone
- Target OS: Linux
- Architecture: x86_64

# 构建后目录结构
Build/
├── SanguoStrategy.x86_64
├── SanguoStrategy_Data/
└── libsteam_api.so
```

### Steam 上传步骤

#### 1. 配置 Depot

在 `steamworks/scripts/app_build_480.vdf` 创建构建配置：

```vdf
"appbuild"
{
    "appid" "480"  // 替换为实际 App ID
    "desc" "Build v1.0.0"
    "buildoutput" "./output/"
    "contentroot" "../Build/"
    "setlive" "default"
    
    "depots"
    {
        "481"  // Windows Depot ID
        {
            "FileMapping"
            {
                "LocalPath" "*"
                "DepotPath" "."
                "recursive" "1"
            }
            "FileExclusion" "*.pdb"
        }
        
        "482"  // macOS Depot ID
        {
            "FileMapping"
            {
                "LocalPath" "*"
                "DepotPath" "."
                "recursive" "1"
            }
        }
        
        "483"  // Linux Depot ID
        {
            "FileMapping"
            {
                "LocalPath" "*"
                "DepotPath" "."
                "recursive" "1"
            }
        }
    }
}
```

#### 2. 使用 SteamPipe 上传

```bash
# Windows
cd steamworks/tools/ContentBuilder
builder\steamcmd.exe +login <username> +run_app_build ..\scripts\app_build_480.vdf +quit

# macOS/Linux
cd steamworks/tools/ContentBuilder
./builder/steamcmd.sh +login <username> +run_app_build ../scripts/app_build_480.vdf +quit
```

### Steam 商店页面配置

#### 必需资源

1. **游戏图标**
   - 格式：PNG
   - 尺寸：32x32, 64x64, 128x128, 256x256

2. **商店图片**
   - 胶囊图：460x215, 616x353, 374x448
   - 页眉图：460x215
   - 主图：616x353
   - 背景图：1920x1080

3. **游戏截图**
   - 至少 5 张
   - 分辨率：1920x1080
   - 展示核心玩法

4. **预告片**
   - 时长：1-2 分钟
   - 展示游戏特色
   - 上传到 Steam 或 YouTube

#### 商店文本

**简短描述**（300字以内）：
```
三国志：地形策略版是一款创新的策略卡牌游戏，融合经典三国杀玩法与动态地形系统。
强制随机选将打破固有套路，历史羁绊系统让每局都充满惊喜。
2-6人在线对战，单局15-20分钟，快节奏策略体验！

核心特色：
• 动态地形系统 - 12回合制战斗，地形影响战局
• 随机武将池 - 告别固定套路，体验全新组合
• 历史羁绊触发 - 特定武将组合激活隐藏技能
• 多人协作 - 每位玩家控制多个武将，团队配合制胜
```

**详细描述**：参考 `README.md` 内容扩展

#### 价格设置

建议定价策略：
- 基础版：$9.99 USD
- 豪华版（含 DLC）：$19.99 USD
- 首发折扣：20% off

### 服务器部署

#### 使用 Docker Compose（推荐）

```bash
# 克隆项目
git clone <repository-url>
cd sgs/server

# 配置环境变量
cp config/config.example.yaml config/config.yaml
# 编辑 config.yaml 填入数据库密码等信息

# 启动服务
docker-compose up -d

# 查看日志
docker-compose logs -f

# 停止服务
docker-compose down
```

#### 手动部署

```bash
# 安装 Go 1.21+
# 安装 MySQL 8.0+
# 安装 Redis 7+

# 配置数据库
mysql -u root -p -e "CREATE DATABASE sanguo_strategy CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
mysql -u root -p sanguo_strategy < scripts/init_data.sql

# 编译并运行
make build
./bin/server
```

#### 云服务器部署（AWS/阿里云/腾讯云）

推荐配置：
- **测试环境**：2核4G，50GB SSD
- **生产环境**：4核8G，100GB SSD，带宽10Mbps
- **数据库**：独立 RDS 实例
- **缓存**：独立 Redis 实例

```bash
# 安装 Docker
curl -fsSL https://get.docker.com | bash

# 克隆项目
git clone <repository-url>
cd sgs/server

# 配置并启动
docker-compose up -d
```

### 监控与日志

#### 日志配置

服务器日志位置：
- 应用日志：`logs/server.log`
- 错误日志：`logs/error.log`
- 访问日志：`logs/access.log`

#### 监控指标

建议监控：
- CPU 使用率
- 内存使用率
- 数据库连接数
- 并发玩家数
- 平均响应时间
- 错误率

### 更新与维护

#### 热更新流程

1. 服务器更新：
```bash
# 拉取新版本
git pull

# 重新构建
docker-compose build

# 滚动更新（零停机）
docker-compose up -d --no-deps --build server
```

2. 客户端更新：
   - 通过 Steam 自动推送更新
   - 玩家重启客户端即可获取新版本

#### 数据库迁移

```bash
# 备份数据库
pg_dump sanguo_strategy > backup_$(date +%Y%m%d).sql

# 运行迁移脚本
psql sanguo_strategy < migrations/v1.1.0.sql
```

### 常见问题

**Q: Steam API 初始化失败？**
A: 检查 `steam_appid.txt` 是否存在，Steam 客户端是否运行

**Q: 服务器连接失败？**
A: 检查防火墙设置，确保 8080 端口开放

**Q: 数据库连接错误？**
A: 检查 `config.yaml` 中的数据库配置是否正确

**Q: 客户端无法连接服务器？**
A: 检查服务器 URL 配置，确保网络可达

### 技术支持

- 官方网站：[https://sanguo-strategy.com](https://sanguo-strategy.com)
- 开发者邮箱：dev@sanguo-strategy.com
- Discord 社区：[链接]
- GitHub Issues：[链接]

---

*最后更新：2024年1月*

