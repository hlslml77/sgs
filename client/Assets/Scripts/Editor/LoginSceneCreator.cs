using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 登录场景创建器 - 自动创建登录/注册UI
    /// </summary>
    public class LoginSceneCreator : EditorWindow
    {
        private TMP_FontAsset chineseFont;
        
        [MenuItem("三国策略/场景管理/创建登录场景")]
        public static void ShowWindow()
        {
            GetWindow<LoginSceneCreator>("创建登录场景");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("登录场景创建器", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具会创建一个完整的登录/注册场景，包括：\n" +
                "• 登录面板（用户名、密码）\n" +
                "• 注册面板（用户名、邮箱、密码、确认密码）\n" +
                "• 消息提示\n" +
                "• 加载动画\n" +
                "• 自动绑定所有引用",
                MessageType.Info
            );
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("创建登录场景", GUILayout.Height(40)))
            {
                CreateLoginScene();
            }
        }
        
        private void CreateLoginScene()
        {
            // 加载中文字体
            chineseFont = LoadChineseFont();
            
            // 创建新场景
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // 确保有 EventSystem
            if (GameObject.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                Debug.Log("✅ 已添加 EventSystem");
            }
            
            // 创建 Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 创建背景
            GameObject bgObj = new GameObject("BackgroundImage");
            bgObj.transform.SetParent(canvasObj.transform, false);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            // 创建登录面板
            GameObject loginPanel = CreateLoginPanel(canvasObj.transform);
            
            // 创建注册面板
            GameObject registerPanel = CreateRegisterPanel(canvasObj.transform);
            registerPanel.SetActive(false);
            
            // 创建消息文本
            GameObject messageObj = CreateMessageText(canvasObj.transform);
            messageObj.SetActive(false);
            
            // 创建加载面板
            GameObject loadingPanel = CreateLoadingPanel(canvasObj.transform);
            loadingPanel.SetActive(false);
            
            // 创建LoginController
            GameObject controllerObj = new GameObject("LoginController");
            var controller = controllerObj.AddComponent<SanguoStrategy.UI.LoginController>();
            
            // 使用反射设置私有字段
            var controllerType = controller.GetType();
            
            SetField(controller, "loginPanel", loginPanel);
            SetField(controller, "registerPanel", registerPanel);
            
            SetField(controller, "loginUsernameInput", loginPanel.transform.Find("UsernameInput").GetComponent<TMP_InputField>());
            SetField(controller, "loginPasswordInput", loginPanel.transform.Find("PasswordInput").GetComponent<TMP_InputField>());
            SetField(controller, "loginButton", loginPanel.transform.Find("LoginButton").GetComponent<Button>());
            SetField(controller, "showRegisterButton", loginPanel.transform.Find("ShowRegisterButton").GetComponent<Button>());
            
            SetField(controller, "registerUsernameInput", registerPanel.transform.Find("UsernameInput").GetComponent<TMP_InputField>());
            SetField(controller, "registerEmailInput", registerPanel.transform.Find("EmailInput").GetComponent<TMP_InputField>());
            SetField(controller, "registerPasswordInput", registerPanel.transform.Find("PasswordInput").GetComponent<TMP_InputField>());
            SetField(controller, "registerConfirmPasswordInput", registerPanel.transform.Find("ConfirmPasswordInput").GetComponent<TMP_InputField>());
            SetField(controller, "registerButton", registerPanel.transform.Find("RegisterButton").GetComponent<Button>());
            SetField(controller, "showLoginButton", registerPanel.transform.Find("ShowLoginButton").GetComponent<Button>());
            
            SetField(controller, "messageText", messageObj.GetComponent<TextMeshProUGUI>());
            SetField(controller, "loadingPanel", loadingPanel);
            
            // 保存场景
            string scenePath = "Assets/Scenes/Login.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            
            Debug.Log($"✅ 登录场景创建成功: {scenePath}");
            EditorUtility.DisplayDialog("成功", "登录场景创建成功！\n\n请将Login场景添加到Build Settings，并设为启动场景。", "确定");
        }
        
        private GameObject CreateLoginPanel(Transform parent)
        {
            GameObject panel = new GameObject("LoginPanel");
            panel.transform.SetParent(parent, false);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.85f);
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 450);
            
            // 标题
            CreateText(panel.transform, "TitleText", "三国策略", 48, TextAlignmentOptions.Center, new Vector2(0, 150));
            
            // 用户名输入
            CreateInputField(panel.transform, "UsernameInput", "用户名", new Vector2(0, 50));
            
            // 密码输入
            var passwordInput = CreateInputField(panel.transform, "PasswordInput", "密码", new Vector2(0, -20));
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            
            // 登录按钮
            CreateButton(panel.transform, "LoginButton", "登录", new Color(0.2f, 0.7f, 0.3f), new Vector2(0, -100));
            
            // 切换到注册
            CreateButton(panel.transform, "ShowRegisterButton", "没有账号？点击注册", new Color(0.3f, 0.3f, 0.3f), new Vector2(0, -160), 14);
            
            return panel;
        }
        
        private GameObject CreateRegisterPanel(Transform parent)
        {
            GameObject panel = new GameObject("RegisterPanel");
            panel.transform.SetParent(parent, false);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.85f);
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 550);
            
            // 标题
            CreateText(panel.transform, "TitleText", "注册账号", 48, TextAlignmentOptions.Center, new Vector2(0, 200));
            
            // 用户名输入
            CreateInputField(panel.transform, "UsernameInput", "用户名 (3-20字符)", new Vector2(0, 100));
            
            // 邮箱输入
            CreateInputField(panel.transform, "EmailInput", "邮箱", new Vector2(0, 30));
            
            // 密码输入
            var passwordInput = CreateInputField(panel.transform, "PasswordInput", "密码 (至少6字符)", new Vector2(0, -40));
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            
            // 确认密码输入
            var confirmInput = CreateInputField(panel.transform, "ConfirmPasswordInput", "确认密码", new Vector2(0, -110));
            confirmInput.contentType = TMP_InputField.ContentType.Password;
            
            // 注册按钮
            CreateButton(panel.transform, "RegisterButton", "注册", new Color(0.3f, 0.5f, 0.9f), new Vector2(0, -180));
            
            // 切换到登录
            CreateButton(panel.transform, "ShowLoginButton", "已有账号？点击登录", new Color(0.3f, 0.3f, 0.3f), new Vector2(0, -240), 14);
            
            return panel;
        }
        
        private GameObject CreateMessageText(Transform parent)
        {
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(parent, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 100);
            rect.sizeDelta = new Vector2(600, 50);
            
            return textObj;
        }
        
        private GameObject CreateLoadingPanel(Transform parent)
        {
            GameObject panel = new GameObject("LoadingPanel");
            panel.transform.SetParent(parent, false);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            CreateText(panel.transform, "LoadingText", "加载中...", 32, TextAlignmentOptions.Center, Vector2.zero);
            
            return panel;
        }
        
        private TextMeshProUGUI CreateText(Transform parent, string name, string content, int fontSize, 
                                          TextAlignmentOptions alignment, Vector2 position)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            
            // 应用中文字体
            if (chineseFont != null)
            {
                text.font = chineseFont;
            }
            
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(400, fontSize + 20);
            
            return text;
        }
        
        private TMP_InputField CreateInputField(Transform parent, string name, string placeholder, Vector2 position)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            Image inputImage = inputObj.AddComponent<Image>();
            inputImage.color = new Color(0.2f, 0.2f, 0.2f);
            
            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
            
            RectTransform rect = inputObj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(400, 50);
            
            // 创建文本区域
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(inputObj.transform, false);
            RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.sizeDelta = Vector2.zero;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);
            
            // 创建占位符
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textArea.transform, false);
            TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 18;
            placeholderText.color = new Color(0.6f, 0.6f, 0.6f);
            placeholderText.enableWordWrapping = false;
            if (chineseFont != null) placeholderText.font = chineseFont;
            RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.sizeDelta = Vector2.zero;
            
            // 创建输入文本
            GameObject inputTextObj = new GameObject("Text");
            inputTextObj.transform.SetParent(textArea.transform, false);
            TextMeshProUGUI inputText = inputTextObj.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 18;
            inputText.color = Color.white;
            inputText.enableWordWrapping = false;
            if (chineseFont != null) inputText.font = chineseFont;
            RectTransform inputTextRect = inputTextObj.GetComponent<RectTransform>();
            inputTextRect.anchorMin = Vector2.zero;
            inputTextRect.anchorMax = Vector2.one;
            inputTextRect.sizeDelta = Vector2.zero;
            
            // 配置InputField
            inputField.textViewport = textAreaRect;
            inputField.textComponent = inputText;
            inputField.placeholder = placeholderText;
            
            return inputField;
        }
        
        private Button CreateButton(Transform parent, string name, string text, Color color, Vector2 position, int fontSize = 20)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = color;
            
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(400, 50);
            
            // 创建按钮文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.fontSize = fontSize;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            if (chineseFont != null) buttonText.font = chineseFont;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            return button;
        }
        
        private void SetField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"字段 {fieldName} 未找到");
            }
        }
        
        private TMP_FontAsset LoadChineseFont()
        {
            // 尝试查找现有的中文字体
            string[] fontPaths = new string[]
            {
                "Assets/Resources/Fonts/SourceHanSansCN-Regular SDF.asset",
                "Assets/TextMesh Pro/Resources/Fonts & Materials/SourceHanSansCN-Regular SDF.asset",
                "Assets/Resources/Fonts/NotoSansCJK-Regular SDF.asset",
                "Assets/TextMesh Pro/Resources/Fonts & Materials/NotoSansCJK-Regular SDF.asset",
            };
            
            foreach (string path in fontPaths)
            {
                TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                if (font != null)
                {
                    Debug.Log($"✅ 找到中文字体: {path}");
                    return font;
                }
            }
            
            // 使用默认的 LiberationSans SDF
            TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (defaultFont != null)
            {
                Debug.LogWarning("⚠️ 未找到中文字体，使用默认字体。文本可能显示为方块。");
                Debug.LogWarning("建议：Window -> TextMeshPro -> Font Asset Creator 创建中文字体");
                return defaultFont;
            }
            
            Debug.LogError("❌ 无法找到任何可用字体！");
            return null;
        }
    }
}

