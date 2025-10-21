using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 背景图片辅助工具
/// 帮助用户快速设置和生成背景图片
/// </summary>
public class BackgroundImageHelper : EditorWindow
{
    private string backgroundPath = "Assets/Resources/UI/Backgrounds/";
    
    [MenuItem("三国策略/背景图片管理器")]
    public static void ShowWindow()
    {
        var window = GetWindow<BackgroundImageHelper>("背景图片管理器");
        window.minSize = new Vector2(500, 600);
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("🎨 背景图片管理器", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "此工具帮助你管理游戏场景的背景图片。\n" +
            "你可以生成默认的纯色背景，或检查已导入的背景图片。",
            MessageType.Info);
        
        GUILayout.Space(15);
        
        // 检查背景图片状态
        EditorGUILayout.LabelField("📋 背景图片状态", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        CheckBackgroundStatus("MainMenu", "主菜单");
        CheckBackgroundStatus("Lobby", "房间列表");
        CheckBackgroundStatus("HeroSelection", "英雄选择");
        CheckBackgroundStatus("MapEditor", "地图编辑器");
        
        GUILayout.Space(20);
        
        // 操作按钮
        EditorGUILayout.LabelField("🛠️ 快速操作", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎨 生成所有默认背景 (三国主题)", GUILayout.Height(40)))
        {
            GenerateAllDefaultBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📁 打开背景目录", GUILayout.Height(30)))
        {
            OpenBackgroundFolder();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 刷新资源", GUILayout.Height(30)))
        {
            AssetDatabase.Refresh();
            Debug.Log("✅ 资源已刷新");
        }
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "💡 提示：\n" +
            "1. 点击「生成所有默认背景」会创建三国主题背景图片\n" +
            "2. 包含战场、城池、竹简等三国元素\n" +
            "3. 如需使用自定义图片，请将图片放入背景目录\n" +
            "4. 图片命名规则：MainMenu.jpg、Lobby.png 等\n" +
            "5. 推荐尺寸：1920x1080\n" +
            "6. 导入后记得设置 Texture Type 为 Sprite (2D and UI)",
            MessageType.None);
    }
    
    void CheckBackgroundStatus(string sceneName, string displayName)
    {
        bool hasJpg = File.Exists($"{backgroundPath}{sceneName}.jpg");
        bool hasPng = File.Exists($"{backgroundPath}{sceneName}.png");
        
        string status = hasJpg || hasPng ? "✅ 已设置" : "❌ 未设置";
        string fileName = hasJpg ? $"{sceneName}.jpg" : (hasPng ? $"{sceneName}.png" : "无");
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"{displayName}:", GUILayout.Width(100));
        
        if (hasJpg || hasPng)
        {
            GUI.color = Color.green;
            EditorGUILayout.LabelField($"{status} ({fileName})");
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField(status);
            GUI.color = Color.white;
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    void GenerateAllDefaultBackgrounds()
    {
        if (!EditorUtility.DisplayDialog("生成三国主题背景",
            "此操作将生成所有场景的三国主题背景图片。\n\n" +
            "已存在的背景文件将被覆盖。\n\n" +
            "是否继续？",
            "生成", "取消"))
        {
            return;
        }
        
        // 确保目录存在
        if (!Directory.Exists(backgroundPath))
        {
            Directory.CreateDirectory(backgroundPath);
        }
        
        // 生成各场景背景
        GenerateSanguoBackground("MainMenu");
        GenerateSanguoBackground("Lobby");
        GenerateSanguoBackground("HeroSelection");
        GenerateSanguoBackground("MapEditor");
        
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("完成", 
            "✅ 三国主题背景已生成！\n\n" +
            "请在场景设置向导中重新生成场景以查看效果。", 
            "确定");
    }
    
    void GenerateSanguoBackground(string sceneName)
    {
        string fullPath = $"{backgroundPath}{sceneName}.png";
        
        int width = 1920;
        int height = 1080;
        
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        // 根据场景类型选择主题
        switch (sceneName)
        {
            case "MainMenu":
                DrawMainMenuBackground(texture, width, height);
                break;
            case "Lobby":
                DrawLobbyBackground(texture, width, height);
                break;
            case "HeroSelection":
                DrawHeroSelectionBackground(texture, width, height);
                break;
            case "MapEditor":
                DrawMapEditorBackground(texture, width, height);
                break;
            default:
                DrawDefaultBackground(texture, width, height);
                break;
        }
        
        texture.Apply();
        
        // 保存为PNG
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);
        
        Debug.Log($"✅ 生成三国主题背景: {sceneName}.png");
        
        DestroyImmediate(texture);
    }
    
    // 主菜单背景 - 古城墙与红旗
    void DrawMainMenuBackground(Texture2D tex, int width, int height)
    {
        // 天空渐变（黄昏效果）
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color skyColor = Color.Lerp(
                new Color(0.35f, 0.25f, 0.20f), // 深橙色地平线
                new Color(0.15f, 0.20f, 0.30f), // 深蓝紫色天空
                t
            );
            
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, skyColor);
            }
        }
        
        // 远山剪影
        DrawMountains(tex, width, height, new Color(0.1f, 0.12f, 0.18f));
        
        // 城墙轮廓
        DrawCityWall(tex, width, height);
        
        // 飘扬的红旗
        DrawFlags(tex, width, height, new Color(0.7f, 0.15f, 0.15f));
        
        // 添加云纹装饰
        DrawCloudPattern(tex, width, height);
    }
    
    // 房间列表背景 - 竹简和书案
    void DrawLobbyBackground(Texture2D tex, int width, int height)
    {
        // 暖色调背景（羊皮纸效果）
        Color paperColor = new Color(0.85f, 0.75f, 0.60f);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f) * 0.1f;
                Color color = paperColor + new Color(noise, noise, noise * 0.5f, 0);
                tex.SetPixel(x, y, color);
            }
        }
        
        // 竹简纹理
        DrawBambooScrolls(tex, width, height);
        
        // 印章装饰
        DrawSeals(tex, width, height);
        
        // 书法笔触效果
        DrawCalligraphyStrokes(tex, width, height);
    }
    
    // 英雄选择背景 - 将旗与营帐
    void DrawHeroSelectionBackground(Texture2D tex, int width, int height)
    {
        // 战场氛围（偏红色调）
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color battleColor = Color.Lerp(
                new Color(0.25f, 0.15f, 0.12f), // 深棕红色地面
                new Color(0.40f, 0.20f, 0.15f), // 红褐色天空
                t
            );
            
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, battleColor);
            }
        }
        
        // 营帐剪影
        DrawCamps(tex, width, height);
        
        // 多彩将旗（魏蜀吴）
        DrawGeneralFlags(tex, width, height);
        
        // 火光效果
        DrawFireGlow(tex, width, height);
    }
    
    // 地图编辑器背景 - 地图卷轴
    void DrawMapEditorBackground(Texture2D tex, int width, int height)
    {
        // 古地图底色
        Color mapColor = new Color(0.75f, 0.70f, 0.60f);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.02f, y * 0.02f) * 0.15f;
                tex.SetPixel(x, y, mapColor + new Color(noise, noise * 0.8f, noise * 0.5f, 0));
            }
        }
        
        // 地图网格
        DrawMapGrid(tex, width, height);
        
        // 山川河流符号
        DrawMapSymbols(tex, width, height);
        
        // 边框装饰
        DrawBorder(tex, width, height);
    }
    
    void DrawDefaultBackground(Texture2D tex, int width, int height)
    {
        Color baseColor = new Color(0.2f, 0.18f, 0.15f);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, baseColor);
            }
        }
    }
    
    // === 绘制辅助函数 ===
    
    void DrawMountains(Texture2D tex, int width, int height, Color color)
    {
        int baseY = height / 3;
        for (int i = 0; i < 5; i++)
        {
            int peakX = Random.Range(width / 6 * i, width / 6 * (i + 1));
            int peakHeight = Random.Range(100, 250);
            
            for (int x = peakX - 300; x < peakX + 300; x++)
            {
                if (x < 0 || x >= width) continue;
                
                int dist = Mathf.Abs(x - peakX);
                int y = baseY + peakHeight - (dist * peakHeight / 300);
                
                for (int yy = 0; yy < y; yy++)
                {
                    if (yy >= 0 && yy < height)
                        tex.SetPixel(x, yy, color);
                }
            }
        }
    }
    
    void DrawCityWall(Texture2D tex, int width, int height)
    {
        Color wallColor = new Color(0.15f, 0.13f, 0.10f);
        int wallHeight = height / 4;
        
        for (int y = 0; y < wallHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 城墙砖块效果
                if ((x / 80 + y / 40) % 2 == 0)
                    tex.SetPixel(x, y, wallColor);
                else
                    tex.SetPixel(x, y, wallColor * 0.9f);
            }
        }
        
        // 城垛
        for (int i = 0; i < 20; i++)
        {
            int x = i * (width / 20);
            for (int y = wallHeight; y < wallHeight + 60; y++)
            {
                for (int xx = x; xx < x + 40; xx++)
                {
                    if (xx < width)
                        tex.SetPixel(xx, y, wallColor * 0.8f);
                }
            }
        }
    }
    
    void DrawFlags(Texture2D tex, int width, int height, Color flagColor)
    {
        for (int i = 0; i < 8; i++)
        {
            int x = 150 + i * 220;
            int y = height / 4 + 60;
            
            // 旗杆
            for (int yy = y; yy < y + 180; yy++)
            {
                for (int xx = x; xx < x + 8; xx++)
                {
                    if (xx < width && yy < height)
                        tex.SetPixel(xx, yy, new Color(0.2f, 0.15f, 0.10f));
                }
            }
            
            // 旗帜（飘动效果）
            for (int yy = 0; yy < 80; yy++)
            {
                int wave = (int)(Mathf.Sin(yy * 0.2f + i) * 15);
                for (int xx = 0; xx < 100; xx++)
                {
                    int px = x + 8 + xx + wave;
                    int py = y + 100 + yy;
                    if (px < width && py < height)
                        tex.SetPixel(px, py, flagColor * (0.8f + Random.Range(-0.1f, 0.1f)));
                }
            }
        }
    }
    
    void DrawCloudPattern(Texture2D tex, int width, int height)
    {
        Color cloudColor = new Color(0.25f, 0.22f, 0.28f, 0.3f);
        for (int i = 0; i < 5; i++)
        {
            int cx = Random.Range(200, width - 200);
            int cy = Random.Range(height * 2 / 3, height - 100);
            
            for (int y = -50; y < 50; y++)
            {
                for (int x = -100; x < 100; x++)
                {
                    float dist = Mathf.Sqrt(x * x / 4 + y * y) / 50f;
                    if (dist < 1f)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < height)
                        {
                            Color current = tex.GetPixel(px, py);
                            tex.SetPixel(px, py, Color.Lerp(current, cloudColor, 0.3f * (1 - dist)));
                        }
                    }
                }
            }
        }
    }
    
    void DrawBambooScrolls(Texture2D tex, int width, int height)
    {
        Color bambooColor = new Color(0.60f, 0.50f, 0.35f);
        
        // 绘制3-4个竹简
        for (int i = 0; i < 4; i++)
        {
            int x = 300 + i * 400;
            int y = height / 4;
            
            // 竹简主体
            for (int yy = y; yy < y + 600; yy++)
            {
                for (int xx = x; xx < x + 80; xx++)
                {
                    if (xx < width && yy < height)
                    {
                        float stripe = Mathf.Sin((yy - y) * 0.5f) * 0.1f;
                        Color color = bambooColor + new Color(stripe, stripe, stripe * 0.5f, 0);
                        tex.SetPixel(xx, yy, color);
                    }
                }
            }
            
            // 竹节
            for (int j = 0; j < 5; j++)
            {
                int nodeY = y + j * 120;
                for (int xx = x; xx < x + 80; xx++)
                {
                    for (int yy = nodeY; yy < nodeY + 6; yy++)
                    {
                        if (xx < width && yy < height)
                            tex.SetPixel(xx, yy, bambooColor * 0.7f);
                    }
                }
            }
        }
    }
    
    void DrawSeals(Texture2D tex, int width, int height)
    {
        Color sealColor = new Color(0.8f, 0.2f, 0.2f);
        
        // 右上角印章
        int x = width - 200;
        int y = height - 200;
        
        for (int yy = -40; yy < 40; yy++)
        {
            for (int xx = -40; xx < 40; xx++)
            {
                if (Mathf.Abs(xx) + Mathf.Abs(yy) < 45)
                {
                    int px = x + xx;
                    int py = y + yy;
                    if (px >= 0 && px < width && py >= 0 && py < height)
                        tex.SetPixel(px, py, sealColor * (0.6f + Random.Range(-0.1f, 0.2f)));
                }
            }
        }
    }
    
    void DrawCalligraphyStrokes(Texture2D tex, int width, int height)
    {
        Color inkColor = new Color(0.2f, 0.15f, 0.12f, 0.4f);
        
        // 绘制几笔书法效果
        for (int i = 0; i < 3; i++)
        {
            int x = 200 + i * 600;
            int y = height / 2;
            
            for (int t = 0; t < 100; t++)
            {
                int px = x + t * 3;
                int py = y + (int)(Mathf.Sin(t * 0.2f) * 30);
                
                for (int r = -5; r < 5; r++)
                {
                    if (px >= 0 && px < width && py + r >= 0 && py + r < height)
                    {
                        Color current = tex.GetPixel(px, py + r);
                        tex.SetPixel(px, py + r, Color.Lerp(current, inkColor, 0.3f));
                    }
                }
            }
        }
    }
    
    void DrawCamps(Texture2D tex, int width, int height)
    {
        Color tentColor = new Color(0.15f, 0.12f, 0.10f);
        
        for (int i = 0; i < 6; i++)
        {
            int x = 200 + i * 280;
            int y = height / 5;
            
            // 三角形帐篷
            for (int yy = 0; yy < 150; yy++)
            {
                int w = (150 - yy) / 2;
                for (int xx = -w; xx < w; xx++)
                {
                    int px = x + xx;
                    int py = y + yy;
                    if (px >= 0 && px < width && py >= 0 && py < height)
                        tex.SetPixel(px, py, tentColor);
                }
            }
        }
    }
    
    void DrawGeneralFlags(Texture2D tex, int width, int height)
    {
        Color[] flagColors = {
            new Color(0.2f, 0.4f, 0.7f),  // 蓝色（魏）
            new Color(0.7f, 0.3f, 0.2f),  // 红色（蜀）
            new Color(0.3f, 0.6f, 0.3f)   // 绿色（吴）
        };
        
        for (int i = 0; i < 3; i++)
        {
            int x = 400 + i * 500;
            int y = height / 3;
            
            // 旗杆
            for (int yy = y; yy < y + 300; yy++)
            {
                for (int xx = x; xx < x + 12; xx++)
                {
                    if (xx < width && yy < height)
                        tex.SetPixel(xx, yy, new Color(0.15f, 0.12f, 0.10f));
                }
            }
            
            // 旗帜
            for (int yy = 0; yy < 120; yy++)
            {
                int wave = (int)(Mathf.Sin(yy * 0.15f) * 20);
                for (int xx = 0; xx < 150; xx++)
                {
                    int px = x + 12 + xx + wave;
                    int py = y + 50 + yy;
                    if (px < width && py < height)
                        tex.SetPixel(px, py, flagColors[i] * (0.7f + Random.Range(0f, 0.3f)));
                }
            }
        }
    }
    
    void DrawFireGlow(Texture2D tex, int width, int height)
    {
        Color fireColor = new Color(1.0f, 0.4f, 0.1f, 0.3f);
        
        for (int i = 0; i < 4; i++)
        {
            int x = 300 + i * 450;
            int y = height / 6;
            
            for (int yy = -60; yy < 60; yy++)
            {
                for (int xx = -60; xx < 60; xx++)
                {
                    float dist = Mathf.Sqrt(xx * xx + yy * yy) / 60f;
                    if (dist < 1f)
                    {
                        int px = x + xx;
                        int py = y + yy;
                        if (px >= 0 && px < width && py >= 0 && py < height)
                        {
                            Color current = tex.GetPixel(px, py);
                            float intensity = (1 - dist) * 0.4f * Random.Range(0.7f, 1.0f);
                            tex.SetPixel(px, py, Color.Lerp(current, fireColor, intensity));
                        }
                    }
                }
            }
        }
    }
    
    void DrawMapGrid(Texture2D tex, int width, int height)
    {
        Color gridColor = new Color(0.5f, 0.45f, 0.35f);
        
        // 横线
        for (int i = 0; i < 12; i++)
        {
            int y = 100 + i * 80;
            for (int x = 100; x < width - 100; x++)
            {
                if (y < height)
                    tex.SetPixel(x, y, gridColor);
            }
        }
        
        // 竖线
        for (int i = 0; i < 20; i++)
        {
            int x = 100 + i * 90;
            for (int y = 100; y < height - 100; y++)
            {
                if (x < width)
                    tex.SetPixel(x, y, gridColor);
            }
        }
    }
    
    void DrawMapSymbols(Texture2D tex, int width, int height)
    {
        Color symbolColor = new Color(0.3f, 0.35f, 0.25f);
        
        // 绘制一些"山"字符号
        for (int i = 0; i < 8; i++)
        {
            int x = Random.Range(200, width - 200);
            int y = Random.Range(200, height - 200);
            
            // 简化的山形符号
            for (int t = -30; t < 30; t++)
            {
                int h = 30 - Mathf.Abs(t);
                for (int yy = 0; yy < h; yy++)
                {
                    int px = x + t;
                    int py = y + yy;
                    if (px >= 0 && px < width && py >= 0 && py < height)
                        tex.SetPixel(px, py, symbolColor);
                }
            }
        }
    }
    
    void DrawBorder(Texture2D tex, int width, int height)
    {
        Color borderColor = new Color(0.4f, 0.35f, 0.25f);
        int borderWidth = 20;
        
        // 上下边框
        for (int x = 0; x < width; x++)
        {
            for (int i = 0; i < borderWidth; i++)
            {
                tex.SetPixel(x, i, borderColor);
                tex.SetPixel(x, height - 1 - i, borderColor);
            }
        }
        
        // 左右边框
        for (int y = 0; y < height; y++)
        {
            for (int i = 0; i < borderWidth; i++)
            {
                tex.SetPixel(i, y, borderColor);
                tex.SetPixel(width - 1 - i, y, borderColor);
            }
        }
        
        // 四角装饰
        DrawCornerDecoration(tex, borderWidth, borderWidth, borderColor);
        DrawCornerDecoration(tex, width - borderWidth * 4, borderWidth, borderColor);
        DrawCornerDecoration(tex, borderWidth, height - borderWidth * 4, borderColor);
        DrawCornerDecoration(tex, width - borderWidth * 4, height - borderWidth * 4, borderColor);
    }
    
    void DrawCornerDecoration(Texture2D tex, int x, int y, Color color)
    {
        for (int i = -20; i < 20; i++)
        {
            for (int j = -20; j < 20; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) < 25)
                {
                    int px = x + i;
                    int py = y + j;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, color * 1.2f);
                }
            }
        }
    }
    
    void OpenBackgroundFolder()
    {
        string fullPath = Path.GetFullPath(backgroundPath);
        
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        
        EditorUtility.RevealInFinder(fullPath);
    }
}

