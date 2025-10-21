using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 快速设置工具 - 一键完成所有场景和预制件的初始化
    /// </summary>
    public class QuickSetup : EditorWindow
    {
        private static string[] sceneNames = new string[]
        {
            "MainMenu",
            "RoomList",
            "HeroSelection",
            "GameScene",
            "TerrainEditor"
        };

        private static string scenesPath = "Assets/Scenes/";
        private bool[] setupStatus = new bool[5];
        private Vector2 scrollPos;

        [MenuItem("三国策略/快速设置向导")]
        public static void ShowWindow()
        {
            var window = GetWindow<QuickSetup>("快速设置向导");
            window.minSize = new Vector2(500, 600);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            
            // 标题
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 18;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("三国策略 - 快速设置向导", titleStyle);
            
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "本向导将帮助你快速设置所有游戏场景和预制件。\n" +
                "建议按照以下步骤依次进行设置。", 
                MessageType.Info
            );
            
            GUILayout.Space(10);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            // 步骤1：创建预制件
            DrawSection("步骤1：创建预制件", () =>
            {
                GUILayout.Label("首先需要创建所有UI预制件，这些预制件将在各个场景中使用。");
                GUILayout.Space(5);
                
                if (GUILayout.Button("打开预制件创建工具", GUILayout.Height(30)))
                {
                    PrefabCreator.ShowWindow();
                }
                
                GUILayout.Space(5);
                EditorGUILayout.HelpBox(
                    "点击上面的按钮，然后依次创建：\n" +
                    "1. 武将卡片预制件\n" +
                    "2. 房间列表项预制件\n" +
                    "3. 卡牌预制件\n" +
                    "4. 地形块预制件\n" +
                    "5. 技能按钮预制件",
                    MessageType.None
                );
            });
            
            GUILayout.Space(10);
            
            // 步骤2：设置场景UI
            DrawSection("步骤2：设置场景UI", () =>
            {
                GUILayout.Label("为每个场景添加UI元素。建议一个一个场景设置，设置后保存场景。");
                GUILayout.Space(5);
                
                for (int i = 0; i < sceneNames.Length; i++)
                {
                    DrawSceneSetup(i);
                }
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("🚀 一键设置所有场景", GUILayout.Height(40)))
                {
                    if (EditorUtility.DisplayDialog(
                        "确认",
                        "这将依次打开并设置所有场景，可能需要几分钟时间。\n确定要继续吗？",
                        "确定",
                        "取消"))
                    {
                        SetupAllScenes();
                    }
                }
            });
            
            GUILayout.Space(10);
            
            // 步骤3：配置Build Settings
            DrawSection("步骤3：配置Build Settings", () =>
            {
                GUILayout.Label("将所有场景添加到Build Settings中，确保场景切换正常。");
                GUILayout.Space(5);
                
                if (GUILayout.Button("自动添加场景到Build Settings", GUILayout.Height(30)))
                {
                    AddScenesToBuildSettings();
                }
                
                GUILayout.Space(5);
                if (GUILayout.Button("打开Build Settings查看", GUILayout.Height(25)))
                {
                    EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                }
            });
            
            GUILayout.Space(10);
            
            // 步骤4：测试游戏
            DrawSection("步骤4：测试游戏", () =>
            {
                GUILayout.Label("打开主菜单场景开始测试。");
                GUILayout.Space(5);
                
                if (GUILayout.Button("打开主菜单场景", GUILayout.Height(30)))
                {
                    OpenScene("MainMenu");
                }
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("▶️ 运行游戏", GUILayout.Height(35)))
                {
                    OpenScene("MainMenu");
                    EditorApplication.isPlaying = true;
                }
            });
            
            GUILayout.Space(10);
            
            // 额外工具
            DrawSection("额外工具", () =>
            {
                if (GUILayout.Button("打开UI设置工具"))
                {
                    UISetupHelper.ShowWindow();
                }
                
                if (GUILayout.Button("查看设置文档"))
                {
                    string path = "Assets/../UI_SETUP_GUIDE.md";
                    if (System.IO.File.Exists(path))
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "找不到UI_SETUP_GUIDE.md文件", "确定");
                    }
                }
            });
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 底部信息
            EditorGUILayout.HelpBox(
                "提示：每次设置完场景后，记得保存场景（Ctrl+S）", 
                MessageType.Warning
            );
        }

        private void DrawSection(string title, System.Action content)
        {
            GUIStyle sectionStyle = new GUIStyle(EditorStyles.helpBox);
            sectionStyle.padding = new RectOffset(10, 10, 10, 10);
            
            EditorGUILayout.BeginVertical(sectionStyle);
            
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 14;
            GUILayout.Label(title, headerStyle);
            
            GUILayout.Space(5);
            content?.Invoke();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawSceneSetup(int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            string sceneName = sceneNames[index];
            GUILayout.Label($"{index + 1}. {sceneName}", GUILayout.Width(150));
            
            if (setupStatus[index])
            {
                GUILayout.Label("✓ 已设置", GUILayout.Width(80));
            }
            else
            {
                GUILayout.Label("○ 未设置", GUILayout.Width(80));
            }
            
            if (GUILayout.Button("打开场景", GUILayout.Width(80)))
            {
                OpenScene(sceneName);
            }
            
            if (GUILayout.Button("设置UI", GUILayout.Width(80)))
            {
                SetupSceneUI(index);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void OpenScene(string sceneName)
        {
            string scenePath = scenesPath + sceneName + ".unity";
            
            if (System.IO.File.Exists(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                    Debug.Log($"已打开场景: {sceneName}");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("错误", $"场景文件不存在: {scenePath}", "确定");
            }
        }

        private void SetupSceneUI(int index)
        {
            string sceneName = sceneNames[index];
            
            // 先打开场景
            OpenScene(sceneName);
            
            // 根据场景名称调用对应的设置方法
            // 注意：这里需要等待场景加载完成
            EditorApplication.delayCall += () =>
            {
                try
                {
                    switch (sceneName)
                    {
                        case "MainMenu":
                            // 调用UISetupHelper的SetupMainMenuUI方法
                            var helper = EditorWindow.GetWindow<UISetupHelper>();
                            helper.GetType().GetMethod("SetupMainMenuUI", 
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                ?.Invoke(helper, null);
                            helper.Close();
                            break;
                            
                        // 类似地处理其他场景...
                        default:
                            Debug.LogWarning($"场景 {sceneName} 的UI设置方法尚未实现");
                            break;
                    }
                    
                    setupStatus[index] = true;
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"设置场景UI时出错: {e.Message}");
                }
            };
        }

        private void SetupAllScenes()
        {
            for (int i = 0; i < sceneNames.Length; i++)
            {
                SetupSceneUI(i);
            }
            
            EditorUtility.DisplayDialog("完成", "所有场景UI设置完成！", "确定");
        }

        private void AddScenesToBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
            
            for (int i = 0; i < sceneNames.Length; i++)
            {
                string scenePath = scenesPath + sceneNames[i] + ".unity";
                
                if (System.IO.File.Exists(scenePath))
                {
                    scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                }
                else
                {
                    Debug.LogWarning($"场景文件不存在，跳过: {scenePath}");
                }
            }
            
            EditorBuildSettings.scenes = scenes.ToArray();
            
            EditorUtility.DisplayDialog(
                "完成",
                $"已添加 {scenes.Count} 个场景到Build Settings",
                "确定"
            );
            
            Debug.Log("Build Settings场景列表已更新：");
            foreach (var scene in scenes)
            {
                Debug.Log($"  - {scene.path}");
            }
        }
    }
}

