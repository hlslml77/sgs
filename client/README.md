# 三国志：地形策略版 - Unity 客户端

## 项目结构

```
Assets/
├── Scripts/
│   ├── Network/          # 网络通信
│   │   ├── NetworkManager.cs
│   │   └── ApiClient.cs
│   ├── Game/             # 游戏核心
│   │   ├── GameManager.cs
│   │   ├── BoardManager.cs
│   │   ├── CardManager.cs
│   │   ├── GeneralController.cs
│   │   └── TerrainController.cs
│   ├── UI/               # 用户界面
│   │   └── UIManager.cs
│   └── Steam/            # Steam 集成
│       └── SteamManager.cs
├── Scenes/               # 游戏场景
│   ├── MainMenu.unity
│   ├── GameScene.unity
│   └── RoomList.unity
├── Prefabs/              # 预制体
│   ├── General.prefab
│   ├── Terrain.prefab
│   ├── Card.prefab
│   └── Tile.prefab
├── Materials/            # 材质
├── Textures/             # 贴图
├── Plugins/              # 插件
│   └── Steamworks.NET/
└── Resources/            # 资源
    ├── Generals/
    ├── Terrains/
    └── Cards/
```

## 开发环境

### 要求

- Unity 2021.3 LTS 或更高版本
- Visual Studio 2022 或 Rider
- .NET Framework 4.8

### 依赖插件

1. **Steamworks.NET**
   - Steam API 集成
   - [下载地址](https://steamworks.github.io/)

2. **Newtonsoft.Json**
   - JSON 序列化/反序列化
   - 通过 Package Manager 安装

3. **WebSocketSharp**
   - WebSocket 通信
   - 手动导入或通过 NuGet

4. **TextMeshPro**
   - UI 文本渲染
   - Unity 自带

## 开始开发

### 1. 克隆项目

```bash
git clone <repository-url>
cd sgs/client
```

### 2. 打开项目

1. 启动 Unity Hub
2. 添加项目：选择 `client` 目录
3. 选择 Unity 版本：2021.3 LTS
4. 打开项目

### 3. 安装依赖

1. 打开 Unity Package Manager (Window > Package Manager)
2. 安装以下包：
   - Newtonsoft Json (com.unity.nuget.newtonsoft-json)
   - Input System (com.unity.inputsystem)
   - TextMeshPro

3. 手动导入 Steamworks.NET：
   - 下载最新版本
   - 解压到 `Assets/Plugins/Steamworks.NET/`

### 4. 配置服务器地址

编辑 `Assets/Scripts/Network/NetworkManager.cs`：
```csharp
[SerializeField] private string serverUrl = "ws://localhost:8080/ws";
```

编辑 `Assets/Scripts/Network/ApiClient.cs`：
```csharp
[SerializeField] private string apiBaseUrl = "http://localhost:8080/api/v1";
```

### 5. 配置 Steam

1. 在项目根目录创建 `steam_appid.txt`：
   ```
   480
   ```

2. 编辑 `SteamManager.cs` 中的 App ID（正式发布时）

## 场景说明

### MainMenu (主菜单场景)

主要功能：
- 快速匹配
- 自定义房间
- 玩家资料
- 设置选项

关键脚本：
- `MainMenuController.cs`
- `UIManager.cs`

### GameScene (游戏场景)

主要功能：
- 3D 棋盘显示
- 武将/地形渲染
- 手牌显示
- 技能界面
- 回合信息

关键脚本：
- `GameManager.cs`
- `BoardManager.cs`
- `CardManager.cs`

### RoomList (房间列表场景)

主要功能：
- 显示可用房间
- 创建自定义房间
- 房间筛选

关键脚本：
- `RoomListController.cs`

## 预制体说明

### General.prefab (武将)

组件：
- `GeneralController.cs` - 武将逻辑
- `Animator` - 动画控制
- `TextMeshPro` - 显示名称和血量

### Terrain.prefab (地形)

组件：
- `TerrainController.cs` - 地形逻辑
- `ParticleSystem` - 特效
- `MeshRenderer` - 渲染

### Card.prefab (卡牌)

组件：
- `CardController.cs` - 卡牌逻辑
- `Image` - 卡牌图片
- `Button` - 点击交互

## 网络通信

### WebSocket 消息格式

```json
{
  "type": "game_action",
  "room_id": "uuid",
  "data": {
    "action_type": "play_card",
    "card_id": "uuid",
    "target_ids": ["uuid1", "uuid2"]
  }
}
```

### HTTP API 调用

```csharp
// 登录
StartCoroutine(ApiClient.Instance.Login(username, password, (success, response) => {
    if (success) {
        Debug.Log("Login successful");
    }
}));

// 获取房间列表
StartCoroutine(ApiClient.Instance.GetRoomList((success, response) => {
    if (success) {
        var data = JsonConvert.DeserializeObject<RoomListResponse>(response);
        // 处理房间列表
    }
}));
```

## 构建发布

### Windows 平台

```
1. File > Build Settings
2. Platform: PC, Mac & Linux Standalone
3. Target Platform: Windows
4. Architecture: x86_64
5. Build
```

构建后文件：
```
Build/
├── SanguoStrategy.exe
├── SanguoStrategy_Data/
├── UnityCrashHandler64.exe
├── UnityPlayer.dll
└── steam_appid.txt  # 手动复制
```

### macOS 平台

```
1. File > Build Settings
2. Platform: PC, Mac & Linux Standalone
3. Target Platform: macOS
4. Architecture: x86_64 + ARM64 (Apple Silicon)
5. Build
```

### Linux 平台

```
1. File > Build Settings
2. Platform: PC, Mac & Linux Standalone
3. Target Platform: Linux
4. Architecture: x86_64
5. Build
```

## 测试

### 本地测试

1. 启动本地服务器：
```bash
cd ../server
make run
```

2. 在 Unity 中运行游戏（Play 模式）

3. 测试功能：
   - 登录/注册
   - 创建/加入房间
   - 游戏流程

### Steam 测试

1. 确保 Steam 客户端运行
2. 使用测试 App ID (480)
3. 在 Unity 中构建并运行
4. 验证 Steam 功能：
   - 登录状态
   - 成就解锁
   - 好友列表

## 性能优化

### 建议

1. **对象池**
   - 武将、地形、卡牌使用对象池
   - 减少 Instantiate/Destroy 调用

2. **批处理**
   - 合并相同材质的模型
   - 使用 GPU Instancing

3. **LOD 系统**
   - 远距离使用低模
   - 近距离使用高模

4. **异步加载**
   - 使用 Addressables 系统
   - 异步加载资源

## 常见问题

**Q: WebSocket 连接失败？**
A: 检查服务器地址和端口，确保服务器正在运行

**Q: Steam API 初始化失败？**
A: 确保 Steam 客户端运行，`steam_appid.txt` 存在

**Q: 编译错误？**
A: 确保所有依赖插件已正确导入

**Q: 性能问题？**
A: 检查对象数量，启用性能分析器（Profiler）

## 开发指南

### 添加新武将

1. 在服务器端 `init_data.sql` 添加武将数据
2. 创建武将预制体和材质
3. 在 `GeneralController.cs` 中添加技能逻辑

### 添加新地形

1. 在服务器端 `init_data.sql` 添加地形数据
2. 创建地形预制体和特效
3. 在 `TerrainController.cs` 中添加效果逻辑

### 添加新UI

1. 在场景中创建 Canvas
2. 使用 TextMeshPro 组件
3. 编写 UI 控制脚本
4. 在 `UIManager.cs` 中注册

## 贡献

欢迎提交 PR 和 Issue！

### 代码规范

- 使用 C# 命名约定
- 添加 XML 注释
- 保持代码简洁

## 许可证

本项目仅用于学习和演示目的。

---

*Happy Coding!* 🎮

