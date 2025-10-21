using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// 商业级场景UI生成向导
/// 自动为所有场景添加完整的商业化UI设计
/// </summary>
public class SceneSetupWizard : EditorWindow
{
    private Vector2 scrollPosition;
    private bool[] sceneStatus = new bool[6]; // 增加到6个场景（包含登录）
    private Font chineseFont; // 旧版UI Text使用
    private TMP_FontAsset tmpChineseFont; // TextMeshPro使用

    [MenuItem("三国策略/场景设置向导")]
    public static void ShowWindow()
    {
        var window = GetWindow<SceneSetupWizard>("场景设置向导");
        window.minSize = new Vector2(500, 600);
        window.Show();
    }

    void OnEnable()
    {
        LoadChineseFont();
    }

    void LoadTMPChineseFont()
    {
        // 尝试加载生成的 TMP 中文字体
        string[] possiblePaths = new string[]
        {
            "Assets/Fonts/TMP/SGSA_ChineseFont.asset",  // 优先路径（与UltimateFontFixer一致）
            "Assets/Resources/Fonts/SGSA_ChineseFont.asset",
            "Assets/TextMesh Pro/Resources/Fonts & Materials/SGSA_ChineseFont.asset",
            "Assets/Resources/SGSA_ChineseFont.asset"
        };
        
        foreach (string path in possiblePaths)
        {
            tmpChineseFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (tmpChineseFont != null)
            {
                Debug.Log($"✅ 成功加载TMP中文字体: {path}");
                return;
            }
        }
        
        Debug.LogWarning("⚠️ 未找到TMP中文字体，请先运行：工具 → ⚡ 终极字体修复（推荐）");
    }

    void LoadChineseFont()
    {
        // 首先尝试加载 TMP 中文字体（优先）
        LoadTMPChineseFont();
        
        // 方法1：尝试从EditorPrefs加载保存的字体设置
        if (EditorPrefs.HasKey("ChineseFont_Name"))
        {
            bool isSystemFont = EditorPrefs.GetBool("ChineseFont_IsSystemFont", true);
            if (isSystemFont)
            {
                string fontName = EditorPrefs.GetString("ChineseFont_Name", "Microsoft YaHei");
                try
                {
                    chineseFont = Font.CreateDynamicFontFromOSFont(fontName, 14);
                    if (chineseFont != null && !string.IsNullOrEmpty(chineseFont.name))
                    {
                        Debug.Log($"✅ 从设置加载字体: {fontName}");
                        return;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"加载保存的字体失败: {ex.Message}");
                }
            }
            else
            {
                string fontPath = EditorPrefs.GetString("ChineseFont_Path", "");
                chineseFont = Resources.Load<Font>(fontPath);
                if (chineseFont != null)
                {
                    Debug.Log($"✅ 从Resources加载字体: {fontPath}");
                    return;
                }
            }
        }

        // 方法2：尝试多个系统字体
        string[] fontNames = { 
            "msyh.ttc",      // Microsoft YaHei 文件名
            "msyhbd.ttc",    // Microsoft YaHei Bold
            "simhei.ttf",    // SimHei
            "simsun.ttc",    // SimSun
            "Microsoft YaHei", 
            "SimHei", 
            "Arial Unicode MS",
            "微软雅黑",
            "黑体"
        };
        
        foreach (string name in fontNames)
        {
            try
            {
                Font font = Font.CreateDynamicFontFromOSFont(name, 14);
                if (font != null && !string.IsNullOrEmpty(font.name))
                {
                    // 验证字体是否真的可用
                    chineseFont = font;
                    Debug.Log($"✅ 成功加载系统字体: {name} (实际名称: {font.name})");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                // 继续尝试下一个
                Debug.Log($"尝试字体 {name} 失败: {ex.Message}");
            }
        }

        // 方法3：使用Unity内置的Arial (不支持中文，但至少不会崩溃)
        chineseFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (chineseFont != null)
        {
            Debug.LogWarning("⚠️ 未找到中文字体，使用Arial字体。中文可能显示为方块。");
            Debug.LogWarning("⚠️ 请运行：三国策略 → 修复中文字体显示");
        }
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        
        // 创建支持中文的样式
        var titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        if (chineseFont != null)
        {
            titleStyle.font = chineseFont;
        }
        
        var labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.fontSize = 12;
        labelStyle.wordWrap = true;
        if (chineseFont != null)
        {
            labelStyle.font = chineseFont;
        }
        
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 12;
        if (chineseFont != null)
        {
            buttonStyle.font = chineseFont;
        }
        
        var bigButtonStyle = new GUIStyle(GUI.skin.button);
        bigButtonStyle.fontSize = 14;
        bigButtonStyle.fontStyle = FontStyle.Bold;
        if (chineseFont != null)
        {
            bigButtonStyle.font = chineseFont;
        }
        
        // 标题
        GUILayout.Label("🎨 三国策略 - 场景UI生成器", titleStyle);
        GUILayout.Space(10);
        
        // 显示字体状态和调试信息
        if (chineseFont != null)
        {
            GUILayout.Label("✅ 中文字体已就绪！", labelStyle);
            GUILayout.Label($"字体名称: {chineseFont.name}", labelStyle);
            GUILayout.Label($"字体大小: {chineseFont.fontSize}", labelStyle);
            GUILayout.Label($"是否动态字体: {chineseFont.dynamic}", labelStyle);
            
            if (GUILayout.Button("重新加载字体", buttonStyle, GUILayout.Height(25)))
            {
                chineseFont = null;
                LoadChineseFont();
            }
        }
        else
        {
            GUILayout.Label("⚠️ 未检测到中文字体，文字可能显示为方块", labelStyle);
            GUILayout.Label("DEBUG: chineseFont 是 null", labelStyle);
            
            if (GUILayout.Button("🔧 重新尝试加载字体", buttonStyle, GUILayout.Height(30)))
            {
                LoadChineseFont();
            }
            
        }
        
        GUILayout.Space(10);
        
        // 说明文字
        GUILayout.Label("此工具会为场景添加完整的商业化UI设计：", labelStyle);
        GUILayout.Label("• 现代渐变背景（支持自定义背景图）", labelStyle);
        GUILayout.Label("• 卡片式面板布局", labelStyle);
        GUILayout.Label("• 带图标的按钮", labelStyle);
        GUILayout.Label("• 自动应用中文字体", labelStyle);
        
        GUILayout.Space(10);
        
        // TextMeshPro 中文字体处理
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("📝 TextMeshPro 中文字体", EditorStyles.boldLabel);
        
        var tmpFont = FindTMPChineseFont();
        if (tmpFont != null)
        {
            GUILayout.Label($"✅ 已找到TMP中文字体: {tmpFont.name}", labelStyle);
            
            if (GUILayout.Button("🔄 更新所有场景的TMP字体", buttonStyle, GUILayout.Height(30)))
            {
                ApplyTMPFontToAllScenes(tmpFont);
            }
        }
        else
        {
            GUILayout.Label("⚠️ 未找到TMP中文字体！游戏运行时中文会显示为方块", labelStyle);
            
            if (GUILayout.Button("🚀 立即生成TMP中文字体", bigButtonStyle, GUILayout.Height(35)))
            {
                GenerateTMPChineseFont();
            }
            
            GUILayout.Label("提示：生成字体需要1-2分钟，请耐心等待", labelStyle);
        }
        
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(10);
        
        // 背景图片管理按钮
        if (GUILayout.Button("🎨 背景图片管理器", buttonStyle, GUILayout.Height(30)))
        {
            BackgroundImageHelper.ShowWindow();
        }

        GUILayout.Space(20);

        DrawSection("步骤1：一键设置所有场景（推荐）", labelStyle);
        
        if (GUILayout.Button("🚀 一键设置所有6个场景", bigButtonStyle, GUILayout.Height(40)))
        {
            SetupAllScenes();
        }

        GUILayout.Space(20);

        DrawSection("步骤2：单独设置场景", labelStyle);
        
        DrawSceneButton("登录场景 (Login)", "Login", 0);
        DrawSceneButton("主菜单 (MainMenu)", "MainMenu", 1);
        DrawSceneButton("房间列表 (RoomList)", "RoomList", 2);
        DrawSceneButton("选将界面 (HeroSelection)", "HeroSelection", 3);
        DrawSceneButton("游戏场景 (GameScene)", "GameScene", 4);
        DrawSceneButton("地形编辑器 (TerrainEditor)", "TerrainEditor", 5);

        GUILayout.Space(20);

        DrawSection("高级操作", labelStyle);
        GUILayout.Label("⚠️ 清空场景会删除所有GameObject（保留Camera和Light）", labelStyle);
        
        if (GUILayout.Button("清空当前场景", buttonStyle, GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("确认清空", "确定要清空当前场景吗？", "确定", "取消"))
            {
                ClearCurrentScene();
            }
        }

        GUILayout.Space(20);

        EditorGUILayout.EndScrollView();
    }

    void DrawSection(string title, GUIStyle labelStyle)
    {
        GUILayout.Space(10);
        var sectionStyle = new GUIStyle(EditorStyles.boldLabel);
        sectionStyle.fontSize = 13;
        sectionStyle.fontStyle = FontStyle.Bold;
        if (chineseFont != null)
        {
            sectionStyle.font = chineseFont;
        }
        GUILayout.Label(title, sectionStyle);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
    }

    void DrawSceneButton(string displayName, string sceneName, int index)
    {
        EditorGUILayout.BeginHorizontal();
        string statusIcon = sceneStatus[index] ? "✅" : "⚪";
        
        var buttonStyle = new GUIStyle(GUI.skin.button);
        if (chineseFont != null)
        {
            buttonStyle.font = chineseFont;
            buttonStyle.fontSize = 12;
        }
        
        if (GUILayout.Button($"{statusIcon} 设置 {displayName}", buttonStyle, GUILayout.Height(30)))
        {
            SetupScene(sceneName);
            sceneStatus[index] = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    void SetupAllScenes()
    {
        if (!EditorUtility.DisplayDialog("确认设置", 
            "将为所有6个场景创建完整的商业化UI结构。\n这可能需要1-2分钟。\n\n确定继续吗？", 
            "确定", "取消"))
        {
            return;
        }

        string[] scenes = { "Login", "MainMenu", "RoomList", "HeroSelection", "GameScene", "TerrainEditor" };
        int successCount = 0;
        int failCount = 0;
        string errorLog = "";
        
        try
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                try
                {
                    float progress = (float)i / scenes.Length;
                    EditorUtility.DisplayProgressBar("设置场景", $"正在设置 {scenes[i]}... ({i + 1}/{scenes.Length})", progress);
                    
                    Debug.Log($"========== 开始设置场景 {i + 1}/{scenes.Length}: {scenes[i]} ==========");
                    
                    bool success = SetupScene(scenes[i]);
                    
                    if (success)
                    {
                        sceneStatus[i] = true;
                        successCount++;
                        Debug.Log($"✅ 场景 {scenes[i]} 设置成功");
                    }
                    else
                    {
                        failCount++;
                        errorLog += $"- {scenes[i]}: 设置失败\n";
                        Debug.LogError($"❌ 场景 {scenes[i]} 设置失败");
                    }
                }
                catch (System.Exception ex)
                {
                    failCount++;
                    errorLog += $"- {scenes[i]}: {ex.Message}\n";
                    Debug.LogError($"❌ 场景 {scenes[i]} 发生错误: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        string resultMessage = $"场景设置完成！\n\n✅ 成功: {successCount} 个\n❌ 失败: {failCount} 个";
        if (failCount > 0)
        {
            resultMessage += $"\n\n失败详情:\n{errorLog}";
            EditorUtility.DisplayDialog("完成（有错误）", resultMessage, "确定");
        }
        else
        {
            resultMessage += "\n\n现在可以打开任意场景查看效果！";
            EditorUtility.DisplayDialog("完成", resultMessage, "确定");
        }
        
        Debug.Log($"========== 场景设置总结 ==========");
        Debug.Log($"成功: {successCount}, 失败: {failCount}");
    }

    bool SetupScene(string sceneName)
    {
        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        
        try
        {
            if (!System.IO.File.Exists(scenePath))
            {
                Debug.LogError($"❌ 场景文件不存在: {scenePath}");
                return false;
            }
            
            Debug.Log($"📂 正在打开场景: {scenePath}");
            
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            if (!scene.IsValid())
            {
                Debug.LogError($"❌ 场景无效: {sceneName}");
                return false;
            }
            
            Debug.Log($"🔧 开始设置场景内容: {sceneName}");

            switch (sceneName)
            {
                case "Login":
                    SetupLoginScene();
                    break;
                case "MainMenu":
                    SetupMainMenuScene();
                    break;
                case "RoomList":
                    SetupRoomListScene();
                    break;
                case "HeroSelection":
                    SetupHeroSelectionScene();
                    break;
                case "GameScene":
                    SetupGameScene();
                    break;
                case "TerrainEditor":
                    SetupTerrainEditorScene();
                    break;
                default:
                    Debug.LogError($"❌ 未知场景类型: {sceneName}");
                    return false;
            }

            Debug.Log($"💾 正在保存场景: {sceneName}");
            
            bool saved = EditorSceneManager.SaveScene(scene);
            
            if (!saved)
            {
                Debug.LogError($"❌ 场景保存失败: {sceneName}");
                return false;
            }
            
            Debug.Log($"✅ 场景设置完成: {sceneName}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 设置场景 {sceneName} 时发生异常: {ex.Message}");
            Debug.LogError($"堆栈跟踪: {ex.StackTrace}");
            return false;
        }
    }

    #region 登录场景设置
    void SetupLoginScene()
    {
        // 确保字体已加载
        if (chineseFont == null)
        {
            LoadChineseFont();
        }
        
        var canvas = CreateCanvas("LoginCanvas");
        
        // ========== 背景层 ==========
        var bgContainer = CreatePanel(canvas.transform, "BackgroundContainer", Color.clear);
        SetAnchor(bgContainer, AnchorPresets.Stretch, Vector2.zero);
        
        // 深色三国主题背景
        var darkBG = CreatePanel(bgContainer.transform, "DarkBackground", new Color(0.08f, 0.05f, 0.02f));
        SetAnchor(darkBG, AnchorPresets.Stretch, Vector2.zero);
        
        // 顶部装饰渐变（金色）
        var topGradient = CreatePanel(bgContainer.transform, "TopGradient", new Color(0.4f, 0.3f, 0.1f, 0.3f));
        SetSize(topGradient, 1920, 400);
        SetAnchor(topGradient, AnchorPresets.TopStretch, Vector2.zero);
        
        // 背景装饰图片
        var bgImagePlaceholder = CreatePanel(bgContainer.transform, "BackgroundImagePlaceholder", new Color(0.1f, 0.08f, 0.05f, 0.5f));
        SetAnchor(bgImagePlaceholder, AnchorPresets.Stretch, Vector2.zero);
        
        var bgImage = bgImagePlaceholder.GetComponent<Image>();
        var bgSprite = LoadBackgroundSprite("Login");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.color = new Color(1f, 1f, 1f, 0.4f);
        }
        else
        {
            var bgNote = CreateText(bgImagePlaceholder.transform, "Note", "💡 提示：可在 Assets/Resources/UI/Backgrounds/ 添加 Login.jpg/png 作为背景图", 18);
            SetAnchor(bgNote, AnchorPresets.MiddleCenter, Vector2.zero);
            SetTextAlignment(bgNote, TextAnchor.MiddleCenter);
            SetTextColor(bgNote, new Color(0.5f, 0.4f, 0.3f));
        }
        
        // ========== 游戏LOGO/标题 ==========
        var logoPanel = CreateCardPanel(canvas.transform, "LogoPanel", new Color(0.15f, 0.1f, 0.05f, 0.85f), 700, 180);
        SetAnchor(logoPanel, AnchorPresets.TopCenter, new Vector2(0, -60));
        
        var titleMain = CreateText(logoPanel.transform, "TitleMain", "三国策略", 68);
        SetAnchor(titleMain, AnchorPresets.MiddleCenter, new Vector2(0, 15));
        SetTextOutline(titleMain, new Color(0.8f, 0.6f, 0.2f));
        SetTextColor(titleMain, new Color(1f, 0.9f, 0.7f));
        
        var titleSub = CreateText(titleMain.transform, "TitleSub", "Three Kingdoms Strategy", 22);
        SetAnchor(titleSub, AnchorPresets.BottomCenter, new Vector2(0, -40));
        SetTextColor(titleSub, new Color(0.7f, 0.6f, 0.4f));
        
        // ========== 登录面板 ==========
        var loginPanel = CreateCardPanel(canvas.transform, "LoginPanel", new Color(0.12f, 0.10f, 0.08f, 0.95f), 480, 420);
        SetAnchor(loginPanel, AnchorPresets.MiddleCenter, new Vector2(0, -60));
        
        var panelTitle = CreateText(loginPanel.transform, "PanelTitle", "欢迎登录", 32);
        SetAnchor(panelTitle, AnchorPresets.TopCenter, new Vector2(0, -30));
        SetTextColor(panelTitle, new Color(1f, 0.95f, 0.8f));
        
        // 用户名输入框
        var usernameInput = CreateStyledInputField(loginPanel.transform, "UsernameInput", "请输入用户名", new Vector2(0, -100));
        SetSize(usernameInput, 400, 55);
        
        var usernameLabel = CreateText(loginPanel.transform, "UsernameLabel", "用户名", 18);
        SetAnchor(usernameLabel, AnchorPresets.TopLeft, new Vector2(40, -75));
        SetTextColor(usernameLabel, new Color(0.9f, 0.85f, 0.7f));
        
        // 密码输入框
        var passwordInput = CreateStyledInputField(loginPanel.transform, "PasswordInput", "请输入密码", new Vector2(0, -190));
        SetSize(passwordInput, 400, 55);
        var passwordInputField = passwordInput.GetComponent<InputField>();
        if (passwordInputField != null)
        {
            passwordInputField.contentType = InputField.ContentType.Password;
        }
        
        var passwordLabel = CreateText(loginPanel.transform, "PasswordLabel", "密码", 18);
        SetAnchor(passwordLabel, AnchorPresets.TopLeft, new Vector2(40, -165));
        SetTextColor(passwordLabel, new Color(0.9f, 0.85f, 0.7f));
        
        // 记住密码选项
        var rememberPanel = CreatePanel(loginPanel.transform, "RememberPanel", Color.clear);
        SetSize(rememberPanel, 400, 30);
        SetAnchor(rememberPanel, AnchorPresets.TopCenter, new Vector2(0, -255));
        
        var rememberCheckbox = CreatePanel(rememberPanel.transform, "Checkbox", new Color(0.3f, 0.25f, 0.2f));
        SetSize(rememberCheckbox, 20, 20);
        SetAnchor(rememberCheckbox, AnchorPresets.MiddleLeft, new Vector2(0, 0));
        
        var rememberText = CreateText(rememberPanel.transform, "RememberText", "记住密码", 16);
        SetAnchor(rememberText, AnchorPresets.MiddleLeft, new Vector2(30, 0));
        SetTextColor(rememberText, new Color(0.8f, 0.75f, 0.6f));
        
        // 登录按钮
        var loginBtn = CreateGradientButton(loginPanel.transform, "LoginBtn", "登录", 
            new Color(0.3f, 0.6f, 0.8f), new Color(0.2f, 0.4f, 0.6f), new Vector2(0, -310), new Vector2(400, 60));
        AddButtonOnClick(loginBtn, "LoginController", "OnLogin");
        
        // 底部链接
        var linkPanel = CreatePanel(loginPanel.transform, "LinkPanel", Color.clear);
        SetSize(linkPanel, 400, 30);
        SetAnchor(linkPanel, AnchorPresets.BottomCenter, new Vector2(0, 20));
        
        var registerBtn = CreateText(linkPanel.transform, "RegisterLink", "注册账号", 16);
        SetAnchor(registerBtn, AnchorPresets.MiddleLeft, Vector2.zero);
        SetTextColor(registerBtn, new Color(0.5f, 0.7f, 0.9f));
        
        var forgetBtn = CreateText(linkPanel.transform, "ForgetLink", "忘记密码", 16);
        SetAnchor(forgetBtn, AnchorPresets.MiddleRight, Vector2.zero);
        SetTextColor(forgetBtn, new Color(0.5f, 0.7f, 0.9f));
        
        // ========== 注册面板（默认隐藏） ==========
        var registerPanel = CreateCardPanel(canvas.transform, "RegisterPanel", new Color(0.12f, 0.10f, 0.08f, 0.95f), 480, 550);
        SetAnchor(registerPanel, AnchorPresets.MiddleCenter, new Vector2(0, -60));
        registerPanel.SetActive(false);
        
        var regPanelTitle = CreateText(registerPanel.transform, "PanelTitle", "注册新账号", 32);
        SetAnchor(regPanelTitle, AnchorPresets.TopCenter, new Vector2(0, -30));
        SetTextColor(regPanelTitle, new Color(1f, 0.95f, 0.8f));
        
        // 注册表单
        var regUsernameLabel = CreateText(registerPanel.transform, "UsernameLabel", "用户名", 18);
        SetAnchor(regUsernameLabel, AnchorPresets.TopLeft, new Vector2(40, -85));
        SetTextColor(regUsernameLabel, new Color(0.9f, 0.85f, 0.7f));
        
        var regUsernameInput = CreateStyledInputField(registerPanel.transform, "UsernameInput", "请输入用户名（4-20字符）", new Vector2(0, -110));
        SetSize(regUsernameInput, 400, 55);
        
        var regEmailLabel = CreateText(registerPanel.transform, "EmailLabel", "邮箱", 18);
        SetAnchor(regEmailLabel, AnchorPresets.TopLeft, new Vector2(40, -175));
        SetTextColor(regEmailLabel, new Color(0.9f, 0.85f, 0.7f));
        
        var regEmailInput = CreateStyledInputField(registerPanel.transform, "EmailInput", "请输入邮箱地址", new Vector2(0, -200));
        SetSize(regEmailInput, 400, 55);
        
        var regPasswordLabel = CreateText(registerPanel.transform, "PasswordLabel", "密码", 18);
        SetAnchor(regPasswordLabel, AnchorPresets.TopLeft, new Vector2(40, -265));
        SetTextColor(regPasswordLabel, new Color(0.9f, 0.85f, 0.7f));
        
        var regPasswordInput = CreateStyledInputField(registerPanel.transform, "PasswordInput", "请输入密码（6-20字符）", new Vector2(0, -290));
        SetSize(regPasswordInput, 400, 55);
        var regPasswordInputField = regPasswordInput.GetComponent<InputField>();
        if (regPasswordInputField != null)
        {
            regPasswordInputField.contentType = InputField.ContentType.Password;
        }
        
        var regConfirmLabel = CreateText(registerPanel.transform, "ConfirmLabel", "确认密码", 18);
        SetAnchor(regConfirmLabel, AnchorPresets.TopLeft, new Vector2(40, -355));
        SetTextColor(regConfirmLabel, new Color(0.9f, 0.85f, 0.7f));
        
        var regConfirmInput = CreateStyledInputField(registerPanel.transform, "ConfirmPasswordInput", "请再次输入密码", new Vector2(0, -380));
        SetSize(regConfirmInput, 400, 55);
        var regConfirmInputField = regConfirmInput.GetComponent<InputField>();
        if (regConfirmInputField != null)
        {
            regConfirmInputField.contentType = InputField.ContentType.Password;
        }
        
        // 注册按钮
        var registerBtn2 = CreateGradientButton(registerPanel.transform, "RegisterBtn", "注册", 
            new Color(0.3f, 0.7f, 0.4f), new Color(0.2f, 0.5f, 0.3f), new Vector2(0, -460), new Vector2(400, 60));
        AddButtonOnClick(registerBtn2, "LoginController", "OnRegister");
        
        // 返回登录
        var backToLoginText = CreateText(registerPanel.transform, "BackToLogin", "已有账号？返回登录", 16);
        SetAnchor(backToLoginText, AnchorPresets.BottomCenter, new Vector2(0, 20));
        SetTextColor(backToLoginText, new Color(0.5f, 0.7f, 0.9f));
        
        // ========== 消息提示框（默认隐藏） ==========
        var messagePanel = CreateCardPanel(canvas.transform, "MessagePanel", new Color(0.15f, 0.12f, 0.08f, 0.95f), 400, 100);
        SetAnchor(messagePanel, AnchorPresets.TopCenter, new Vector2(0, -280));
        messagePanel.SetActive(false);
        
        var messageText = CreateText(messagePanel.transform, "MessageText", "提示信息", 20);
        SetAnchor(messageText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(messageText, new Color(1f, 1f, 1f));
        SetTextAlignment(messageText, TextAnchor.MiddleCenter);
        
        // ========== 加载动画（默认隐藏） ==========
        var loadingPanel = CreatePanel(canvas.transform, "LoadingPanel", new Color(0, 0, 0, 0.7f));
        SetAnchor(loadingPanel, AnchorPresets.Stretch, Vector2.zero);
        loadingPanel.SetActive(false);
        
        var loadingCard = CreateCardPanel(loadingPanel.transform, "LoadingCard", new Color(0.12f, 0.10f, 0.08f, 0.95f), 300, 150);
        SetAnchor(loadingCard, AnchorPresets.MiddleCenter, Vector2.zero);
        
        var loadingText = CreateText(loadingCard.transform, "LoadingText", "正在登录...", 24);
        SetAnchor(loadingText, AnchorPresets.MiddleCenter, new Vector2(0, -20));
        SetTextColor(loadingText, new Color(0.9f, 0.85f, 0.7f));
        
        var loadingDots = CreateText(loadingCard.transform, "LoadingDots", "...", 32);
        SetAnchor(loadingDots, AnchorPresets.MiddleCenter, new Vector2(0, 20));
        SetTextColor(loadingDots, new Color(0.7f, 0.6f, 0.4f));
        
        // ========== 底部版本信息 ==========
        var versionText = CreateText(canvas.transform, "VersionText", "v1.0.0 | 三国策略游戏", 14);
        SetAnchor(versionText, AnchorPresets.BottomCenter, new Vector2(0, 15));
        SetTextColor(versionText, new Color(0.5f, 0.45f, 0.35f));
        
        // ========== 服务器状态指示器 ==========
        var statusPanel = CreatePanel(canvas.transform, "StatusPanel", new Color(0.1f, 0.08f, 0.05f, 0.8f));
        SetSize(statusPanel, 180, 40);
        SetAnchor(statusPanel, AnchorPresets.TopLeft, new Vector2(20, -20));
        
        var statusDot = CreatePanel(statusPanel.transform, "StatusDot", new Color(0.3f, 0.8f, 0.3f));
        SetSize(statusDot, 12, 12);
        SetAnchor(statusDot, AnchorPresets.MiddleLeft, new Vector2(15, 0));
        
        var statusText = CreateText(statusPanel.transform, "StatusText", "服务器在线", 16);
        SetAnchor(statusText, AnchorPresets.MiddleLeft, new Vector2(35, 0));
        SetTextColor(statusText, new Color(0.7f, 0.9f, 0.7f));
        
        CreateEventSystem();
        
        Debug.Log("✅ 登录场景UI创建完成");
    }
    #endregion

    #region 主菜单场景设置
    void SetupMainMenuScene()
    {
        // 确保字体已加载
        if (chineseFont == null)
        {
            LoadChineseFont();
        }
        
        var canvas = CreateCanvas("MainMenuCanvas");
        
        // 创建渐变背景层
        var bgContainer = CreatePanel(canvas.transform, "BackgroundContainer", Color.clear);
        SetAnchor(bgContainer, AnchorPresets.Stretch, Vector2.zero);
        
        // 深色渐变背景
        var darkBG = CreatePanel(bgContainer.transform, "DarkBackground", new Color(0.05f, 0.08f, 0.12f));
        SetAnchor(darkBG, AnchorPresets.Stretch, Vector2.zero);
        
        // 顶部渐变装饰
        var topGradient = CreatePanel(bgContainer.transform, "TopGradient", new Color(0.15f, 0.25f, 0.35f, 0.3f));
        SetSize(topGradient, 1920, 400);
        SetAnchor(topGradient, AnchorPresets.TopStretch, Vector2.zero);
        
        // 底部渐变装饰
        var bottomGradient = CreatePanel(bgContainer.transform, "BottomGradient", new Color(0.1f, 0.15f, 0.2f, 0.5f));
        SetSize(bottomGradient, 1920, 300);
        SetAnchor(bottomGradient, AnchorPresets.BottomStretch, Vector2.zero);
        
        // 背景装饰图片 - 尝试加载背景图
        var bgImagePlaceholder = CreatePanel(bgContainer.transform, "BackgroundImagePlaceholder", new Color(0.1f, 0.15f, 0.2f, 0.2f));
        SetAnchor(bgImagePlaceholder, AnchorPresets.Stretch, Vector2.zero);
        
        var bgImage = bgImagePlaceholder.GetComponent<Image>();
        var bgSprite = LoadBackgroundSprite("MainMenu");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.color = new Color(1f, 1f, 1f, 0.6f); // 半透明效果
        }
        else
        {
            // 没有图片时显示提示
            var bgNote = CreateText(bgImagePlaceholder.transform, "Note", "💡 提示：可在 Assets/UI/Backgrounds/ 添加背景图\n支持文件名: MainMenu.jpg/png", 18);
            SetAnchor(bgNote, AnchorPresets.MiddleCenter, Vector2.zero);
            SetTextAlignment(bgNote, TextAnchor.MiddleCenter);
            SetTextColor(bgNote, new Color(0.4f, 0.4f, 0.5f));
        }
        
        // ========== 主内容层 ==========
        
        // 游戏LOGO/标题区域
        var logoPanel = CreateCardPanel(canvas.transform, "LogoPanel", new Color(0.1f, 0.15f, 0.25f, 0.8f), 800, 200);
        SetAnchor(logoPanel, AnchorPresets.TopCenter, new Vector2(0, -80));
        
        var titleMain = CreateText(logoPanel.transform, "TitleMain", "三国策略", 72);
        SetAnchor(titleMain, AnchorPresets.MiddleCenter, new Vector2(0, 20));
        SetTextOutline(titleMain, new Color(1f, 0.8f, 0.3f));
        
        var titleSub = CreateText(titleMain.transform, "TitleSub", "地形玩法版", 28);
        SetAnchor(titleSub, AnchorPresets.BottomCenter, new Vector2(0, -45));
        SetTextColor(titleSub, new Color(0.8f, 0.8f, 1f));
        
        // 玩家信息卡片（左上角）
        var playerCard = CreateCardPanel(canvas.transform, "PlayerInfoCard", new Color(0.12f, 0.15f, 0.22f, 0.95f), 350, 180);
        SetAnchor(playerCard, AnchorPresets.TopLeft, new Vector2(30, -30));
        
        var playerIcon = CreateIconPlaceholder(playerCard.transform, "PlayerIcon", new Color(0.3f, 0.6f, 0.9f), 80, 80);
        SetAnchor(playerIcon, AnchorPresets.TopLeft, new Vector2(20, -20));
        
        var playerNameText = CreateText(playerCard.transform, "PlayerName", "玩家昵称", 24);
        SetAnchor(playerNameText, AnchorPresets.TopLeft, new Vector2(120, -25));
        SetTextColor(playerNameText, new Color(1f, 1f, 1f));
        
        var playerLevelBadge = CreateBadge(playerCard.transform, "LevelBadge", "Lv.1", new Color(0.8f, 0.5f, 0.2f));
        SetAnchor(playerLevelBadge, AnchorPresets.TopLeft, new Vector2(120, -60));
        
        var coinsPanel = CreateInfoRow(playerCard.transform, "CoinsRow", "💰", "欢乐豆: 1000", new Vector2(20, -120));
        var vipPanel = CreateInfoRow(playerCard.transform, "VIPRow", "👑", "VIP等级: 0", new Vector2(20, -150));
        
        // 公告栏卡片（右上角）
        var noticeCard = CreateCardPanel(canvas.transform, "NoticeCard", new Color(0.12f, 0.15f, 0.22f, 0.95f), 450, 180);
        SetAnchor(noticeCard, AnchorPresets.TopRight, new Vector2(-30, -30));
        
        var noticeTitle = CreateText(noticeCard.transform, "NoticeTitle", "📢 游戏公告", 24);
        SetAnchor(noticeTitle, AnchorPresets.TopLeft, new Vector2(20, -20));
        SetTextColor(noticeTitle, new Color(1f, 0.9f, 0.3f));
        
        var noticeContent = CreateText(noticeCard.transform, "NoticeContent", "欢迎来到三国策略！\n地形玩法全新体验", 18);
        SetAnchor(noticeContent, AnchorPresets.TopLeft, new Vector2(20, -60));
        SetTextColor(noticeContent, new Color(0.9f, 0.9f, 0.9f));
        SetTextAlignment(noticeContent, TextAnchor.UpperLeft);
        
        // 中央主按钮区域
        var mainButtonArea = CreatePanel(canvas.transform, "MainButtonArea", Color.clear);
        SetSize(mainButtonArea, 500, 700);
        SetAnchor(mainButtonArea, AnchorPresets.MiddleCenter, new Vector2(0, -50));
        
        // 创建大型主按钮
        float btnY = 250;
        float btnSpacing = 90;
        
        var quickMatchBtn = CreateGradientButton(mainButtonArea.transform, "QuickMatchBtn", "⚔️ 快速匹配", 
            new Color(0.2f, 0.7f, 0.3f), new Color(0.15f, 0.5f, 0.2f), new Vector2(0, btnY), new Vector2(480, 70));
        AddButtonOnClick(quickMatchBtn, "MainMenuController", "OnQuickMatch");
        
        var roomListBtn = CreateGradientButton(mainButtonArea.transform, "RoomListBtn", "🏛️ 房间列表", 
            new Color(0.25f, 0.5f, 0.95f), new Color(0.15f, 0.35f, 0.7f), new Vector2(0, btnY - btnSpacing), new Vector2(480, 70));
        AddButtonOnClick(roomListBtn, "MainMenuController", "OnRoomList");
        
        var profileBtn = CreateGradientButton(mainButtonArea.transform, "ProfileBtn", "👤 玩家资料", 
            new Color(0.8f, 0.5f, 0.2f), new Color(0.6f, 0.35f, 0.1f), new Vector2(0, btnY - btnSpacing * 2), new Vector2(480, 70));
        AddButtonOnClick(profileBtn, "MainMenuController", "OnProfile");
        
        var shopBtn = CreateGradientButton(mainButtonArea.transform, "ShopBtn", "🛒 商店", 
            new Color(0.9f, 0.6f, 0.2f), new Color(0.7f, 0.4f, 0.1f), new Vector2(0, btnY - btnSpacing * 3), new Vector2(480, 70));
        AddButtonOnClick(shopBtn, "MainMenuController", "OnShop");
        
        var settingsBtn = CreateGradientButton(mainButtonArea.transform, "SettingsBtn", "⚙️ 设置", 
            new Color(0.5f, 0.5f, 0.5f), new Color(0.3f, 0.3f, 0.3f), new Vector2(0, btnY - btnSpacing * 4), new Vector2(480, 70));
        AddButtonOnClick(settingsBtn, "MainMenuController", "OnSettings");
        
        var quitBtn = CreateGradientButton(mainButtonArea.transform, "QuitBtn", "🚪 退出游戏", 
            new Color(0.7f, 0.25f, 0.25f), new Color(0.5f, 0.15f, 0.15f), new Vector2(0, btnY - btnSpacing * 5), new Vector2(480, 70));
        AddButtonOnClick(quitBtn, "MainMenuController", "OnQuit");
        
        // 底部社交/辅助功能栏
        var bottomBar = CreatePanel(canvas.transform, "BottomBar", new Color(0.08f, 0.1f, 0.15f, 0.9f));
        SetSize(bottomBar, 1920, 80);
        SetAnchor(bottomBar, AnchorPresets.BottomStretch, Vector2.zero);
        
        var friendBtn = CreateIconTextButton(bottomBar.transform, "FriendBtn", "👥", "好友",
            new Color(0.25f, 0.5f, 0.8f), new Vector2(100, 0), new Vector2(120, 60));
        var mailBtn = CreateIconTextButton(bottomBar.transform, "MailBtn", "✉️", "邮件",
            new Color(0.6f, 0.3f, 0.8f), new Vector2(240, 0), new Vector2(120, 60));
        var rankBtn = CreateIconTextButton(bottomBar.transform, "RankBtn", "🏆", "排行",
            new Color(0.9f, 0.6f, 0.2f), new Vector2(380, 0), new Vector2(120, 60));
        var achievementBtn = CreateIconTextButton(bottomBar.transform, "AchievementBtn", "🎖️", "成就",
            new Color(0.2f, 0.7f, 0.3f), new Vector2(520, 0), new Vector2(120, 60));
        
        var versionText = CreateText(bottomBar.transform, "Version", "v1.0.0 Alpha", 14);
        SetAnchor(versionText, AnchorPresets.MiddleRight, new Vector2(-30, 0));
        SetTextColor(versionText, new Color(0.5f, 0.5f, 0.5f));
        
        // 左下角状态指示器
        var statusIndicator = CreatePanel(bottomBar.transform, "StatusIndicator", Color.clear);
        SetSize(statusIndicator, 200, 60);
        SetAnchor(statusIndicator, AnchorPresets.MiddleLeft, new Vector2(30, 0));
        
        var onlineDot = CreatePanel(statusIndicator.transform, "OnlineDot", new Color(0.2f, 0.8f, 0.3f));
        SetSize(onlineDot, 12, 12);
        SetAnchor(onlineDot, AnchorPresets.MiddleLeft, Vector2.zero);
        
        var statusText = CreateText(statusIndicator.transform, "StatusText", "服务器在线", 16);
        SetAnchor(statusText, AnchorPresets.MiddleLeft, new Vector2(20, 0));
        SetTextColor(statusText, new Color(0.7f, 0.9f, 0.7f));
        
        CreateEventSystem();
    }
    #endregion

    #region 房间列表场景设置
    void SetupRoomListScene()
    {
        // 确保字体已加载
        if (chineseFont == null)
        {
            LoadChineseFont();
        }
        
        var canvas = CreateCanvas("RoomListCanvas");
        
        // 背景层
        var bgContainer = CreatePanel(canvas.transform, "BackgroundContainer", Color.clear);
        SetAnchor(bgContainer, AnchorPresets.Stretch, Vector2.zero);
        
        var darkBG = CreatePanel(bgContainer.transform, "DarkBackground", new Color(0.06f, 0.08f, 0.12f));
        SetAnchor(darkBG, AnchorPresets.Stretch, Vector2.zero);
        
        // 尝试加载背景图
        var bgImage = darkBG.GetComponent<Image>();
        var bgSprite = LoadBackgroundSprite("Lobby");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.color = new Color(1f, 1f, 1f, 0.5f); // 半透明显示背景图
        }
        
        // 顶部导航栏
        var topNav = CreatePanel(canvas.transform, "TopNavBar", new Color(0.1f, 0.12f, 0.18f, 0.95f));
        SetSize(topNav, 1920, 100);
        SetAnchor(topNav, AnchorPresets.TopStretch, Vector2.zero);
        
        var backBtn = CreateIconTextButton(topNav.transform, "BackBtn", "←", "返回", 
            new Color(0.6f, 0.25f, 0.25f), new Vector2(80, 0), new Vector2(140, 60));
        AddButtonOnClick(backBtn, "RoomListController", "OnBack");
        
        var titleText = CreateText(topNav.transform, "Title", "房间列表", 40);
        SetAnchor(titleText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(titleText, new Color(1f, 0.95f, 0.8f));
        SetTextOutline(titleText, new Color(0.3f, 0.3f, 0.4f));
        
        var refreshBtn = CreateIconTextButton(topNav.transform, "RefreshBtn", "🔄", "刷新", 
            new Color(0.25f, 0.5f, 0.8f), new Vector2(-300, 0), new Vector2(140, 60));
        AddButtonOnClick(refreshBtn, "RoomListController", "OnRefresh");
        
        // 筛选/搜索栏
        var filterBar = CreateCardPanel(canvas.transform, "FilterBar", new Color(0.12f, 0.14f, 0.20f, 0.9f), 1200, 80);
        SetAnchor(filterBar, AnchorPresets.TopCenter, new Vector2(0, -120));
        
        var searchInput = CreateStyledInputField(filterBar.transform, "SearchInput", "🔍 搜索房间名称...", new Vector2(-400, 0));
        
        var filterAllBtn = CreateTabButton(filterBar.transform, "FilterAll", "全部", true, new Vector2(-100, 0));
        AddButtonOnClick(filterAllBtn, "RoomListController", "OnFilterAll");
        var filterWaitingBtn = CreateTabButton(filterBar.transform, "FilterWaiting", "等待中", false, new Vector2(0, 0));
        AddButtonOnClick(filterWaitingBtn, "RoomListController", "OnFilterWaiting");
        var filterPlayingBtn = CreateTabButton(filterBar.transform, "FilterPlaying", "游戏中", false, new Vector2(100, 0));
        AddButtonOnClick(filterPlayingBtn, "RoomListController", "OnFilterPlaying");
        
        var createRoomBtnTop = CreateGradientButton(filterBar.transform, "CreateRoomBtn", "+ 创建房间", 
            new Color(0.2f, 0.7f, 0.3f), new Color(0.15f, 0.5f, 0.2f), new Vector2(450, 0), new Vector2(180, 60));
        AddButtonOnClick(createRoomBtnTop, "RoomListController", "OnCreateRoom");
        
        // 房间列表滚动区域
        var scrollBG = CreateCardPanel(canvas.transform, "RoomListBackground", new Color(0.08f, 0.10f, 0.15f, 0.8f), 1200, 700);
        SetAnchor(scrollBG, AnchorPresets.MiddleCenter, new Vector2(0, -50));
        
        var scrollView = CreateScrollView(scrollBG.transform, "RoomScrollView");
        SetSize(scrollView, 1160, 660);
        SetAnchor(scrollView, AnchorPresets.MiddleCenter, Vector2.zero);
        
        // 创建示例房间卡片
        var content = scrollView.transform.Find("Viewport/Content");
        for (int i = 0; i < 5; i++)
        {
            var roomCard = CreateRoomCard(content, $"RoomCard{i}", i);
        }
        
        // 底部操作栏
        var bottomBar = CreatePanel(canvas.transform, "BottomBar", new Color(0.10f, 0.12f, 0.18f, 0.9f));
        SetSize(bottomBar, 1920, 100);
        SetAnchor(bottomBar, AnchorPresets.BottomStretch, Vector2.zero);
        
        var createRoomBtn = CreateGradientButton(bottomBar.transform, "CreateRoomBtn", "➕ 创建房间", 
            new Color(0.3f, 0.7f, 0.4f), new Color(0.2f, 0.5f, 0.3f), new Vector2(0, 5), new Vector2(350, 70));
        AddButtonOnClick(createRoomBtn, "RoomListController", "OnCreateRoom");
        
        var quickMatchBtn = CreateGradientButton(bottomBar.transform, "QuickMatchBtn", "⚔️ 快速匹配", 
            new Color(0.9f, 0.5f, 0.2f), new Color(0.7f, 0.35f, 0.1f), new Vector2(400, 5), new Vector2(250, 60));
        AddButtonOnClick(quickMatchBtn, "RoomListController", "OnQuickMatch");
        
        // 在线人数显示
        var onlinePanel = CreatePanel(bottomBar.transform, "OnlinePanel", new Color(0.15f, 0.20f, 0.25f, 0.8f));
        SetSize(onlinePanel, 200, 60);
        SetAnchor(onlinePanel, AnchorPresets.MiddleLeft, new Vector2(30, 0));
        
        var onlineIcon = CreateText(onlinePanel.transform, "OnlineIcon", "👥", 24);
        SetAnchor(onlineIcon, AnchorPresets.MiddleLeft, new Vector2(20, 0));
        
        var onlineText = CreateText(onlinePanel.transform, "OnlineCount", "在线: 42人", 18);
        SetAnchor(onlineText, AnchorPresets.MiddleLeft, new Vector2(60, 0));
        SetTextColor(onlineText, new Color(0.3f, 0.9f, 0.3f));
        
        // 创建房间对话框（默认隐藏）
        var createDialog = CreateDialog(canvas.transform, "CreateRoomDialog", "创建房间", 600, 500);
        createDialog.SetActive(false);
        
        var dialogContent = createDialog.transform.Find("DialogPanel");
        
        CreateStyledInputField(dialogContent, "RoomNameInput", "房间名称 (最多20字)", new Vector2(0, 120));
        CreateStyledInputField(dialogContent, "PasswordInput", "密码 (可选)", new Vector2(0, 40));
        CreateStyledInputField(dialogContent, "MaxPlayersInput", "最大人数 (2-6)", new Vector2(0, -40));
        
        var confirmBtn = CreateGradientButton(dialogContent, "ConfirmBtn", "✔️ 确认创建", 
            new Color(0.2f, 0.7f, 0.3f), new Color(0.15f, 0.5f, 0.2f), new Vector2(0, -150), new Vector2(250, 60));
        
        var cancelBtn = CreateGradientButton(dialogContent, "CancelBtn", "✖️ 取消", 
            new Color(0.6f, 0.3f, 0.3f), new Color(0.4f, 0.2f, 0.2f), new Vector2(0, -220), new Vector2(250, 60));
        
        CreateEventSystem();
    }
    #endregion

    #region 选将场景设置
    void SetupHeroSelectionScene()
    {
        // 确保字体已加载
        if (chineseFont == null)
        {
            LoadChineseFont();
        }
        
        var canvas = CreateCanvas("HeroSelectionCanvas");
        
        // 背景
        var bgContainer = CreatePanel(canvas.transform, "BackgroundContainer", Color.clear);
        SetAnchor(bgContainer, AnchorPresets.Stretch, Vector2.zero);
        
        var darkBG = CreatePanel(bgContainer.transform, "DarkBackground", new Color(0.08f, 0.06f, 0.10f));
        SetAnchor(darkBG, AnchorPresets.Stretch, Vector2.zero);
        
        // 尝试加载背景图
        var bgImage = darkBG.GetComponent<Image>();
        var bgSprite = LoadBackgroundSprite("HeroSelection");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.color = new Color(1f, 1f, 1f, 0.4f); // 半透明显示背景图
        }
        
        // 顶部信息栏
        var topBar = CreatePanel(canvas.transform, "TopBar", new Color(0.1f, 0.08f, 0.15f, 0.95f));
        SetSize(topBar, 1920, 100);
        SetAnchor(topBar, AnchorPresets.TopStretch, Vector2.zero);
        
        var countdownCard = CreateCardPanel(topBar.transform, "CountdownCard", new Color(0.8f, 0.3f, 0.2f, 0.8f), 200, 70);
        SetAnchor(countdownCard, AnchorPresets.MiddleLeft, new Vector2(50, 0));
        var countdownText = CreateText(countdownCard.transform, "Countdown", "⏱️ 30s", 32);
        SetAnchor(countdownText, AnchorPresets.MiddleCenter, Vector2.zero);
        
        var titleText = CreateText(topBar.transform, "Title", "选择你的武将", 36);
        SetAnchor(titleText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(titleText, new Color(1f, 0.9f, 0.7f));
        
        var selectedCard = CreateCardPanel(topBar.transform, "SelectedCard", new Color(0.2f, 0.6f, 0.3f, 0.8f), 220, 70);
        SetAnchor(selectedCard, AnchorPresets.MiddleRight, new Vector2(-50, 0));
        var selectedText = CreateText(selectedCard.transform, "SelectedCount", "✓ 已选择: 0/4", 28);
        SetAnchor(selectedText, AnchorPresets.MiddleCenter, Vector2.zero);
        
        // 中央选将区域
        var selectionArea = CreatePanel(canvas.transform, "SelectionArea", Color.clear);
        SetSize(selectionArea, 1800, 800);
        SetAnchor(selectionArea, AnchorPresets.MiddleCenter, new Vector2(0, -30));
        
        // 职能分类标签栏
        var roleTabBar = CreatePanel(selectionArea.transform, "RoleTabBar", new Color(0.12f, 0.10f, 0.18f, 0.8f));
        SetSize(roleTabBar, 1800, 60);
        SetAnchor(roleTabBar, AnchorPresets.TopCenter, Vector2.zero);
        
        string[] roleNames = { "输出型", "控制型", "辅助型", "特殊型" };
        Color[] roleColors = {
            new Color(0.9f, 0.3f, 0.2f),
            new Color(0.3f, 0.5f, 0.95f),
            new Color(0.3f, 0.8f, 0.3f),
            new Color(0.8f, 0.3f, 0.9f)
        };
        
        for (int i = 0; i < 4; i++)
        {
            float x = -675 + i * 450;
            var roleTab = CreateRoleTab(roleTabBar.transform, $"RoleTab{i}", roleNames[i], roleColors[i], new Vector2(x, 0));
        }
        
        // 武将卡片网格
        var heroGrid = CreatePanel(selectionArea.transform, "HeroGrid", new Color(0.08f, 0.08f, 0.12f, 0.6f));
        SetSize(heroGrid, 1760, 660);
        SetAnchor(heroGrid, AnchorPresets.BottomCenter, new Vector2(0, 20));
        
        // 创建武将卡片 (4x3布局)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                int index = row * 4 + col;
                float x = -660 + col * 440;
                float y = 210 - row * 220;
                var heroCard = CreateHeroCard(heroGrid.transform, $"HeroCard{index}", new Vector2(x, y), roleColors[col]);
            }
        }
        
        // 底部操作栏
        var bottomBar = CreatePanel(canvas.transform, "BottomBar", new Color(0.1f, 0.08f, 0.15f, 0.95f));
        SetSize(bottomBar, 1920, 100);
        SetAnchor(bottomBar, AnchorPresets.BottomStretch, Vector2.zero);
        
        var randomBtn = CreateGradientButton(bottomBar.transform, "RandomBtn", "🎲 随机选择", 
            new Color(0.7f, 0.5f, 0.2f), new Color(0.5f, 0.3f, 0.1f), new Vector2(-200, 0), new Vector2(220, 70));
        AddButtonOnClick(randomBtn, "HeroSelectionController", "OnRandomSelect");
        
        var confirmBtn = CreateGradientButton(bottomBar.transform, "ConfirmBtn", "✔️ 确认选择", 
            new Color(0.2f, 0.7f, 0.3f), new Color(0.15f, 0.5f, 0.2f), new Vector2(200, 0), new Vector2(220, 70));
        AddButtonOnClick(confirmBtn, "HeroSelectionController", "OnConfirm");
        
        // 武将详情面板（右侧，默认隐藏）
        var detailPanel = CreateCardPanel(canvas.transform, "HeroDetailPanel", new Color(0.1f, 0.1f, 0.15f, 0.98f), 450, 900);
        SetAnchor(detailPanel, AnchorPresets.MiddleRight, new Vector2(-20, 0));
        detailPanel.SetActive(false);
        
        var detailTitle = CreateText(detailPanel.transform, "HeroName", "武将名称", 32);
        SetAnchor(detailTitle, AnchorPresets.TopCenter, new Vector2(0, -30));
        
        var heroPortrait = CreateIconPlaceholder(detailPanel.transform, "HeroPortrait", new Color(0.3f, 0.3f, 0.4f), 300, 300);
        SetAnchor(heroPortrait, AnchorPresets.TopCenter, new Vector2(0, -80));
        
        var heroStatsPanel = CreatePanel(detailPanel.transform, "HeroStats", new Color(0.08f, 0.08f, 0.12f, 0.8f));
        SetSize(heroStatsPanel, 340, 180);
        SetAnchor(heroStatsPanel, AnchorPresets.BottomCenter, new Vector2(0, 30));
        
        var statsTitle = CreateText(heroStatsPanel.transform, "StatsTitle", "武将属性", 20);
        SetAnchor(statsTitle, AnchorPresets.TopCenter, new Vector2(0, -15));
        
        string[] statNames = { "体力: ★★★☆☆", "攻击: ★★★★☆", "防御: ★★☆☆☆", "技能: ★★★★★" };
        for (int i = 0; i < statNames.Length; i++)
        {
            var statText = CreateText(heroStatsPanel.transform, $"Stat{i}", statNames[i], 16);
            SetAnchor(statText, AnchorPresets.TopLeft, new Vector2(20, -50 - i * 30));
            SetTextColor(statText, new Color(0.9f, 0.9f, 0.7f));
            SetTextAlignment(statText, TextAnchor.MiddleLeft);
        }
        
        // 底部确认按钮
        var confirmPanel = CreatePanel(canvas.transform, "ConfirmPanel", new Color(0.08f, 0.10f, 0.15f, 0.95f));
        SetSize(confirmPanel, 1920, 90);
        SetAnchor(confirmPanel, AnchorPresets.BottomStretch, Vector2.zero);
        
        var confirmBtn2 = CreateGradientButton(confirmPanel.transform, "ConfirmBtn", "✔️ 确认选择", 
            new Color(0.2f, 0.7f, 0.3f), new Color(0.15f, 0.5f, 0.2f), new Vector2(0, 0), new Vector2(400, 65));
        AddButtonOnClick(confirmBtn2, "HeroSelectionController", "OnConfirm");
        
        var randomBtn2 = CreateGradientButton(confirmPanel.transform, "RandomBtn", "🎲 随机选择", 
            new Color(0.7f, 0.5f, 0.2f), new Color(0.5f, 0.35f, 0.1f), new Vector2(-450, 0), new Vector2(250, 55));
        AddButtonOnClick(randomBtn2, "HeroSelectionController", "OnRandomSelect");
        
        CreateEventSystem();
    }
    #endregion

    #region 游戏场景设置
    void SetupGameScene()
    {
        // 确保字体已加载
        if (chineseFont == null)
        {
            LoadChineseFont();
        }
        
        var canvas = CreateCanvas("GameCanvas");
        
        // 半透明深色背景（游戏时可能需要看到3D场景）
        var bgOverlay = CreatePanel(canvas.transform, "BackgroundOverlay", new Color(0, 0, 0, 0.2f));
        SetAnchor(bgOverlay, AnchorPresets.Stretch, Vector2.zero);
        bgOverlay.SetActive(false); // 默认隐藏，可在暂停时显示
        
        // ========== 顶部信息栏 ==========
        var topBar = CreatePanel(canvas.transform, "TopInfoBar", new Color(0.08f, 0.08f, 0.12f, 0.95f));
        SetSize(topBar, 1920, 80);
        SetAnchor(topBar, AnchorPresets.TopStretch, Vector2.zero);
        
        var roundCard = CreateInfoCard(topBar.transform, "RoundCard", "回合", "1/12", new Color(0.3f, 0.5f, 0.8f), new Vector2(100, 0));
        var phaseCard = CreateInfoCard(topBar.transform, "PhaseCard", "阶段", "探索", new Color(0.5f, 0.3f, 0.8f), new Vector2(280, 0));
        var actionCard = CreateInfoCard(topBar.transform, "ActionCard", "行动点", "3/3", new Color(0.8f, 0.6f, 0.2f), new Vector2(460, 0));
        
        var currentPlayerBar = CreateCardPanel(topBar.transform, "CurrentPlayerBar", new Color(0.2f, 0.6f, 0.3f, 0.9f), 350, 60);
        SetAnchor(currentPlayerBar, AnchorPresets.MiddleRight, new Vector2(-100, 0));
        var currentPlayerText = CreateText(currentPlayerBar.transform, "CurrentPlayer", "👤 当前回合: 玩家1", 22);
        SetAnchor(currentPlayerText, AnchorPresets.MiddleCenter, Vector2.zero);
        
        var menuBtn = CreateIconButton(topBar.transform, "MenuBtn", "☰", new Vector2(-40, 0));
        
        // ========== 左侧玩家列表面板 ==========
        var leftPanel = CreateCardPanel(canvas.transform, "PlayerListPanel", new Color(0.08f, 0.08f, 0.15f, 0.92f), 260, 600);
        SetAnchor(leftPanel, AnchorPresets.MiddleLeft, new Vector2(15, 50));
        
        var playerListTitle = CreateText(leftPanel.transform, "Title", "玩家列表", 22);
        SetAnchor(playerListTitle, AnchorPresets.TopCenter, new Vector2(0, -15));
        
        // 创建玩家卡片
        for (int i = 0; i < 4; i++)
        {
            var playerCard = CreatePlayerListCard(leftPanel.transform, $"Player{i}Card", i, new Vector2(0, -60 - i * 130));
        }
        
        // ========== 中央游戏区域（留给BoardManager） ==========
        var gameAreaNote = CreatePanel(canvas.transform, "GameAreaNote", new Color(0.1f, 0.15f, 0.1f, 0.3f));
        SetSize(gameAreaNote, 400, 100);
        SetAnchor(gameAreaNote, AnchorPresets.MiddleCenter, Vector2.zero);
        var noteText = CreateText(gameAreaNote.transform, "Note", "游戏棋盘区域\n由BoardManager管理", 20);
        SetAnchor(noteText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(noteText, new Color(0.7f, 0.7f, 0.7f, 0.8f));
        
        // ========== 右侧技能/行动面板 ==========
        var rightPanel = CreateCardPanel(canvas.transform, "SkillPanel", new Color(0.10f, 0.08f, 0.12f, 0.92f), 280, 700);
        SetAnchor(rightPanel, AnchorPresets.MiddleRight, new Vector2(-15, 0));
        
        var skillTitle = CreateText(rightPanel.transform, "Title", "可用技能", 22);
        SetAnchor(skillTitle, AnchorPresets.TopCenter, new Vector2(0, -15));
        
        // 技能槽
        for (int i = 0; i < 4; i++)
        {
            var skillSlot = CreateSkillSlot(rightPanel.transform, $"SkillSlot{i}", new Vector2(0, -60 - i * 140));
        }
        
        var skillNote = CreateText(rightPanel.transform, "SkillNote", "[技能系统待实现]", 14);
        SetAnchor(skillNote, AnchorPresets.BottomCenter, new Vector2(0, 20));
        SetTextColor(skillNote, new Color(0.6f, 0.6f, 0.6f));
        
        // ========== 底部手牌区 ==========
        var handArea = CreatePanel(canvas.transform, "HandArea", new Color(0.08f, 0.08f, 0.12f, 0.92f));
        SetSize(handArea, 1920, 180);
        SetAnchor(handArea, AnchorPresets.BottomStretch, Vector2.zero);
        
        var handTitle = CreateText(handArea.transform, "HandTitle", "手牌", 20);
        SetAnchor(handTitle, AnchorPresets.TopLeft, new Vector2(30, -10));
        
        var cardCountText = CreateText(handArea.transform, "CardCount", "0/8", 18);
        SetAnchor(cardCountText, AnchorPresets.TopLeft, new Vector2(100, -12));
        SetTextColor(cardCountText, new Color(0.7f, 0.7f, 0.7f));
        
        // 手牌卡片占位符
        var handCardsContainer = CreatePanel(handArea.transform, "HandCardsContainer", Color.clear);
        SetSize(handCardsContainer, 1800, 140);
        SetAnchor(handCardsContainer, AnchorPresets.MiddleCenter, new Vector2(0, -5));
        
        for (int i = 0; i < 8; i++)
        {
            float x = -630 + i * 180;
            var cardSlot = CreateCardSlot(handCardsContainer.transform, $"CardSlot{i}", new Vector2(x, 0));
        }
        
        // ========== 浮动操作按钮 ==========
        var actionButtons = CreatePanel(canvas.transform, "ActionButtons", Color.clear);
        SetSize(actionButtons, 300, 80);
        SetAnchor(actionButtons, AnchorPresets.BottomCenter, new Vector2(0, 200));
        
        var endTurnBtn = CreateGradientButton(actionButtons.transform, "EndTurnBtn", "结束回合", 
            new Color(0.2f, 0.7f, 0.3f), new Color(0.15f, 0.5f, 0.2f), new Vector2(0, 0), new Vector2(200, 70));
        AddButtonOnClick(endTurnBtn, "GameController", "OnEndTurn");
        
        var menuBtn2 = CreateIconTextButton(actionButtons.transform, "MenuBtn", "☰", "菜单", 
            new Color(0.4f, 0.4f, 0.4f), new Vector2(-130, 0), new Vector2(80, 60));
        AddButtonOnClick(menuBtn2, "GameController", "OnMenu");
        
        // ========== 小地图（右下角） ==========
        var minimapCard = CreateCardPanel(canvas.transform, "MinimapCard", new Color(0.08f, 0.08f, 0.12f, 0.9f), 250, 220);
        SetAnchor(minimapCard, AnchorPresets.BottomRight, new Vector2(-310, 15));
        
        var minimapTitle = CreateText(minimapCard.transform, "Title", "小地图", 16);
        SetAnchor(minimapTitle, AnchorPresets.TopCenter, new Vector2(0, -8));
        
        var minimapPlaceholder = CreateIconPlaceholder(minimapCard.transform, "MinimapView", new Color(0.15f, 0.15f, 0.2f), 220, 170);
        SetAnchor(minimapPlaceholder, AnchorPresets.BottomCenter, new Vector2(0, 10));
        
        CreateEventSystem();
    }
    #endregion

    #region 地形编辑器场景设置
    void SetupTerrainEditorScene()
    {
        // 确保字体已加载
        if (chineseFont == null)
        {
            LoadChineseFont();
        }
        
        var canvas = CreateCanvas("EditorCanvas");
        
        // 背景
        var darkBG = CreatePanel(canvas.transform, "DarkBackground", new Color(0.06f, 0.07f, 0.09f));
        SetAnchor(darkBG, AnchorPresets.Stretch, Vector2.zero);
        
        // 尝试加载背景图
        var bgImage = darkBG.GetComponent<Image>();
        var bgSprite = LoadBackgroundSprite("MapEditor");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.color = new Color(1f, 1f, 1f, 0.3f); // 半透明显示背景图
        }
        
        // ========== 顶部工具栏 ==========
        var topToolbar = CreatePanel(canvas.transform, "TopToolbar", new Color(0.10f, 0.10f, 0.14f, 0.98f));
        SetSize(topToolbar, 1920, 70);
        SetAnchor(topToolbar, AnchorPresets.TopStretch, Vector2.zero);
        
        var editorTitle = CreateText(topToolbar.transform, "Title", "地形编辑器", 28);
        SetAnchor(editorTitle, AnchorPresets.MiddleLeft, new Vector2(30, 0));
        SetTextColor(editorTitle, new Color(0.9f, 0.9f, 1f));
        
        var toolButtons = CreatePanel(topToolbar.transform, "ToolButtons", Color.clear);
        SetAnchor(toolButtons, AnchorPresets.MiddleCenter, Vector2.zero);
        
        float btnX = -400;
        CreateToolbarButton(toolButtons.transform, "NewBtn", "📄 新建", new Color(0.2f, 0.7f, 0.3f), new Vector2(btnX, 0));
        CreateToolbarButton(toolButtons.transform, "OpenBtn", "📂 打开", new Color(0.3f, 0.5f, 0.8f), new Vector2(btnX + 120, 0));
        CreateToolbarButton(toolButtons.transform, "SaveBtn", "💾 保存", new Color(0.2f, 0.6f, 0.9f), new Vector2(btnX + 240, 0));
        CreateToolbarButton(toolButtons.transform, "ExportBtn", "📤 导出", new Color(0.7f, 0.5f, 0.2f), new Vector2(btnX + 360, 0));
        CreateToolbarButton(toolButtons.transform, "TestBtn", "▶️ 测试", new Color(0.5f, 0.3f, 0.8f), new Vector2(btnX + 480, 0));
        CreateToolbarButton(toolButtons.transform, "SettingsBtn", "⚙️ 设置", new Color(0.5f, 0.5f, 0.5f), new Vector2(btnX + 600, 0));
        CreateToolbarButton(toolButtons.transform, "ExitBtn", "🚪 退出", new Color(0.7f, 0.3f, 0.3f), new Vector2(btnX + 720, 0));
        
        // ========== 左侧地形工具面板 ==========
        var leftPanel = CreateCardPanel(canvas.transform, "TerrainToolPanel", new Color(0.08f, 0.08f, 0.12f, 0.95f), 280, 930);
        SetAnchor(leftPanel, AnchorPresets.MiddleLeft, new Vector2(15, -35));
        
        var toolPanelTitle = CreateText(leftPanel.transform, "Title", "地形类型", 24);
        SetAnchor(toolPanelTitle, AnchorPresets.TopCenter, new Vector2(0, -20));
        
        string[] terrainTypes = { "平原", "山地", "森林", "河流", "沼泽", "城镇", "要塞", "桥梁" };
        Color[] terrainColors = {
            new Color(0.5f, 0.7f, 0.3f),
            new Color(0.6f, 0.5f, 0.4f),
            new Color(0.3f, 0.6f, 0.3f),
            new Color(0.3f, 0.5f, 0.8f),
            new Color(0.4f, 0.5f, 0.4f),
            new Color(0.7f, 0.6f, 0.4f),
            new Color(0.5f, 0.4f, 0.4f),
            new Color(0.6f, 0.6f, 0.6f)
        };
        
        for (int i = 0; i < terrainTypes.Length; i++)
        {
            var terrainBtn = CreateTerrainTypeButton(leftPanel.transform, $"Terrain{i}Btn", terrainTypes[i], terrainColors[i], new Vector2(0, -70 - i * 75));
        }
        
        var brushSizeText = CreateText(leftPanel.transform, "BrushSizeText", "笔刷大小", 18);
        SetAnchor(brushSizeText, AnchorPresets.BottomCenter, new Vector2(0, 120));
        
        var brushSlider = CreateSliderPlaceholder(leftPanel.transform, "BrushSizeSlider", new Vector2(0, 80));
        
        // ========== 右侧属性面板 ==========
        var rightPanel = CreateCardPanel(canvas.transform, "PropertyPanel", new Color(0.08f, 0.08f, 0.12f, 0.95f), 320, 930);
        SetAnchor(rightPanel, AnchorPresets.MiddleRight, new Vector2(-15, -35));
        
        var propTitle = CreateText(rightPanel.transform, "Title", "地形属性", 24);
        SetAnchor(propTitle, AnchorPresets.TopCenter, new Vector2(0, -20));
        
        var propLabels = new string[] { "名称", "移动消耗", "防御加成", "视野范围", "资源产出" };
        for (int i = 0; i < propLabels.Length; i++)
        {
            var labelText = CreateText(rightPanel.transform, $"Label{i}", propLabels[i], 16);
            SetAnchor(labelText, AnchorPresets.TopLeft, new Vector2(20, -70 - i * 90));
            SetTextColor(labelText, new Color(0.8f, 0.8f, 0.8f));
            
            var inputField = CreateStyledInputField(rightPanel.transform, $"Input{i}", "...", new Vector2(0, -95 - i * 90));
        }
        
        var previewTitle = CreateText(rightPanel.transform, "PreviewTitle", "预览", 20);
        SetAnchor(previewTitle, AnchorPresets.TopCenter, new Vector2(0, -540));
        
        var previewBox = CreateIconPlaceholder(rightPanel.transform, "PreviewBox", new Color(0.15f, 0.15f, 0.20f), 280, 280);
        SetAnchor(previewBox, AnchorPresets.TopCenter, new Vector2(0, -575));
        
        // ========== 中央编辑区域 ==========
        var editArea = CreateCardPanel(canvas.transform, "EditArea", new Color(0.10f, 0.12f, 0.10f, 0.85f), 1280, 930);
        SetAnchor(editArea, AnchorPresets.MiddleCenter, new Vector2(0, -35));
        
        var gridOverlay = CreatePanel(editArea.transform, "GridOverlay", new Color(0.2f, 0.2f, 0.2f, 0.3f));
        SetSize(gridOverlay, 1240, 890);
        SetAnchor(gridOverlay, AnchorPresets.MiddleCenter, Vector2.zero);
        
        var editNote = CreateText(gridOverlay.transform, "EditNote", "地形编辑区域\n由TerrainManager管理\n\n点击此处放置地形", 22);
        SetAnchor(editNote, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(editNote, new Color(0.6f, 0.6f, 0.6f, 0.7f));
        
        // ========== 底部状态栏 ==========
        var bottomBar = CreatePanel(canvas.transform, "BottomStatusBar", new Color(0.08f, 0.08f, 0.12f, 0.95f));
        SetSize(bottomBar, 1920, 50);
        SetAnchor(bottomBar, AnchorPresets.BottomStretch, Vector2.zero);
        
        var statusText = CreateText(bottomBar.transform, "StatusText", "就绪 | 可以开始编辑地形", 16);
        SetAnchor(statusText, AnchorPresets.MiddleLeft, new Vector2(30, 0));
        SetTextColor(statusText, new Color(0.3f, 0.9f, 0.3f));
        
        var coordsText = CreateText(bottomBar.transform, "CoordsText", "坐标: (0, 0)", 16);
        SetAnchor(coordsText, AnchorPresets.MiddleCenter, new Vector2(-200, 0));
        SetTextColor(coordsText, new Color(0.8f, 0.8f, 0.8f));
        
        var zoomText = CreateText(bottomBar.transform, "ZoomText", "缩放: 100%", 16);
        SetAnchor(zoomText, AnchorPresets.MiddleCenter, new Vector2(0, 0));
        SetTextColor(zoomText, new Color(0.8f, 0.8f, 0.8f));
        
        var gridText = CreateText(bottomBar.transform, "GridText", "网格: 开", 16);
        SetAnchor(gridText, AnchorPresets.MiddleCenter, new Vector2(150, 0));
        SetTextColor(gridText, new Color(0.8f, 0.8f, 0.8f));
        
        CreateEventSystem();
    }
    #endregion

    #region UI创建辅助方法 - 基础组件

    GameObject CreateCanvas(string name)
    {
        var existing = GameObject.Find(name);
        if (existing != null)
        {
            Debug.Log($"Canvas已存在: {name}，将使用现有的");
            return existing;
        }

        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    GameObject CreatePanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        var image = go.AddComponent<Image>();
        image.color = color;
        return go;
    }

    GameObject CreateCardPanel(Transform parent, string name, Color color, float width, float height)
    {
        var go = CreatePanel(parent, name, color);
        SetSize(go, width, height);
        // 可以在这里添加圆角效果的组件（需要自定义Shader或使用Asset）
        return go;
    }

    GameObject CreateText(Transform parent, string name, string text, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        
        // 使用 TextMeshPro 支持中文（优先）
        var tmpText = go.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.enableWordWrapping = false;
        
        // 使用 TMP 中文字体
        if (tmpChineseFont != null)
        {
            tmpText.font = tmpChineseFont;
            Debug.Log($"✅ 为 {name} 应用TMP中文字体");
        }
        else
        {
            Debug.LogWarning($"⚠️ {name} 未找到TMP中文字体，可能无法显示中文");
            // 尝试使用 TMP 默认字体
            var defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (defaultFont != null)
            {
                tmpText.font = defaultFont;
            }
        }
        
        return go;
    }

    void SetTextColor(GameObject textObj, Color color)
    {
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
            tmp.color = color;
        else
        {
            var text = textObj.GetComponent<Text>();
            if (text != null)
                text.color = color;
        }
    }

    void SetTextAlignment(GameObject textObj, TextAnchor alignment)
    {
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            // 转换 TextAnchor 到 TextAlignmentOptions
            tmp.alignment = ConvertTextAnchorToTMP(alignment);
        }
        else
        {
            var text = textObj.GetComponent<Text>();
            if (text != null)
                text.alignment = alignment;
        }
    }
    
    TextAlignmentOptions ConvertTextAnchorToTMP(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.MidlineLeft;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
            default: return TextAlignmentOptions.Center;
        }
    }

    void SetTextOutline(GameObject textObj, Color color)
    {
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            // TextMeshPro 使用内置的 Outline 功能
            // 检查材质是否已经初始化
            if (tmp.fontMaterial != null)
            {
                tmp.outlineWidth = 0.2f;
                tmp.outlineColor = color;
            }
            else
            {
                // 如果材质还没初始化，延迟设置
                EditorApplication.delayCall += () =>
                {
                    if (tmp != null && tmp.fontMaterial != null)
                    {
                        tmp.outlineWidth = 0.2f;
                        tmp.outlineColor = color;
                    }
                };
            }
        }
        else
        {
            // 普通 Text 使用 Outline 组件
            var outline = textObj.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(2, -2);
        }
    }

    GameObject CreateGradientButton(Transform parent, string name, string text, Color topColor, Color bottomColor, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        var image = go.AddComponent<Image>();
        image.color = topColor; // 简化版本，实际渐变需要Shader
        
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        
        // 按钮文字
        var textGo = CreateText(go.transform, "Text", text, (int)(size.y * 0.35f));
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        SetTextOutline(textGo, new Color(0, 0, 0, 0.5f));
        
        return go;
    }

    GameObject CreateIconPlaceholder(Transform parent, string name, Color color, float width, float height)
    {
        var go = CreatePanel(parent, name, color);
        SetSize(go, width, height);
        
        var iconText = CreateText(go.transform, "IconText", "[图标]", (int)(height * 0.2f));
        SetAnchor(iconText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(iconText, new Color(0.5f, 0.5f, 0.5f));
        
        return go;
    }

    GameObject CreateStyledInputField(Transform parent, string name, string placeholder, Vector2 position)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280, 50);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.22f, 0.9f);

        var inputField = go.AddComponent<InputField>();
        
        // 占位符
        var placeholderGo = new GameObject("Placeholder");
        placeholderGo.transform.SetParent(go.transform, false);
        var placeholderRect = placeholderGo.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = new Vector2(-20, 0);
        
        var placeholderText = placeholderGo.AddComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 16;
        placeholderText.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
        placeholderText.alignment = TextAnchor.MiddleLeft;
        
        // 使用中文字体
        if (chineseFont != null)
            placeholderText.font = chineseFont;
        else
        {
            try
            {
                placeholderText.font = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 16);
            }
            catch
            {
                placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        // 实际文本
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var textRect = textGo.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-20, 0);
        
        var text = textGo.AddComponent<Text>();
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        
        // 使用中文字体
        if (chineseFont != null)
            text.font = chineseFont;
        else
        {
            try
            {
                text.font = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 18);
            }
            catch
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        inputField.textComponent = text;
        inputField.placeholder = placeholderText;

        return go;
    }

    GameObject CreateScrollView(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        
        var image = go.AddComponent<Image>();
        image.color = new Color(0.08f, 0.08f, 0.12f, 0.5f);
        
        var scrollRect = go.AddComponent<ScrollRect>();

        // Viewport
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(go.transform, false);
        var viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>();

        // Content
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 1500);

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        scrollRect.vertical = true;
        scrollRect.horizontal = false;

        return go;
    }

    void CreateEventSystem()
    {
        if (GameObject.Find("EventSystem") == null)
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<UnityEngine.EventSystems.EventSystem>();
            go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    #endregion

    #region UI创建辅助方法 - 复杂组件

    GameObject CreateBadge(Transform parent, string name, string text, Color color)
    {
        var go = CreatePanel(parent, name, color);
        SetSize(go, 60, 28);
        
        var badgeText = CreateText(go.transform, "Text", text, 14);
        SetAnchor(badgeText, AnchorPresets.MiddleCenter, Vector2.zero);
        
        return go;
    }

    GameObject CreateInfoRow(Transform parent, string name, string icon, string text, Vector2 position)
    {
        var go = CreatePanel(parent, name, Color.clear);
        SetSize(go, 300, 25);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var iconText = CreateText(go.transform, "Icon", icon, 18);
        SetAnchor(iconText, AnchorPresets.MiddleLeft, Vector2.zero);
        
        var infoText = CreateText(go.transform, "Text", text, 16);
        SetAnchor(infoText, AnchorPresets.MiddleLeft, new Vector2(35, 0));
        SetTextColor(infoText, new Color(0.9f, 0.9f, 0.9f));
        
        return go;
    }

    GameObject CreateIconButton(Transform parent, string name, string icon, Vector2 position)
    {
        var go = CreateGradientButton(parent, name, icon, new Color(0.3f, 0.3f, 0.35f), new Color(0.2f, 0.2f, 0.25f), position, new Vector2(60, 60));
        return go;
    }

    GameObject CreateIconTextButton(Transform parent, string name, string icon, string text, Color color, Vector2 position, Vector2 size)
    {
        var go = CreateGradientButton(parent, name, $"{icon} {text}", color, color * 0.7f, position, size);
        return go;
    }

    GameObject CreateTabButton(Transform parent, string name, string text, bool selected, Vector2 position)
    {
        Color color = selected ? new Color(0.3f, 0.5f, 0.8f) : new Color(0.2f, 0.2f, 0.25f);
        var go = CreateGradientButton(parent, name, text, color, color * 0.8f, position, new Vector2(120, 50));
        return go;
    }

    GameObject CreateDialog(Transform parent, string name, string title, float width, float height)
    {
        var dialogRoot = CreatePanel(parent, name, new Color(0, 0, 0, 0.7f));
        SetAnchor(dialogRoot, AnchorPresets.Stretch, Vector2.zero);
        
        var dialogPanel = CreateCardPanel(dialogRoot.transform, "DialogPanel", new Color(0.12f, 0.14f, 0.20f, 0.98f), width, height);
        SetAnchor(dialogPanel, AnchorPresets.MiddleCenter, Vector2.zero);
        
        var titleBar = CreatePanel(dialogPanel.transform, "TitleBar", new Color(0.2f, 0.25f, 0.35f));
        SetSize(titleBar, width, 60);
        SetAnchor(titleBar, AnchorPresets.TopCenter, Vector2.zero);
        
        var titleText = CreateText(titleBar.transform, "Title", title, 28);
        SetAnchor(titleText, AnchorPresets.MiddleCenter, Vector2.zero);
        
        return dialogRoot;
    }

    GameObject CreateRoomCard(Transform parent, string name, int index)
    {
        var card = CreateCardPanel(parent, name, new Color(0.12f, 0.14f, 0.20f, 0.9f), 1120, 120);
        var rect = card.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -70 - index * 140);
        
        var roomName = CreateText(card.transform, "RoomName", $"房间 #{index + 1}", 24);
        SetAnchor(roomName, AnchorPresets.MiddleLeft, new Vector2(30, 20));
        SetTextColor(roomName, new Color(1f, 0.95f, 0.8f));
        
        var hostName = CreateText(card.transform, "HostName", $"房主: 玩家{index + 1}", 18);
        SetAnchor(hostName, AnchorPresets.MiddleLeft, new Vector2(30, -15));
        SetTextColor(hostName, new Color(0.7f, 0.8f, 1f));
        
        var playerCount = CreateBadge(card.transform, "PlayerCount", "2/4", new Color(0.3f, 0.6f, 0.3f));
        SetAnchor(playerCount, AnchorPresets.MiddleLeft, new Vector2(300, 0));
        
        var statusBadge = CreateBadge(card.transform, "Status", "等待中", new Color(0.8f, 0.6f, 0.2f));
        SetAnchor(statusBadge, AnchorPresets.MiddleLeft, new Vector2(380, 0));
        
        var joinBtn = CreateGradientButton(card.transform, "JoinBtn", "加入", 
            new Color(0.2f, 0.6f, 0.9f), new Color(0.15f, 0.4f, 0.7f), new Vector2(500, 0), new Vector2(100, 50));
        
        return card;
    }

    GameObject CreateRoleTab(Transform parent, string name, string text, Color color, Vector2 position)
    {
        var tab = CreatePanel(parent, name, new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 0.8f));
        SetSize(tab, 440, 50);
        var rect = tab.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var colorIndicator = CreatePanel(tab.transform, "ColorBar", color);
        SetSize(colorIndicator, 10, 50);
        SetAnchor(colorIndicator, AnchorPresets.MiddleLeft, Vector2.zero);
        
        var tabText = CreateText(tab.transform, "Text", text, 22);
        SetAnchor(tabText, AnchorPresets.MiddleCenter, Vector2.zero);
        
        return tab;
    }

    GameObject CreateHeroCard(Transform parent, string name, Vector2 position, Color roleColor)
    {
        var card = CreateCardPanel(parent, name, new Color(0.12f, 0.12f, 0.18f, 0.9f), 420, 200);
        var rect = card.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var portrait = CreateIconPlaceholder(card.transform, "Portrait", new Color(0.2f, 0.2f, 0.25f), 120, 160);
        SetAnchor(portrait, AnchorPresets.MiddleLeft, new Vector2(15, 0));
        
        var heroName = CreateText(card.transform, "HeroName", "武将名", 22);
        SetAnchor(heroName, AnchorPresets.TopLeft, new Vector2(150, -15));
        SetTextColor(heroName, new Color(1f, 0.9f, 0.7f));
        
        var roleLabel = CreateBadge(card.transform, "Role", "输出", roleColor);
        SetAnchor(roleLabel, AnchorPresets.TopLeft, new Vector2(150, -50));
        
        var statsText = CreateText(card.transform, "Stats", "攻击:5 防御:3 移动:4", 14);
        SetAnchor(statsText, AnchorPresets.TopLeft, new Vector2(150, -85));
        SetTextColor(statsText, new Color(0.8f, 0.8f, 0.8f));
        
        var skillText = CreateText(card.transform, "Skill", "技能: [待补充]", 14);
        SetAnchor(skillText, AnchorPresets.TopLeft, new Vector2(150, -110));
        SetTextColor(skillText, new Color(0.7f, 0.8f, 1f));
        
        var selectBtn = CreateGradientButton(card.transform, "SelectBtn", "选择", 
            new Color(0.2f, 0.6f, 0.3f), new Color(0.15f, 0.4f, 0.2f), new Vector2(310, -75), new Vector2(90, 40));
        
        return card;
    }

    GameObject CreateInfoCard(Transform parent, string name, string label, string value, Color color, Vector2 position)
    {
        var card = CreateCardPanel(parent, name, color, 150, 60);
        var rect = card.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var labelText = CreateText(card.transform, "Label", label, 16);
        SetAnchor(labelText, AnchorPresets.TopCenter, new Vector2(0, -8));
        SetTextColor(labelText, new Color(0.9f, 0.9f, 0.9f));
        
        var valueText = CreateText(card.transform, "Value", value, 22);
        SetAnchor(valueText, AnchorPresets.BottomCenter, new Vector2(0, 8));
        
        return card;
    }

    GameObject CreatePlayerListCard(Transform parent, string name, int index, Vector2 position)
    {
        var card = CreateCardPanel(parent, name, new Color(0.15f, 0.15f, 0.22f, 0.9f), 240, 120);
        var rect = card.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var avatar = CreateIconPlaceholder(card.transform, "Avatar", new Color(0.3f, 0.5f, 0.7f), 60, 60);
        SetAnchor(avatar, AnchorPresets.MiddleLeft, new Vector2(15, 0));
        
        var playerName = CreateText(card.transform, "Name", $"玩家{index + 1}", 18);
        SetAnchor(playerName, AnchorPresets.TopLeft, new Vector2(85, -15));
        SetTextColor(playerName, new Color(1f, 1f, 1f));
        
        var hp = CreateText(card.transform, "HP", "HP: 10/10", 14);
        SetAnchor(hp, AnchorPresets.TopLeft, new Vector2(85, -40));
        SetTextColor(hp, new Color(0.3f, 0.8f, 0.3f));
        
        var cards = CreateText(card.transform, "Cards", "手牌: 5", 14);
        SetAnchor(cards, AnchorPresets.TopLeft, new Vector2(85, -60));
        SetTextColor(cards, new Color(0.8f, 0.8f, 1f));
        
        var status = CreateBadge(card.transform, "Status", "存活", new Color(0.2f, 0.6f, 0.3f));
        SetAnchor(status, AnchorPresets.BottomCenter, new Vector2(0, 10));
        
        return card;
    }

    GameObject CreateSkillSlot(Transform parent, string name, Vector2 position)
    {
        var slot = CreateCardPanel(parent, name, new Color(0.15f, 0.12f, 0.18f, 0.9f), 250, 120);
        var rect = slot.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var icon = CreateIconPlaceholder(slot.transform, "Icon", new Color(0.3f, 0.3f, 0.4f), 80, 80);
        SetAnchor(icon, AnchorPresets.MiddleLeft, new Vector2(15, 0));
        
        var skillName = CreateText(slot.transform, "SkillName", "技能名", 16);
        SetAnchor(skillName, AnchorPresets.TopRight, new Vector2(-15, -10));
        SetTextColor(skillName, new Color(1f, 0.9f, 0.7f));
        
        var cooldown = CreateText(slot.transform, "Cooldown", "CD: 0", 14);
        SetAnchor(cooldown, AnchorPresets.BottomRight, new Vector2(-15, 10));
        SetTextColor(cooldown, new Color(0.7f, 0.7f, 0.7f));
        
        return slot;
    }

    GameObject CreateCardSlot(Transform parent, string name, Vector2 position)
    {
        var slot = CreateCardPanel(parent, name, new Color(0.15f, 0.15f, 0.20f, 0.6f), 160, 120);
        var rect = slot.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var slotText = CreateText(slot.transform, "Text", "[空]", 16);
        SetAnchor(slotText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(slotText, new Color(0.5f, 0.5f, 0.5f));
        
        return slot;
    }

    GameObject CreateToolbarButton(Transform parent, string name, string text, Color color, Vector2 position)
    {
        return CreateGradientButton(parent, name, text, color, color * 0.7f, position, new Vector2(110, 50));
    }

    GameObject CreateTerrainTypeButton(Transform parent, string name, string text, Color color, Vector2 position)
    {
        var btn = CreateGradientButton(parent, name, text, color, color * 0.7f, position, new Vector2(250, 65));
        
        var colorSwatch = CreatePanel(btn.transform, "ColorSwatch", color);
        SetSize(colorSwatch, 30, 30);
        SetAnchor(colorSwatch, AnchorPresets.MiddleLeft, new Vector2(15, 0));
        
        return btn;
    }

    GameObject CreateSliderPlaceholder(Transform parent, string name, Vector2 position)
    {
        var go = CreatePanel(parent, name, new Color(0.2f, 0.2f, 0.25f, 0.8f));
        SetSize(go, 240, 30);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        
        var sliderText = CreateText(go.transform, "Text", "[滑块]", 14);
        SetAnchor(sliderText, AnchorPresets.MiddleCenter, Vector2.zero);
        SetTextColor(sliderText, new Color(0.6f, 0.6f, 0.6f));
        
        return go;
    }

    #endregion

    #region 布局辅助方法

    void SetSize(GameObject go, float width, float height)
    {
        var rect = go.GetComponent<RectTransform>();
        if (rect != null)
            rect.sizeDelta = new Vector2(width, height);
    }

    void SetAnchor(GameObject go, AnchorPresets preset, Vector2 offset)
    {
        var rect = go.GetComponent<RectTransform>();
        if (rect == null) return;
        
        switch (preset)
        {
            case AnchorPresets.TopLeft:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                break;
            case AnchorPresets.TopCenter:
                rect.anchorMin = new Vector2(0.5f, 1);
                rect.anchorMax = new Vector2(0.5f, 1);
                rect.pivot = new Vector2(0.5f, 1);
                break;
            case AnchorPresets.TopRight:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                break;
            case AnchorPresets.MiddleLeft:
                rect.anchorMin = new Vector2(0, 0.5f);
                rect.anchorMax = new Vector2(0, 0.5f);
                rect.pivot = new Vector2(0, 0.5f);
                break;
            case AnchorPresets.MiddleCenter:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPresets.MiddleRight:
                rect.anchorMin = new Vector2(1, 0.5f);
                rect.anchorMax = new Vector2(1, 0.5f);
                rect.pivot = new Vector2(1, 0.5f);
                break;
            case AnchorPresets.BottomLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                break;
            case AnchorPresets.BottomCenter:
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.pivot = new Vector2(0.5f, 0);
                break;
            case AnchorPresets.BottomRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                break;
            case AnchorPresets.TopStretch:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 1);
                break;
            case AnchorPresets.BottomStretch:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 0);
                break;
            case AnchorPresets.Stretch:
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = Vector2.zero;
                break;
        }
        
        rect.anchoredPosition = offset;
    }

    void ClearCurrentScene()
    {
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                DestroyImmediate(obj);
            }
        }
        Debug.Log("场景已清空（保留了Camera和Light）");
    }

    // 加载背景图片
    Sprite LoadBackgroundSprite(string sceneName)
    {
        // 尝试多种格式和路径
        string[] possiblePaths = {
            $"Assets/Resources/UI/Backgrounds/{sceneName}.jpg",
            $"Assets/Resources/UI/Backgrounds/{sceneName}.png",
            $"Assets/UI/Backgrounds/{sceneName}.jpg",
            $"Assets/UI/Backgrounds/{sceneName}.png",
            $"Assets/Textures/Backgrounds/{sceneName}.jpg",
            $"Assets/Textures/Backgrounds/{sceneName}.png"
        };
        
        foreach (var path in possiblePaths)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null)
            {
                // 将Texture2D转换为Sprite
                string texturePath = AssetDatabase.GetAssetPath(texture);
                
                // 先尝试直接加载Sprite
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
                
                if (sprite == null)
                {
                    // 如果直接加载失败，确保纹理导入设置正确
                    TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                    if (importer != null && importer.textureType != TextureImporterType.Sprite)
                    {
                        Debug.LogWarning($"⚠️ 背景图 {sceneName} 需要设置为 Sprite 类型。正在自动修复...");
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        importer.maxTextureSize = 2048;
                        importer.SaveAndReimport();
                        
                        // 重新加载
                        sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
                    }
                    
                    // 如果还是null，创建临时Sprite
                    if (sprite == null)
                    {
                        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    }
                }
                
                Debug.Log($"✅ 成功加载背景图: {path}");
                return sprite;
            }
        }
        
        Debug.Log($"💡 未找到 {sceneName} 的背景图，使用默认样式。可运行「三国策略 → 背景图片管理器」生成默认背景。");
        return null;
    }

    // 添加按钮点击事件（通过创建占位脚本引用）
    void AddButtonOnClick(GameObject buttonObj, string controllerName, string methodName)
    {
        var button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            // 创建一个空的GameObject作为控制器占位符
            var controllerObj = GameObject.Find(controllerName);
            if (controllerObj == null)
            {
                controllerObj = new GameObject(controllerName);
                // 添加注释组件说明这是个占位符
                var note = controllerObj.AddComponent<UnityEngine.UI.Text>();
                note.text = $"// 控制器占位符 - 实际游戏中应添加对应的 {controllerName} 脚本";
                note.enabled = false;
            }
            
            // 在Editor模式下，我们添加一个临时监听器用于测试
            button.onClick.AddListener(() => {
                Debug.Log($"🎯 按钮点击: {buttonObj.name} -> {controllerName}.{methodName}()");
            });
        }
    }

    #endregion

    #region TextMeshPro 中文字体处理
    
    TMP_FontAsset FindTMPChineseFont()
    {
        // 查找项目中的中文TMP字体
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            // 检查是否是我们要找的字体
            if (path.Contains("Chinese") || path.Contains("中文") || 
                path.Contains("YaHei") || path.Contains("SimHei") || 
                path.Contains("SGSA"))
            {
                try
                {
                    // 使用 AssetDatabase.LoadMainAssetAtPath 避免触发 OnValidate
                    TMP_FontAsset fontAsset = AssetDatabase.LoadMainAssetAtPath(path) as TMP_FontAsset;
                    
                    // 检查字体是否有效（有材质）
                    if (fontAsset != null)
                    {
                        // 尝试访问 material，捕获可能的异常
                        try
                        {
                            var mat = fontAsset.material;
                            if (mat != null)
                            {
                                return fontAsset;
                            }
                        }
                        catch
                        {
                            // 材质无效，删除损坏的字体
                            Debug.LogWarning($"⚠️ 找到字体 {path} 但它已损坏（缺少材质），将被删除");
                            AssetDatabase.DeleteAsset(path);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ 加载字体 {path} 时出错，将被删除: {ex.Message}");
                    try
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                    catch
                    {
                        // 忽略删除失败
                    }
                }
            }
        }
        
        return null;
    }
    
    void GenerateTMPChineseFont()
    {
        try
        {
            EditorUtility.DisplayProgressBar("生成中文字体", "正在准备...", 0.1f);
            
            // 1. 查找或创建系统字体
            Font sourceFont = null;
            string[] fontNames = { "Microsoft YaHei", "SimHei", "Arial Unicode MS" };
            
            foreach (string fontName in fontNames)
            {
                try
                {
                    sourceFont = Font.CreateDynamicFontFromOSFont(fontName, 32);
                    if (sourceFont != null)
                    {
                        Debug.Log($"✅ 使用系统字体: {fontName}");
                        break;
                    }
                }
                catch { }
            }
            
            if (sourceFont == null)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("错误", 
                    "无法找到中文字体！\n\n请手动操作：\n" +
                    "1. 打开 C:\\Windows\\Fonts\n" +
                    "2. 复制 msyh.ttc (微软雅黑) 到项目的 Assets/Fonts/ 目录\n" +
                    "3. 重新运行此工具", 
                    "确定");
                return;
            }
            
            // 2. 创建输出目录
            string fontDir = "Assets/Fonts/TMP";
            if (!System.IO.Directory.Exists(fontDir))
            {
                System.IO.Directory.CreateDirectory(fontDir);
                AssetDatabase.Refresh();
            }
            
            EditorUtility.DisplayProgressBar("生成中文字体", "正在生成字体图集...", 0.4f);
            
            // 3. 生成常用中文字符集
            string characterSet = GetGameCommonCharacters();
            
            // 4. 创建TMP字体资源
            string fontPath = $"{fontDir}/SGSA_ChineseFont.asset";
            
            EditorUtility.DisplayProgressBar("生成中文字体", "正在创建字体资源...", 0.6f);
            
            // 先创建材质和纹理（必须在创建 TMP_FontAsset 之前准备好）
            EditorUtility.DisplayProgressBar("生成中文字体", "正在创建字体材质...", 0.5f);
            
            // 创建空白纹理图集
            var atlasTexture = new Texture2D(512, 512, TextureFormat.Alpha8, false);
            atlasTexture.name = "SGSA_ChineseFont Atlas";
            
            // 创建材质
            Material fontMaterial = new Material(Shader.Find("TextMeshPro/Distance Field"));
            fontMaterial.name = "SGSA_ChineseFont Material";
            fontMaterial.SetTexture("_MainTex", atlasTexture);
            
            Debug.Log("✅ 创建字体材质和纹理图集");
            
            // 创建TMP字体资源（材质必须在创建之前准备好）
            EditorUtility.DisplayProgressBar("生成中文字体", "正在创建TMP字体资源...", 0.6f);
            
            TMP_FontAsset fontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
            
            if (fontAsset == null)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("失败", "无法创建TMP_FontAsset实例！", "确定");
                return;
            }
            
            Debug.Log("✅ 创建了TMP_FontAsset实例");
            
            // 使用反射设置所有必需的内部字段（避免触发 OnValidate）
            var fontType = typeof(TMP_FontAsset);
            var bindingFlags = System.Reflection.BindingFlags.NonPublic | 
                              System.Reflection.BindingFlags.Public | 
                              System.Reflection.BindingFlags.Instance;
            
            // 1. 先通过反射设置材质字段（m_Material）
            var materialField = fontType.GetField("m_Material", bindingFlags);
            if (materialField != null)
            {
                materialField.SetValue(fontAsset, fontMaterial);
                Debug.Log("✅ 通过反射设置 m_Material 字段");
            }
            else
            {
                // 如果找不到私有字段，使用公共属性
                fontAsset.material = fontMaterial;
                Debug.Log("✅ 通过属性设置 material");
            }
            
            // 2. 设置图集纹理（m_AtlasTexture）
            var atlasTextureField = fontType.GetField("m_AtlasTexture", bindingFlags);
            if (atlasTextureField != null)
            {
                atlasTextureField.SetValue(fontAsset, atlasTexture);
                Debug.Log("✅ 设置图集纹理");
            }
            
            // 3. 设置源字体（兼容不同版本）
            bool fontSet = false;
            string[] possibleFields = new string[] 
            {
                "m_SourceFontFile",
                "m_SourceFontFile_EditorRef", 
                "sourceFontFile",
                "m_SourceFont"
            };
            
            foreach (var fieldName in possibleFields)
            {
                var field = fontType.GetField(fieldName, bindingFlags);
                if (field != null)
                {
                    try
                    {
                        field.SetValue(fontAsset, sourceFont);
                        fontSet = true;
                        Debug.Log($"✅ 成功通过字段 '{fieldName}' 设置源字体");
                        break;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"⚠️ 字段 '{fieldName}' 设置失败: {ex.Message}");
                    }
                }
            }
            
            if (!fontSet)
            {
                Debug.LogWarning("⚠️ 无法通过反射设置源字体，字体可能无法正常工作");
            }
            
            // 4. 设置字体名称和模式
            fontAsset.name = "SGSA_ChineseFont";
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            
            Debug.Log("✅ 设置字体为动态模式");
            
            // 6. 现在保存所有资源
            EditorUtility.DisplayProgressBar("生成中文字体", "正在保存资源...", 0.8f);
            
            // 使用 SerializedObject 来安全地设置属性（避免触发 OnValidate）
            SerializedObject serializedFont = new SerializedObject(fontAsset);
            
            // 再次确保材质已设置
            SerializedProperty materialProp = serializedFont.FindProperty("m_Material");
            if (materialProp != null)
            {
                materialProp.objectReferenceValue = fontMaterial;
                Debug.Log("✅ 通过 SerializedProperty 设置材质");
            }
            
            // 应用更改但不触发 OnValidate
            serializedFont.ApplyModifiedPropertiesWithoutUndo();
            
            // 确保材质已正确设置（在保存之前）
            fontAsset.material = fontMaterial;
            
            // 先创建材质和纹理作为独立资源
            string materialPath = $"{fontDir}/SGSA_ChineseFont_Material.mat";
            string texturePath = $"{fontDir}/SGSA_ChineseFont_Atlas.asset";
            
            AssetDatabase.CreateAsset(fontMaterial, materialPath);
            AssetDatabase.CreateAsset(atlasTexture, texturePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("✅ 保存材质和纹理资源");
            
            // 现在创建字体资源（主资源），此时材质已经是持久化的资源了
            AssetDatabase.CreateAsset(fontAsset, fontPath);
            
            // 重新加载已持久化的资源，确保引用正确
            fontMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            
            // 确保材质引用正确的图集纹理
            fontMaterial.SetTexture("_MainTex", atlasTexture);
            
            // 使用SerializedObject来安全地设置所有引用
            SerializedObject so = new SerializedObject(fontAsset);
            
            // 设置材质引用
            SerializedProperty matProp = so.FindProperty("m_Material");
            if (matProp != null)
            {
                matProp.objectReferenceValue = fontMaterial;
            }
            
            // 关键：设置图集纹理数组引用
            SerializedProperty atlasTexturesProp = so.FindProperty("m_AtlasTextures");
            if (atlasTexturesProp != null && atlasTexturesProp.isArray)
            {
                atlasTexturesProp.ClearArray();
                atlasTexturesProp.InsertArrayElementAtIndex(0);
                atlasTexturesProp.GetArrayElementAtIndex(0).objectReferenceValue = atlasTexture;
                Debug.Log("✅ 设置字体图集纹理引用");
            }
            
            // 应用所有更改
            so.ApplyModifiedPropertiesWithoutUndo();
            
            // 保存所有更改
            EditorUtility.SetDirty(fontAsset);
            EditorUtility.SetDirty(fontMaterial);
            EditorUtility.SetDirty(atlasTexture);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayProgressBar("生成中文字体", "完成！", 1.0f);
            
            Debug.Log($"✅ 成功生成TMP中文字体: {fontPath}");
            Debug.Log($"   字体模式: Dynamic（动态加载）");
            Debug.Log($"   中文字符将在运行时自动从系统字体加载");
            
            EditorUtility.ClearProgressBar();
            
            // 自动应用到所有场景
            if (EditorUtility.DisplayDialog("成功", 
                "✅ TMP中文字体生成成功！\n\n" +
                "字体已设置为动态模式，会在运行时自动加载中文字符。\n\n" +
                "是否立即应用到所有场景？", 
                "是", "稍后"))
            {
                ApplyTMPFontToAllScenes(fontAsset);
            }
            
            // 选中生成的字体（延迟执行避免错误）
            EditorApplication.delayCall += () => 
            {
                if (fontAsset != null)
                {
                    Selection.activeObject = fontAsset;
                    EditorGUIUtility.PingObject(fontAsset);
                }
            };
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("错误", $"生成字体时出错：\n{e.Message}", "确定");
            Debug.LogError($"❌ 生成字体失败: {e}");
        }
    }
    
    string GetGameCommonCharacters()
    {
        // 游戏中使用的所有中文字符
        return "三国策略战斗" +
               "登录注册账号密码确认" +
               "用户名邮箱手机号码" +
               "没有点击返回提交取消" +
               "开始游戏设置退出" +
               "主城野外英雄背包商店" +
               "金币木材石料粮食资源" +
               "等级经验战力属性技能" +
               "攻击防御生命法力速度" +
               "装备道具武器防具饰品" +
               "任务成就排行榜奖励" +
               "好友聊天邮件公告消息" +
               "联盟军团公会帮派势力" +
               "建筑升级招募训练科技" +
               "士兵武将城池关卡副本" +
               "刘备关羽张飞赵云马超黄忠" +
               "曹操夏侯惇夏侯渊典韦许褚张辽" +
               "孙权周瑜鲁肃吕蒙陆逊甘宁" +
               "诸葛亮庞统司马懿郭嘉荀彧" +
               "吕布貂蝉董卓袁绍袁术" +
               "一二三四五六七八九十百千万亿" +
               "年月日时分秒天周期" +
               "的了是在不有和人这中大为上个我" +
               "来说到于地他时要就出会可也你" +
               "对生能而子得如于着下自之年过" +
               "发后作里用道行所然家种事成方" +
               "多经么去法学如都同现当没动面" +
               "起看定天分还进好小部其些主样" +
               "理心她本前开但因只从想实日军" +
               "者意无力它与长把机十民第公此" +
               "使结解知民很情量长程度该常非" +
               "间由问工作地给总体合相样被两" +
               "重新线内正外将点此变条物何通" +
               "0123456789" +
               "abcdefghijklmnopqrstuvwxyz" +
               "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
               "！？。，、；：（）【】《》" +
               "+-*/=<>≤≥×÷%@#$&" +
               "❤★☆◆◇○●□■△▲▽▼" +
               "⚔🛡💰🏆🎖👤👥✉🏛🚪⚙";
    }
    
    void ApplyTMPFontToAllScenes(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null)
        {
            EditorUtility.DisplayDialog("错误", "字体资源为空！", "确定");
            return;
        }
        
        // 警告用户
        if (!EditorUtility.DisplayDialog("警告", 
            "⚠️ 此操作会替换所有场景中的TMP字体！\n\n" +
            "如果某些场景已经配置好了字体，建议只手动修复有问题的场景。\n\n" +
            "确定要继续吗？",
            "继续", "取消"))
        {
            return;
        }
        
        // 获取所有场景路径
        string[] scenePaths = {
            "Assets/Scenes/Login.unity",
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/RoomList.unity",
            "Assets/Scenes/HeroSelection.unity",
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/TerrainEditor.unity"
        };
        
        int updatedCount = 0;
        int totalCount = 0;
        int skippedCount = 0;
        
        try
        {
            foreach (string scenePath in scenePaths)
            {
                if (!System.IO.File.Exists(scenePath))
                    continue;
                
                EditorUtility.DisplayProgressBar("应用字体", $"正在处理场景: {System.IO.Path.GetFileNameWithoutExtension(scenePath)}...", (float)totalCount / scenePaths.Length);
                
                // 打开场景
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                
                // 查找所有TextMeshProUGUI组件
                var tmpComponents = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
                
                int sceneUpdated = 0;
                foreach (var tmp in tmpComponents)
                {
                    // 只替换字体为null或者是默认字体的组件
                    if (tmp.font == null || tmp.font.name.Contains("LiberationSans"))
                    {
                        tmp.font = fontAsset;
                        updatedCount++;
                        sceneUpdated++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
                
                // 只有在有更新时才保存场景
                if (sceneUpdated > 0)
                {
                    EditorSceneManager.SaveScene(scene);
                    Debug.Log($"✅ 场景 {System.IO.Path.GetFileNameWithoutExtension(scenePath)} 已更新 {sceneUpdated} 个TMP组件（跳过 {tmpComponents.Length - sceneUpdated} 个已配置的组件）");
                }
                else
                {
                    Debug.Log($"ℹ️ 场景 {System.IO.Path.GetFileNameWithoutExtension(scenePath)} 中所有TMP组件都已正确配置，无需更新");
                }
                
                totalCount++;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        
        EditorUtility.DisplayDialog("完成", 
            $"✅ 字体应用完成！\n\n" +
            $"处理场景: {totalCount} 个\n" +
            $"更新组件: {updatedCount} 个\n" +
            $"跳过已配置: {skippedCount} 个\n\n" +
            $"现在运行游戏，中文应该可以正常显示了！", 
            "确定");
        
        Debug.Log($"========== TMP字体应用完成 ==========");
        Debug.Log($"处理场景: {totalCount}, 更新组件: {updatedCount}, 跳过: {skippedCount}");
    }
    
    #endregion

    enum AnchorPresets
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight,
        TopStretch, BottomStretch, Stretch
    }
}
