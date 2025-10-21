using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 中文字体自动修复工具
/// 一键解决场景中所有Text组件的中文乱码问题
/// </summary>
public class ChineseFontFixer : EditorWindow
{
    private Font systemChineseFont;
    private bool useSystemFont = true;
    private string fontPath = "";
    private int fixedCount = 0;
    private Vector2 scrollPosition;

    [MenuItem("三国策略/修复中文字体显示")]
    public static void ShowWindow()
    {
        var window = GetWindow<ChineseFontFixer>("中文字体修复工具");
        window.minSize = new Vector2(450, 400);
        window.Show();
    }

    void OnEnable()
    {
        // 尝试加载系统字体
        TryLoadSystemFont();
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("中文字体修复工具", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Unity默认字体不支持中文，会显示为方块□。\n" +
            "此工具会自动为所有Text组件设置中文字体。", 
            MessageType.Info);

        GUILayout.Space(15);

        // 字体选择
        DrawSection("步骤1：选择字体来源");
        
        useSystemFont = EditorGUILayout.Toggle("使用系统字体（推荐）", useSystemFont);
        
        if (useSystemFont)
        {
            EditorGUILayout.HelpBox(
                "将使用Windows系统中的 Microsoft YaHei (微软雅黑) 字体。\n" +
                "如果系统没有此字体，会尝试使用 SimHei (黑体) 或 Arial Unicode。", 
                MessageType.Info);

            if (systemChineseFont != null)
            {
                EditorGUILayout.LabelField("✅ 系统字体已加载: " + systemChineseFont.name);
            }
            else
            {
                EditorGUILayout.LabelField("⚠️ 未找到系统中文字体");
                if (GUILayout.Button("重新检测系统字体"))
                {
                    TryLoadSystemFont();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("自定义字体路径（Assets/下的相对路径）：");
            fontPath = EditorGUILayout.TextField(fontPath);
            EditorGUILayout.HelpBox(
                "例如：Fonts/MyChineseFont\n" +
                "请先将字体文件放入 Assets/Fonts/ 文件夹", 
                MessageType.Info);
        }

        GUILayout.Space(15);

        // 修复操作
        DrawSection("步骤2：执行修复");

        EditorGUI.BeginDisabledGroup(useSystemFont && systemChineseFont == null);
        
        if (GUILayout.Button("🔧 修复当前场景的所有Text组件", GUILayout.Height(40)))
        {
            FixCurrentScene();
        }

        if (GUILayout.Button("🔧 修复所有打开场景的Text组件", GUILayout.Height(40)))
        {
            FixAllOpenScenes();
        }

        EditorGUI.EndDisabledGroup();

        if (fixedCount > 0)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox($"✅ 已修复 {fixedCount} 个Text组件！", MessageType.Info);
        }

        GUILayout.Space(15);

        // 高级选项
        DrawSection("高级选项");

        if (GUILayout.Button("📦 导出字体设置配置"))
        {
            ExportFontConfig();
        }

        if (GUILayout.Button("🔄 重新生成所有场景（带字体修复）"))
        {
            if (EditorUtility.DisplayDialog("确认", 
                "将重新生成所有场景UI，并自动应用中文字体。\n确定继续吗？", 
                "确定", "取消"))
            {
                RegenerateAllScenesWithFont();
            }
        }

        GUILayout.Space(15);

        // 说明
        DrawSection("提示");
        EditorGUILayout.HelpBox(
            "• 如果仍然显示乱码，请检查字体是否包含中文字符\n" +
            "• 推荐使用 TextMeshPro 获得更好的中文显示效果\n" +
            "• 可以在 Window → TextMeshPro → Font Asset Creator 创建字体资源", 
            MessageType.Info);

        EditorGUILayout.EndScrollView();
    }

    void DrawSection(string title)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    void TryLoadSystemFont()
    {
        // 尝试多个常见的中文系统字体
        string[] fontNames = {
            "Microsoft YaHei",
            "微软雅黑",
            "SimHei",
            "黑体",
            "Arial Unicode MS",
            "SimSun",
            "宋体"
        };

        foreach (string fontName in fontNames)
        {
            try
            {
                Font font = Font.CreateDynamicFontFromOSFont(fontName, 16);
                if (font != null && font.fontNames.Length > 0)
                {
                    systemChineseFont = font;
                    Debug.Log($"✅ 成功加载系统字体: {fontName}");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"无法加载字体 {fontName}: {ex.Message}");
            }
        }

        Debug.LogWarning("⚠️ 未找到合适的系统中文字体");
    }

    void FixCurrentScene()
    {
        fixedCount = 0;

        Font fontToUse = GetFontToUse();
        if (fontToUse == null)
        {
            EditorUtility.DisplayDialog("错误", "无法加载字体！", "确定");
            return;
        }

        // 查找场景中所有的Text组件
        Text[] textComponents = FindObjectsOfType<Text>(true);
        
        foreach (Text text in textComponents)
        {
            Undo.RecordObject(text, "Fix Chinese Font");
            text.font = fontToUse;
            EditorUtility.SetDirty(text);
            fixedCount++;
        }

        // 保存场景
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log($"✅ 当前场景修复完成！共修复 {fixedCount} 个Text组件");
        EditorUtility.DisplayDialog("完成", $"成功修复 {fixedCount} 个Text组件！", "确定");
    }

    void FixAllOpenScenes()
    {
        fixedCount = 0;

        Font fontToUse = GetFontToUse();
        if (fontToUse == null)
        {
            EditorUtility.DisplayDialog("错误", "无法加载字体！", "确定");
            return;
        }

        for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i++)
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
            
            foreach (GameObject rootObj in scene.GetRootGameObjects())
            {
                Text[] textComponents = rootObj.GetComponentsInChildren<Text>(true);
                
                foreach (Text text in textComponents)
                {
                    Undo.RecordObject(text, "Fix Chinese Font");
                    text.font = fontToUse;
                    EditorUtility.SetDirty(text);
                    fixedCount++;
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }

        Debug.Log($"✅ 所有场景修复完成！共修复 {fixedCount} 个Text组件");
        EditorUtility.DisplayDialog("完成", $"成功修复 {fixedCount} 个Text组件！", "确定");
    }

    Font GetFontToUse()
    {
        if (useSystemFont)
        {
            if (systemChineseFont == null)
                TryLoadSystemFont();
            return systemChineseFont;
        }
        else
        {
            if (string.IsNullOrEmpty(fontPath))
            {
                Debug.LogError("请指定字体路径！");
                return null;
            }

            Font font = Resources.Load<Font>(fontPath);
            if (font == null)
            {
                Debug.LogError($"无法加载字体: {fontPath}");
                return null;
            }

            return font;
        }
    }

    void ExportFontConfig()
    {
        Font font = GetFontToUse();
        if (font == null)
        {
            EditorUtility.DisplayDialog("错误", "无法加载字体！", "确定");
            return;
        }

        string config = $@"===============================
字体配置信息
===============================

字体名称: {font.name}
字体样式: {font.fontNames[0]}
动态字体: {font.dynamic}
字符数量: {font.characterInfo.Length}

此配置可用于其他场景或项目。
";

        Debug.Log(config);
        EditorUtility.DisplayDialog("字体配置", config, "确定");
    }

    void RegenerateAllScenesWithFont()
    {
        // 先修复字体
        Font fontToUse = GetFontToUse();
        if (fontToUse == null)
        {
            EditorUtility.DisplayDialog("错误", "无法加载字体！请先选择有效的字体", "确定");
            return;
        }

        // 保存字体设置到临时位置，让SceneSetupWizard使用
        EditorPrefs.SetString("ChineseFont_Name", fontToUse.name);
        EditorPrefs.SetBool("ChineseFont_IsSystemFont", useSystemFont);
        if (!useSystemFont)
            EditorPrefs.SetString("ChineseFont_Path", fontPath);

        Debug.Log("✅ 字体设置已保存，现在可以运行场景设置向导");
        
        // 尝试打开场景设置向导
        var wizardWindow = EditorWindow.GetWindow<SceneSetupWizard>("场景设置向导");
        if (wizardWindow != null)
        {
            wizardWindow.Show();
            EditorUtility.DisplayDialog("提示", 
                "字体设置已保存！\n\n请在场景设置向导窗口中点击「一键设置所有场景」按钮。\n\n新生成的场景将自动使用中文字体。", 
                "确定");
        }
    }
}

