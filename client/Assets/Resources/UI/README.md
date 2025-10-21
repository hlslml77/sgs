# UI资源文件夹

此文件夹用于存放UI相关的资源文件。

## 建议的文件结构

```
UI/
├── Backgrounds/        # 背景图片
│   ├── MainMenu.png
│   ├── RoomList.png
│   ├── HeroSelection.png
│   └── GameScene.png
│
├── Buttons/           # 按钮图片
│   ├── Normal/
│   ├── Hover/
│   └── Pressed/
│
├── Icons/            # 图标
│   ├── Coins.png
│   ├── Level.png
│   └── Settings.png
│
├── Fonts/            # 字体文件
│   ├── Title.ttf
│   └── Body.ttf
│
├── Sounds/           # UI音效
│   ├── ButtonHover.wav
│   └── ButtonClick.wav
│
└── Particles/        # 粒子特效
    └── GlowParticle.png
```

## 获取免费资源

### 背景图片
1. **Unsplash** - https://unsplash.com/
   - 搜索: "ancient china", "battlefield", "strategy"
   
2. **Pixabay** - https://pixabay.com/
   - 免费无版权图片

3. **ArtStation** - https://www.artstation.com/
   - 搜索三国主题作品（需注意版权）

### 字体
1. **Google Fonts** - https://fonts.google.com/
   - Noto Sans SC (思源黑体)
   - Noto Serif SC (思源宋体)

2. **免费商用中文字体**
   - 站酷系列字体
   - 思源黑体
   - 阿里巴巴普惠体

### 音效
1. **Freesound** - https://freesound.org/
   - 搜索: "button", "click", "ui"

2. **OpenGameArt** - https://opengameart.org/
   - 免费游戏音效

### UI图标
1. **Flaticon** - https://www.flaticon.com/
   - 大量免费图标

2. **Font Awesome** - https://fontawesome.com/
   - 矢量图标

## 使用说明

1. 将下载的资源文件放入相应文件夹
2. 在Unity中刷新资源 (右键 -> Reimport)
3. 在BackgroundManager或ButtonEnhancer中引用这些资源

## 图片规格建议

### 背景图片
- 分辨率: 1920x1080 或更高
- 格式: PNG (支持透明) 或 JPG
- 文件大小: < 5MB

### 按钮图片
- 分辨率: 256x128 或 512x256
- 格式: PNG (支持透明)
- 文件大小: < 500KB

### 图标
- 分辨率: 128x128 或 256x256
- 格式: PNG (支持透明)
- 文件大小: < 100KB

### 音效
- 格式: WAV 或 OGG
- 采样率: 44100 Hz
- 时长: < 1秒（按钮音效）
- 文件大小: < 500KB

