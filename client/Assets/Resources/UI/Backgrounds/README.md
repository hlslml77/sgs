# 背景图片资源目录

这个目录用于存放游戏各场景的背景图片。

## 支持的场景背景

场景设置向导会自动为以下场景加载背景图片：

1. **主菜单** - `MainMenu.jpg` 或 `MainMenu.png`
2. **房间列表** - `Lobby.jpg` 或 `Lobby.png`
3. **英雄选择** - `HeroSelection.jpg` 或 `HeroSelection.png`
4. **地图编辑器** - `MapEditor.jpg` 或 `MapEditor.png`

## 使用方法

1. 将背景图片放入此目录
2. 确保文件名与上述列表中的名称匹配
3. 推荐图片尺寸：1920x1080 或更高
4. 支持格式：JPG、PNG

## 图片导入设置

在Unity中选中背景图片后，请确保设置：
- **Texture Type**: Sprite (2D and UI)
- **Max Size**: 2048 或更高（根据实际图片大小）
- **Compression**: 根据需要选择（推荐 High Quality）

## 说明

- 如果没有提供背景图片，场景将使用默认的渐变色背景
- 背景图片会以半透明方式显示，确保UI元素清晰可见
- 你可以在 `SceneSetupWizard.cs` 的 `LoadBackgroundSprite` 方法中修改加载逻辑

## 推荐背景风格

根据场景主题，建议使用：
- **主菜单**: 三国战场、城池远景、磅礴大气的场景
- **房间列表**: 议事厅、军营、战略地图
- **英雄选择**: 英雄群像、武将特写、气势恢宏的背景
- **地图编辑器**: 简洁的网格背景、地图纹理

## 免费素材推荐

你可以从以下网站获取免费的游戏背景素材：
- Pixabay (https://pixabay.com/)
- Unsplash (https://unsplash.com/)
- OpenGameArt (https://opengameart.org/)
- Kenney.nl (https://kenney.nl/)

