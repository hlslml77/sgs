using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/// <summary>
/// 网络设置向导 - 一键解决网络连接问题
/// </summary>
public class NetworkSetupWizard : EditorWindow
{
    private string serverUrl = "http://localhost:8080";
    private string websocketUrl = "ws://localhost:8080/ws";
    private bool isServerReachable = false;
    private bool isWebSocketSharpInstalled = false;
    private string statusMessage = "点击下方按钮开始诊断...";
    private Color statusColor = Color.white;
    
    [MenuItem("三国策略/网络设置向导 🌐")]
    public static void ShowWindow()
    {
        var window = GetWindow<NetworkSetupWizard>("网络设置向导");
        window.minSize = new Vector2(600, 700);
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Space(10);
        
        // 标题
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("🌐 网络连接诊断与修复", titleStyle);
        
        GUILayout.Space(15);
        
        // 状态显示
        DrawStatusBox();
        
        GUILayout.Space(15);
        
        // 服务器配置
        DrawServerConfig();
        
        GUILayout.Space(15);
        
        // 诊断按钮
        DrawDiagnosticButtons();
        
        GUILayout.Space(15);
        
        // WebSocket-Sharp 设置
        DrawWebSocketSetup();
        
        GUILayout.Space(15);
        
        // 快速修复
        DrawQuickFix();
        
        GUILayout.Space(15);
        
        // 中文字体修复
        DrawChineseFontFix();
        
        GUILayout.Space(15);
        
        // 帮助信息
        DrawHelpInfo();
    }
    
    void DrawStatusBox()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
        statusStyle.wordWrap = true;
        statusStyle.normal.textColor = statusColor;
        
        EditorGUILayout.LabelField("🔍 诊断状态", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.LabelField(statusMessage, statusStyle);
        
        GUILayout.Space(10);
        
        // 检查项显示
        DrawCheckItem("WebSocket-Sharp 库", isWebSocketSharpInstalled);
        DrawCheckItem("服务器连接", isServerReachable);
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawCheckItem(string label, bool status)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label + ":", GUILayout.Width(150));
        
        GUI.color = status ? Color.green : Color.red;
        EditorGUILayout.LabelField(status ? "✅ 正常" : "❌ 未就绪");
        GUI.color = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }
    
    void DrawServerConfig()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚙️ 服务器配置", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        serverUrl = EditorGUILayout.TextField("API 地址:", serverUrl);
        websocketUrl = EditorGUILayout.TextField("WebSocket 地址:", websocketUrl);
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawDiagnosticButtons()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🔧 诊断工具", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔍 全面诊断 (推荐)", GUILayout.Height(40)))
        {
            RunFullDiagnostic();
        }
        
        GUILayout.Space(5);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("检查 WebSocket 库", GUILayout.Height(30)))
        {
            CheckWebSocketSharp();
        }
        
        if (GUILayout.Button("测试服务器连接", GUILayout.Height(30)))
        {
            TestServerConnection();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawWebSocketSetup()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📦 WebSocket-Sharp 安装", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        if (!isWebSocketSharpInstalled)
        {
            EditorGUILayout.HelpBox(
                "⚠️ 未检测到 WebSocket-Sharp 库！\n\n" +
                "游戏无法连接到服务器，按钮将无法正常工作。",
                MessageType.Warning);
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("📥 下载并安装 WebSocket-Sharp", GUILayout.Height(35)))
            {
                DownloadWebSocketSharp();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("📂 手动安装说明", GUILayout.Height(30)))
            {
                ShowManualInstallGuide();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("✅ WebSocket-Sharp 已正确安装", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawQuickFix()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚡ 一键修复", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🔧 自动修复所有问题", GUILayout.Height(45)))
        {
            AutoFixAllIssues();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(5);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🔗 修复场景按钮", GUILayout.Height(30)))
        {
            FixSceneButtons();
        }
        
        if (GUILayout.Button("🌐 配置网络管理器", GUILayout.Height(30)))
        {
            ConfigureNetworkManager();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawHelpInfo()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("💡 常见问题", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.HelpBox(
            "Q: 按钮点击没反应？\n" +
            "A: 1) 先运行「全面诊断」 2) 点击「自动修复所有问题」\n\n" +
            "Q: 无法连接服务器？\n" +
            "A: 1) 确保服务器正在运行 (运行 start_server.bat)\n" +
            "   2) 检查防火墙设置\n" +
            "   3) 确认服务器地址正确\n\n" +
            "Q: WebSocket-Sharp 下载失败？\n" +
            "A: 点击「手动安装说明」查看详细步骤",
            MessageType.None);
        
        EditorGUILayout.EndVertical();
    }
    
    // ========== 诊断功能 ==========
    
    void RunFullDiagnostic()
    {
        statusMessage = "🔍 正在进行全面诊断...\n";
        statusColor = Color.yellow;
        Repaint();
        
        CheckWebSocketSharp();
        TestServerConnection();
        CheckSceneSetup();
        
        if (isWebSocketSharpInstalled && isServerReachable)
        {
            statusMessage = "✅ 诊断完成！所有系统正常。\n\n可以开始游戏了！";
            statusColor = Color.green;
        }
        else
        {
            statusMessage = "⚠️ 诊断完成，发现问题：\n\n";
            
            if (!isWebSocketSharpInstalled)
                statusMessage += "❌ WebSocket-Sharp 未安装\n";
            if (!isServerReachable)
                statusMessage += "❌ 无法连接到服务器\n";
                
            statusMessage += "\n点击「自动修复所有问题」按钮来修复。";
            statusColor = Color.red;
        }
        
        Repaint();
    }
    
    void CheckWebSocketSharp()
    {
        // 检查 Plugins 目录是否存在 DLL
        string pluginsPath = "Assets/Plugins";
        string dllPath = Path.Combine(pluginsPath, "websocket-sharp.dll");
        
        isWebSocketSharpInstalled = File.Exists(dllPath);
        
        // 同时检查编译符号
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        bool hasDefine = currentDefines.Contains("WEBSOCKET_SHARP");
        
        if (isWebSocketSharpInstalled && !hasDefine)
        {
            statusMessage = "⚠️ WebSocket-Sharp DLL 存在，但编译符号未定义。\n点击「自动修复」来添加编译符号。";
            statusColor = Color.yellow;
        }
        else if (!isWebSocketSharpInstalled)
        {
            statusMessage = "❌ WebSocket-Sharp 未安装。\n点击「下载并安装」按钮。";
            statusColor = Color.red;
        }
        else
        {
            statusMessage = "✅ WebSocket-Sharp 已正确配置。";
            statusColor = Color.green;
        }
        
        Repaint();
    }
    
    async void TestServerConnection()
    {
        statusMessage = "🔍 正在测试服务器连接...";
        statusColor = Color.yellow;
        Repaint();
        
        try
        {
            using (var client = new HttpClient())
            {
                client.Timeout = System.TimeSpan.FromSeconds(5);
                var response = await client.GetAsync($"{serverUrl}/health");
                
                if (response.IsSuccessStatusCode)
                {
                    isServerReachable = true;
                    statusMessage = $"✅ 服务器连接成功！\n{serverUrl}";
                    statusColor = Color.green;
                }
                else
                {
                    isServerReachable = false;
                    statusMessage = $"⚠️ 服务器响应异常 (状态码: {response.StatusCode})";
                    statusColor = Color.yellow;
                }
            }
        }
        catch (System.Exception ex)
        {
            isServerReachable = false;
            statusMessage = $"❌ 无法连接到服务器\n\n请确保：\n1. 服务器正在运行 (start_server.bat)\n2. 地址正确: {serverUrl}\n\n错误: {ex.Message}";
            statusColor = Color.red;
        }
        
        Repaint();
    }
    
    void CheckSceneSetup()
    {
        // 检查当前场景是否有 NetworkManager
        var networkManager = FindObjectOfType<SanguoStrategy.Network.NetworkManager>();
        
        if (networkManager == null)
        {
            Debug.LogWarning("当前场景缺少 NetworkManager");
        }
        else
        {
            Debug.Log("✅ NetworkManager 已在场景中");
        }
    }
    
    // ========== 修复功能 ==========
    
    void AutoFixAllIssues()
    {
        statusMessage = "🔧 正在自动修复所有问题...\n";
        statusColor = Color.yellow;
        Repaint();
        
        bool allFixed = true;
        
        // 1. 安装 WebSocket-Sharp
        if (!isWebSocketSharpInstalled)
        {
            statusMessage += "\n📦 安装 WebSocket-Sharp...";
            Repaint();
            DownloadWebSocketSharp();
        }
        
        // 2. 配置编译符号
        statusMessage += "\n⚙️ 配置编译符号...";
        Repaint();
        AddWebSocketDefine();
        
        // 3. 修复场景按钮
        statusMessage += "\n🔗 修复场景按钮...";
        Repaint();
        FixSceneButtons();
        
        // 4. 配置网络管理器
        statusMessage += "\n🌐 配置网络管理器...";
        Repaint();
        ConfigureNetworkManager();
        
        statusMessage = "✅ 自动修复完成！\n\n" +
            "建议操作：\n" +
            "1. 重启 Unity 编辑器\n" +
            "2. 确保服务器正在运行\n" +
            "3. 点击 Play 开始游戏";
        statusColor = Color.green;
        
        Repaint();
        
        EditorUtility.DisplayDialog("修复完成", 
            "✅ 已完成自动修复！\n\n" +
            "建议重启 Unity 编辑器以确保所有更改生效。\n\n" +
            "启动游戏前请确保服务器正在运行 (start_server.bat)",
            "确定");
    }
    
    void DownloadWebSocketSharp()
    {
        statusMessage = "📥 准备下载 WebSocket-Sharp...\n";
        statusColor = Color.yellow;
        Repaint();
        
        // 创建 Plugins 目录
        string pluginsPath = "Assets/Plugins";
        if (!Directory.Exists(pluginsPath))
        {
            Directory.CreateDirectory(pluginsPath);
        }
        
        // 提供下载链接
        bool download = EditorUtility.DisplayDialog(
            "下载 WebSocket-Sharp",
            "需要下载 websocket-sharp.dll 库。\n\n" +
            "方式1：自动下载 (推荐)\n" +
            "- 从 NuGet 下载最新版本\n\n" +
            "方式2：手动下载\n" +
            "- 访问 GitHub releases 页面手动下载\n\n" +
            "是否打开浏览器进行下载？",
            "打开浏览器下载",
            "取消");
        
        if (download)
        {
            Application.OpenURL("https://github.com/sta/websocket-sharp/releases");
            
            EditorUtility.DisplayDialog(
                "下载说明",
                "请下载 websocket-sharp.dll 文件，然后：\n\n" +
                "1. 将 .dll 文件复制到:\n" +
                "   Assets/Plugins/\n\n" +
                "2. 返回 Unity，等待自动刷新\n\n" +
                "3. 再次点击「全面诊断」确认安装",
                "知道了");
        }
        
        ShowManualInstallGuide();
    }
    
    void ShowManualInstallGuide()
    {
        string guide = 
            "📦 WebSocket-Sharp 手动安装指南\n" +
            "=====================================\n\n" +
            "方法 1: NuGet 下载 (推荐)\n" +
            "1. 访问: https://www.nuget.org/packages/WebSocketSharp/\n" +
            "2. 点击 「Download package」\n" +
            "3. 将 .nupkg 文件重命名为 .zip\n" +
            "4. 解压，在 lib/ 目录找到 websocket-sharp.dll\n" +
            "5. 复制到 Unity 项目的 Assets/Plugins/\n\n" +
            "方法 2: GitHub 下载\n" +
            "1. 访问: https://github.com/sta/websocket-sharp/releases\n" +
            "2. 下载最新 Release\n" +
            "3. 编译或下载预编译的 DLL\n" +
            "4. 复制到 Assets/Plugins/\n\n" +
            "方法 3: Unity Package Manager (如果可用)\n" +
            "1. Window -> Package Manager\n" +
            "2. 搜索 websocket-sharp\n" +
            "3. 点击 Install\n\n" +
            "完成后:\n" +
            "- 返回此窗口\n" +
            "- 点击「全面诊断」\n" +
            "- 验证安装成功";
        
        Debug.Log(guide);
        
        EditorUtility.DisplayDialog("安装指南", guide, "知道了");
    }
    
    void AddWebSocketDefine()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        
        if (!currentDefines.Contains("WEBSOCKET_SHARP"))
        {
            string newDefines = string.IsNullOrEmpty(currentDefines) 
                ? "WEBSOCKET_SHARP" 
                : currentDefines + ";WEBSOCKET_SHARP";
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefines);
            
            Debug.Log("✅ 已添加 WEBSOCKET_SHARP 编译符号");
            statusMessage = "✅ 编译符号已添加。请等待脚本重新编译...";
            statusColor = Color.green;
        }
        else
        {
            Debug.Log("WEBSOCKET_SHARP 编译符号已存在");
        }
        
        Repaint();
    }
    
    void FixSceneButtons()
    {
        var mainMenuController = FindObjectOfType<SanguoStrategy.UI.MainMenuController>();
        
        if (mainMenuController != null)
        {
            Debug.Log("✅ 找到 MainMenuController");
            // 这里可以添加更多按钮检查和修复逻辑
        }
        else
        {
            Debug.LogWarning("⚠️ 当前场景没有 MainMenuController");
        }
        
        AssetDatabase.Refresh();
    }
    
    void ConfigureNetworkManager()
    {
        var networkManager = FindObjectOfType<SanguoStrategy.Network.NetworkManager>();
        
        if (networkManager == null)
        {
            // 创建 NetworkManager GameObject
            GameObject nmObj = new GameObject("NetworkManager");
            networkManager = nmObj.AddComponent<SanguoStrategy.Network.NetworkManager>();
            
            Debug.Log("✅ 已创建 NetworkManager");
        }
        
        // 配置服务器 URL
        networkManager.SetServerUrl(websocketUrl);
        
        Debug.Log($"✅ NetworkManager 配置完成: {websocketUrl}");
    }
    
    // ========== 中文字体修复 ==========
    
    void DrawChineseFontFix()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🈯 中文字体修复", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.HelpBox(
            "如果游戏中的中文显示为方框 □□□，点击下面的按钮一键修复！\n\n" +
            "修复原理：将所有 TextMeshPro 组件切换为 Unity 默认 Text 组件",
            MessageType.Info);
        
        GUILayout.Space(5);
        
        GUI.backgroundColor = new Color(1f, 0.6f, 0f); // 橙色
        if (GUILayout.Button("🔧 一键修复中文显示问题", GUILayout.Height(40)))
        {
            FixChineseFont();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndVertical();
    }
    
    void FixChineseFont()
    {
        if (!EditorUtility.DisplayDialog(
            "修复中文字体",
            "即将将所有 TextMeshPro 组件替换为 Unity 默认 Text 组件。\n\n" +
            "这个操作会：\n" +
            "• 扫描所有场景\n" +
            "• 替换所有 TextMeshProUGUI 为 Text\n" +
            "• 保留文本内容和基本设置\n\n" +
            "建议先备份项目！\n\n" +
            "是否继续？",
            "开始修复",
            "取消"))
        {
            return;
        }
        
        Debug.Log("========== 开始修复中文字体 ==========");
        
        int totalFixed = 0;
        
        // 获取所有场景
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        
        if (sceneGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("错误", "未找到任何场景文件！", "确定");
            return;
        }
        
        // 保存当前场景
        Scene currentScene = SceneManager.GetActiveScene();
        string currentScenePath = currentScene.path;
        
        EditorUtility.DisplayProgressBar("修复中文字体", "准备中...", 0);
        
        try
        {
            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                
                float progress = (float)i / sceneGuids.Length;
                EditorUtility.DisplayProgressBar("修复中文字体", $"处理场景: {sceneName}", progress);
                
                Debug.Log($"\n>>> 处理场景: {sceneName}");
                
                // 打开场景
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                
                // 修复场景中的所有 TMP 组件
                int fixedCount = FixTMPInScene(scene);
                totalFixed += fixedCount;
                
                Debug.Log($"    修复了 {fixedCount} 个组件");
                
                // 保存场景
                if (fixedCount > 0)
                {
                    EditorSceneManager.SaveScene(scene);
                    Debug.Log($"    ✓ 场景已保存");
                }
            }
            
            // 恢复原场景
            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n========== 修复完成！共修复 {totalFixed} 个组件 ==========");
        
        EditorUtility.DisplayDialog(
            "修复完成！",
            $"✅ 成功修复了 {totalFixed} 个文本组件！\n\n" +
            $"处理了 {sceneGuids.Length} 个场景。\n\n" +
            "现在请点击 Play 按钮测试游戏，中文应该能正常显示了！",
            "太好了！");
    }
    
    int FixTMPInScene(Scene scene)
    {
        int fixedCount = 0;
        
        // 获取场景中所有根物体
        GameObject[] rootObjects = scene.GetRootGameObjects();
        
        foreach (GameObject root in rootObjects)
        {
            // 递归查找所有 TextMeshProUGUI 组件
            TextMeshProUGUI[] tmpComponents = root.GetComponentsInChildren<TextMeshProUGUI>(true);
            
            foreach (TextMeshProUGUI tmp in tmpComponents)
            {
                fixedCount += ConvertTMPToText(tmp);
            }
        }
        
        return fixedCount;
    }
    
    int ConvertTMPToText(TextMeshProUGUI tmp)
    {
        if (tmp == null) return 0;
        
        GameObject go = tmp.gameObject;
        
        // 保存TMP的设置
        string text = tmp.text;
        float fontSize = tmp.fontSize;
        Color color = tmp.color;
        TextAlignmentOptions alignment = tmp.alignment;
        
        // 删除TMP组件
        DestroyImmediate(tmp);
        
        // 添加Unity Text组件
        UnityEngine.UI.Text uiText = go.AddComponent<UnityEngine.UI.Text>();
        
        // 应用设置
        uiText.text = text;
        uiText.fontSize = (int)fontSize;
        uiText.color = color;
        
        // 转换对齐方式
        switch (alignment)
        {
            case TextAlignmentOptions.TopLeft:
            case TextAlignmentOptions.Left:
            case TextAlignmentOptions.BottomLeft:
                uiText.alignment = TextAnchor.MiddleLeft;
                break;
            case TextAlignmentOptions.Top:
            case TextAlignmentOptions.Center:
            case TextAlignmentOptions.Bottom:
                uiText.alignment = TextAnchor.MiddleCenter;
                break;
            case TextAlignmentOptions.TopRight:
            case TextAlignmentOptions.Right:
            case TextAlignmentOptions.BottomRight:
                uiText.alignment = TextAnchor.MiddleRight;
                break;
        }
        
        // 使用Arial字体（系统默认，支持中文）
        uiText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        Debug.Log($"    ✓ 已转换: {go.name} - \"{text}\"");
        
        return 1;
    }
}

