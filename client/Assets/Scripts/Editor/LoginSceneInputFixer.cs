using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 登录场景输入框修复工具
    /// 解决InputField无法输入的问题
    /// </summary>
    public class LoginSceneInputFixer : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showDebugInfo = true;

        [MenuItem("三国策略/修复工具/修复登录输入框 &L")]
        public static void ShowWindow()
        {
            var window = GetWindow<LoginSceneInputFixer>("登录输入修复");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("此工具可以修复登录场景中InputField无法输入的问题", MessageType.Info);

            EditorGUILayout.Space(10);

            // 主要功能按钮
            if (GUILayout.Button("🔧 一键修复所有输入框", GUILayout.Height(40)))
            {
                FixAllInputFields();
            }

            EditorGUILayout.Space(10);

            // 分步修复
            EditorGUILayout.LabelField("分步修复:", EditorStyles.boldLabel);

            if (GUILayout.Button("1. 检查并修复EventSystem"))
            {
                FixEventSystem();
            }

            if (GUILayout.Button("2. 修复所有TMP_InputField组件"))
            {
                FixTMPInputFields();
            }

            if (GUILayout.Button("3. 修复Canvas设置"))
            {
                FixCanvasSettings();
            }

            if (GUILayout.Button("4. 修复LoginController引用"))
            {
                FixLoginControllerReferences();
            }

            EditorGUILayout.Space(10);

            // 调试选项
            showDebugInfo = EditorGUILayout.Toggle("显示调试信息", showDebugInfo);

            EditorGUILayout.Space(10);

            // 快速测试
            EditorGUILayout.LabelField("快速测试:", EditorStyles.boldLabel);

            if (GUILayout.Button("▶ 运行场景测试"))
            {
                if (!Application.isPlaying)
                {
                    EditorApplication.isPlaying = true;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 一键修复所有输入框问题
        /// </summary>
        private void FixAllInputFields()
        {
            if (!CheckSceneOpened())
                return;

            int fixCount = 0;

            fixCount += FixEventSystem();
            fixCount += FixCanvasSettings();
            fixCount += FixTMPInputFields();
            fixCount += FixLoginControllerReferences();

            SaveScene();

            string message = $"✅ 修复完成！共修复 {fixCount} 个问题。\n\n现在可以点击 Play 测试输入功能。";
            EditorUtility.DisplayDialog("修复完成", message, "确定");
            Debug.Log(message);
        }

        /// <summary>
        /// 检查EventSystem
        /// </summary>
        private int FixEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                
                LogInfo("✅ 已创建 EventSystem");
                return 1;
            }

            // 检查StandaloneInputModule
            if (eventSystem.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                LogInfo("✅ 已添加 StandaloneInputModule");
                return 1;
            }

            LogInfo("✓ EventSystem 正常");
            return 0;
        }

        /// <summary>
        /// 修复Canvas设置
        /// </summary>
        private int FixCanvasSettings()
        {
            int fixCount = 0;
            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>(true);

            foreach (var canvas in canvases)
            {
                // 确保Canvas有GraphicRaycaster
                if (canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                    LogInfo($"✅ 为 {canvas.name} 添加了 GraphicRaycaster");
                    fixCount++;
                }

                // 确保Canvas Scaler存在
                if (canvas.GetComponent<CanvasScaler>() == null)
                {
                    var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    LogInfo($"✅ 为 {canvas.name} 添加了 CanvasScaler");
                    fixCount++;
                }
            }

            if (fixCount == 0)
            {
                LogInfo("✓ Canvas 设置正常");
            }

            return fixCount;
        }

        /// <summary>
        /// 修复TMP_InputField
        /// </summary>
        private int FixTMPInputFields()
        {
            int fixCount = 0;
            TMP_InputField[] inputFields = GameObject.FindObjectsOfType<TMP_InputField>(true);

            LogInfo($"找到 {inputFields.Length} 个 TMP_InputField");

            foreach (var inputField in inputFields)
            {
                bool fieldFixed = false;

                // 1. 确保有TextMeshProUGUI作为文本组件
                if (inputField.textComponent == null)
                {
                    var textComponent = inputField.GetComponentInChildren<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        inputField.textComponent = textComponent;
                        LogInfo($"✅ 为 {inputField.name} 设置了文本组件");
                        fieldFixed = true;
                    }
                    else
                    {
                        LogWarning($"⚠️ {inputField.name} 缺少TextMeshProUGUI组件！");
                    }
                }

                // 2. 确保Interactable
                if (!inputField.interactable)
                {
                    inputField.interactable = true;
                    LogInfo($"✅ 启用了 {inputField.name} 的交互");
                    fieldFixed = true;
                }

                // 3. 检查Placeholder
                if (inputField.placeholder == null)
                {
                    var placeholderObj = inputField.transform.Find("Placeholder");
                    if (placeholderObj != null)
                    {
                        var placeholderText = placeholderObj.GetComponent<TextMeshProUGUI>();
                        if (placeholderText != null)
                        {
                            inputField.placeholder = placeholderText;
                            LogInfo($"✅ 为 {inputField.name} 设置了占位符");
                            fieldFixed = true;
                        }
                    }
                }

                // 4. 确保有正确的图像组件
                Image image = inputField.GetComponent<Image>();
                if (image == null)
                {
                    image = inputField.gameObject.AddComponent<Image>();
                    image.color = new Color(1, 1, 1, 0.3f);
                    LogInfo($"✅ 为 {inputField.name} 添加了背景图像");
                    fieldFixed = true;
                }

                // 5. 设置字符限制（如果是密码框）
                if (inputField.name.Contains("Password") || inputField.name.Contains("password"))
                {
                    if (inputField.contentType != TMP_InputField.ContentType.Password)
                    {
                        inputField.contentType = TMP_InputField.ContentType.Password;
                        LogInfo($"✅ 将 {inputField.name} 设置为密码类型");
                        fieldFixed = true;
                    }
                }

                // 6. 确保字体不为空
                if (inputField.textComponent != null && inputField.textComponent.font == null)
                {
                    // 尝试加载默认字体
                    var defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                    if (defaultFont != null)
                    {
                        inputField.textComponent.font = defaultFont;
                        LogInfo($"✅ 为 {inputField.name} 设置了默认字体");
                        fieldFixed = true;
                    }
                }

                if (fieldFixed)
                {
                    fixCount++;
                    EditorUtility.SetDirty(inputField);
                }
            }

            if (fixCount == 0 && inputFields.Length > 0)
            {
                LogInfo("✓ 所有 InputField 配置正常");
            }

            return fixCount;
        }

        /// <summary>
        /// 修复LoginController引用
        /// </summary>
        private int FixLoginControllerReferences()
        {
            var loginController = GameObject.FindObjectOfType<SanguoStrategy.UI.LoginController>();

            if (loginController == null)
            {
                LogWarning("⚠️ 场景中没有找到 LoginController！");
                return 0;
            }

            int fixCount = 0;

            // 使用反射检查所有SerializeField
            var fields = loginController.GetType().GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public
            );

            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(SerializeField), true).Length > 0 ||
                    field.IsPublic)
                {
                    var value = field.GetValue(loginController);
                    if (value == null || value.Equals(null))
                    {
                        // 尝试自动查找并赋值
                        if (field.FieldType == typeof(GameObject))
                        {
                            var obj = GameObject.Find(field.Name);
                            if (obj != null)
                            {
                                field.SetValue(loginController, obj);
                                LogInfo($"✅ 自动设置 {field.Name}");
                                fixCount++;
                            }
                        }
                        else if (field.FieldType == typeof(Button))
                        {
                            var buttons = GameObject.FindObjectsOfType<Button>(true);
                            foreach (var btn in buttons)
                            {
                                if (btn.name.ToLower().Contains(field.Name.ToLower().Replace("button", "")))
                                {
                                    field.SetValue(loginController, btn);
                                    LogInfo($"✅ 自动设置按钮 {field.Name} -> {btn.name}");
                                    fixCount++;
                                    break;
                                }
                            }
                        }
                        else if (field.FieldType == typeof(TMP_InputField))
                        {
                            var inputs = GameObject.FindObjectsOfType<TMP_InputField>(true);
                            foreach (var input in inputs)
                            {
                                if (input.name.ToLower().Contains(field.Name.ToLower().Replace("input", "").Replace("field", "")))
                                {
                                    field.SetValue(loginController, input);
                                    LogInfo($"✅ 自动设置输入框 {field.Name} -> {input.name}");
                                    fixCount++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (fixCount > 0)
            {
                EditorUtility.SetDirty(loginController);
            }
            else
            {
                LogInfo("✓ LoginController 引用正常");
            }

            return fixCount;
        }

        /// <summary>
        /// 检查场景是否已打开
        /// </summary>
        private bool CheckSceneOpened()
        {
            Scene currentScene = SceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(currentScene.path))
            {
                EditorUtility.DisplayDialog("错误", "请先打开登录场景（Login.unity）", "确定");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存场景
        /// </summary>
        private void SaveScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            LogInfo("✅ 场景已保存");
        }

        private void LogInfo(string message)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[LoginInputFixer] {message}");
            }
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"[LoginInputFixer] {message}");
        }
    }
}

