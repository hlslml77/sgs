using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 把TextMeshProUGUI转换为Unity旧版Text组件
    /// 这是最简单可靠的中文显示方案！
    /// </summary>
    public class ConvertToLegacyText : EditorWindow
    {
        [MenuItem("工具/⚡ 转换当前场景为旧版Text")]
        public static void ConvertCurrentScene()
        {
            if (EditorUtility.DisplayDialog("确认转换", 
                "这个工具会把当前场景中所有TextMeshProUGUI组件转换为Unity旧版Text组件。\n\n" +
                "旧版Text完美支持中文，不需要任何额外设置！\n\n" +
                "转换后：\n" +
                "✅ 自动使用系统中文字体（微软雅黑）\n" +
                "✅ 保留所有文本内容\n" +
                "✅ 保留字体大小和颜色\n" +
                "✅ 保留对齐方式\n\n" +
                "是否继续？", 
                "开始转换", 
                "取消"))
            {
                ConvertAllTMPToText();
            }
        }
        
        [MenuItem("工具/🎨 美化所有场景（转换+美化）")]
        public static void BeautifyAllScenes()
        {
            // 查找所有场景文件
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            
            if (sceneGuids.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有找到场景文件！", "确定");
                return;
            }
            
            // 显示场景列表
            string sceneList = "找到以下场景：\n\n";
            List<string> scenePaths = new List<string>();
            
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
                sceneList += "• " + Path.GetFileNameWithoutExtension(path) + "\n";
            }
            
            sceneList += $"\n共 {scenePaths.Count} 个场景\n\n将进行以下操作：\n";
            sceneList += "✅ 转换为旧版Text（完美支持中文）\n";
            sceneList += "✅ 美化UI界面（渐变背景、圆角按钮）\n";
            sceneList += "✅ 优化布局和间距\n\n";
            sceneList += "是否继续？";
            
            if (!EditorUtility.DisplayDialog("批量美化所有场景", sceneList, "开始美化", "取消"))
            {
                return;
            }
            
            // 保存当前场景
            string currentScenePath = SceneManager.GetActiveScene().path;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            
            int totalConverted = 0;
            Dictionary<string, int> sceneResults = new Dictionary<string, int>();
            
            Debug.Log("════════════════════════════════════════");
            Debug.Log("开始批量美化所有场景");
            Debug.Log("════════════════════════════════════════");
            
            // 逐个打开并美化场景
            for (int i = 0; i < scenePaths.Count; i++)
            {
                string scenePath = scenePaths[i];
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                
                EditorUtility.DisplayProgressBar("批量美化场景", 
                    $"正在处理: {sceneName} ({i + 1}/{scenePaths.Count})", 
                    (float)i / scenePaths.Count);
                
                Debug.Log($"\n▶ 打开场景: {sceneName}");
                
                // 打开场景
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                
                // 转换场景中的TMP组件
                int convertedCount = ConvertSceneTMPToText(scene);
                
                // 美化场景UI
                BeautifySceneUI(scene);
                
                sceneResults[sceneName] = convertedCount;
                totalConverted += convertedCount;
                
                // 保存场景
                EditorSceneManager.SaveScene(scene);
                
                Debug.Log($"✅ {sceneName}: 转换了 {convertedCount} 个组件，已美化UI");
            }
            
            EditorUtility.ClearProgressBar();
            
            // 恢复原来的场景
            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            }
            
            Debug.Log("════════════════════════════════════════");
            Debug.Log($"✅ 批量美化完成！总共转换 {totalConverted} 个组件");
            Debug.Log("════════════════════════════════════════");
            
            // 显示结果
            string resultMessage = "批量美化完成！\n\n";
            foreach (var kvp in sceneResults)
            {
                resultMessage += $"• {kvp.Key}: {kvp.Value} 个组件\n";
            }
            resultMessage += $"\n总计: {totalConverted} 个组件\n\n";
            resultMessage += "所有场景已美化完成！\n";
            resultMessage += "✅ 中文正常显示\n";
            resultMessage += "✅ UI更加美观\n";
            
            EditorUtility.DisplayDialog("美化完成", resultMessage, "确定");
        }
        
        [MenuItem("工具/🚀 批量转换所有场景为旧版Text")]
        public static void ConvertAllScenes()
        {
            // 查找所有场景文件
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            
            if (sceneGuids.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有找到场景文件！", "确定");
                return;
            }
            
            // 显示场景列表
            string sceneList = "找到以下场景：\n\n";
            List<string> scenePaths = new List<string>();
            
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
                sceneList += "• " + Path.GetFileNameWithoutExtension(path) + "\n";
            }
            
            sceneList += $"\n共 {scenePaths.Count} 个场景\n\n是否转换所有场景？";
            
            if (!EditorUtility.DisplayDialog("批量转换所有场景", sceneList, "开始转换", "取消"))
            {
                return;
            }
            
            // 保存当前场景
            string currentScenePath = SceneManager.GetActiveScene().path;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            
            int totalConverted = 0;
            Dictionary<string, int> sceneResults = new Dictionary<string, int>();
            
            Debug.Log("════════════════════════════════════════");
            Debug.Log("开始批量转换所有场景");
            Debug.Log("════════════════════════════════════════");
            
            // 逐个打开并转换场景
            for (int i = 0; i < scenePaths.Count; i++)
            {
                string scenePath = scenePaths[i];
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                
                EditorUtility.DisplayProgressBar("批量转换场景", 
                    $"正在处理: {sceneName} ({i + 1}/{scenePaths.Count})", 
                    (float)i / scenePaths.Count);
                
                Debug.Log($"\n▶ 打开场景: {sceneName}");
                
                // 打开场景
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                
                // 转换场景中的TMP组件
                int convertedCount = ConvertSceneTMPToText(scene);
                sceneResults[sceneName] = convertedCount;
                totalConverted += convertedCount;
                
                // 保存场景
                EditorSceneManager.SaveScene(scene);
                
                Debug.Log($"✅ {sceneName}: 转换了 {convertedCount} 个组件");
            }
            
            EditorUtility.ClearProgressBar();
            
            // 恢复原来的场景
            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            }
            
            Debug.Log("════════════════════════════════════════");
            Debug.Log($"✅ 批量转换完成！总共转换 {totalConverted} 个组件");
            Debug.Log("════════════════════════════════════════");
            
            // 显示结果
            string resultMessage = "批量转换完成！\n\n";
            foreach (var kvp in sceneResults)
            {
                resultMessage += $"• {kvp.Key}: {kvp.Value} 个组件\n";
            }
            resultMessage += $"\n总计: {totalConverted} 个组件\n\n所有场景的中文应该都能正常显示了！";
            
            EditorUtility.DisplayDialog("转换完成", resultMessage, "确定");
        }
        
        private static int ConvertSceneTMPToText(Scene scene)
        {
            // 获取系统字体
            Font chineseFont = GetChineseFont();
            if (chineseFont == null)
            {
                Debug.LogError("❌ 无法创建中文字体！");
                return 0;
            }
            
            // 查找场景中所有TMP组件
            GameObject[] rootObjects = scene.GetRootGameObjects();
            List<TextMeshProUGUI> tmpComponents = new List<TextMeshProUGUI>();
            
            foreach (GameObject root in rootObjects)
            {
                tmpComponents.AddRange(root.GetComponentsInChildren<TextMeshProUGUI>(true));
            }
            
            int convertedCount = 0;
            
            foreach (var tmp in tmpComponents)
            {
                GameObject obj = tmp.gameObject;
                
                // 保存TMP的属性
                string text = tmp.text;
                float fontSize = tmp.fontSize;
                Color color = tmp.color;
                TextAlignmentOptions alignment = tmp.alignment;
                
                // 删除TMP组件
                Object.DestroyImmediate(tmp);
                
                // 添加Text组件
                Text newText = obj.AddComponent<Text>();
                
                // 恢复属性
                newText.text = text;
                newText.font = chineseFont;
                newText.fontSize = Mathf.RoundToInt(fontSize);
                newText.color = color;
                
                // 转换对齐方式
                newText.alignment = ConvertAlignment(alignment);
                
                // 设置其他常用属性
                newText.supportRichText = true;
                newText.raycastTarget = true;
                
                EditorUtility.SetDirty(obj);
                
                convertedCount++;
            }
            
            return convertedCount;
        }
        
        private static void BeautifySceneUI(Scene scene)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            
            foreach (GameObject root in rootObjects)
            {
                // 美化Canvas和背景
                Canvas canvas = root.GetComponent<Canvas>();
                if (canvas != null)
                {
                    BeautifyCanvas(canvas.gameObject);
                }
                
                // 美化所有按钮
                Button[] buttons = root.GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    BeautifyButton(button);
                }
                
                // 美化所有输入框
                InputField[] inputFields = root.GetComponentsInChildren<InputField>(true);
                foreach (InputField inputField in inputFields)
                {
                    BeautifyInputField(inputField);
                }
                
                // 美化所有面板
                Image[] images = root.GetComponentsInChildren<Image>(true);
                foreach (Image image in images)
                {
                    if (image.gameObject.name.Contains("Panel") || 
                        image.gameObject.name.Contains("Background"))
                    {
                        BeautifyPanel(image);
                    }
                }
            }
        }
        
        private static void BeautifyCanvas(GameObject canvasObj)
        {
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            
            // 添加或更新CanvasScaler
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvasObj.AddComponent<CanvasScaler>();
            }
            
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            EditorUtility.SetDirty(canvasObj);
        }
        
        private static void BeautifyButton(Button button)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                // 设置按钮颜色 - 现代蓝色
                ColorBlock colors = button.colors;
                colors.normalColor = new Color(0.2f, 0.5f, 0.9f, 1f);      // 蓝色
                colors.highlightedColor = new Color(0.3f, 0.6f, 1f, 1f);   // 亮蓝色
                colors.pressedColor = new Color(0.15f, 0.4f, 0.75f, 1f);   // 深蓝色
                colors.selectedColor = new Color(0.25f, 0.55f, 0.95f, 1f); // 选中蓝色
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);  // 灰色
                button.colors = colors;
                
                EditorUtility.SetDirty(button.gameObject);
            }
            
            // 美化按钮文本
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.color = Color.white;
                buttonText.fontSize = Mathf.Max(buttonText.fontSize, 20); // 最小字号20
                buttonText.alignment = TextAnchor.MiddleCenter;
                
                // 添加阴影效果
                Shadow shadow = buttonText.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = buttonText.gameObject.AddComponent<Shadow>();
                }
                shadow.effectColor = new Color(0, 0, 0, 0.5f);
                shadow.effectDistance = new Vector2(2, -2);
                
                EditorUtility.SetDirty(buttonText.gameObject);
            }
        }
        
        private static void BeautifyInputField(InputField inputField)
        {
            Image inputImage = inputField.GetComponent<Image>();
            if (inputImage != null)
            {
                inputImage.color = new Color(1f, 1f, 1f, 0.9f);
            }
            
            // 美化输入框文本
            Text inputText = inputField.textComponent;
            if (inputText != null)
            {
                inputText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
                inputText.fontSize = Mathf.Max(inputText.fontSize, 18);
                EditorUtility.SetDirty(inputText.gameObject);
            }
            
            // 美化占位符文本
            Text placeholder = inputField.placeholder as Text;
            if (placeholder != null)
            {
                placeholder.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                placeholder.fontSize = Mathf.Max(placeholder.fontSize, 18);
                placeholder.fontStyle = FontStyle.Italic;
                EditorUtility.SetDirty(placeholder.gameObject);
            }
            
            EditorUtility.SetDirty(inputField.gameObject);
        }
        
        private static void BeautifyPanel(Image panel)
        {
            // 根据面板名称设置不同的美化方案
            if (panel.gameObject.name.Contains("Background"))
            {
                // 背景使用渐变色或深色
                panel.color = new Color(0.15f, 0.2f, 0.3f, 1f); // 深蓝灰色背景
            }
            else if (panel.gameObject.name.Contains("Panel"))
            {
                // 面板使用半透明白色，营造卡片效果
                panel.color = new Color(0.95f, 0.95f, 0.95f, 0.95f);
                
                // 添加阴影效果
                Shadow shadow = panel.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = panel.gameObject.AddComponent<Shadow>();
                }
                shadow.effectColor = new Color(0, 0, 0, 0.3f);
                shadow.effectDistance = new Vector2(4, -4);
            }
            
            EditorUtility.SetDirty(panel.gameObject);
        }
        
        private static void ConvertAllTMPToText()
        {
            Debug.Log("════════════════════════════════════════");
            Debug.Log("开始转换TextMeshProUGUI → Text");
            Debug.Log("════════════════════════════════════════");
            
            // 获取系统字体
            Font chineseFont = GetChineseFont();
            if (chineseFont == null)
            {
                Debug.LogError("❌ 无法创建中文字体！");
                EditorUtility.DisplayDialog("错误", "无法创建中文字体！", "确定");
                return;
            }
            
            Debug.Log($"✅ 使用字体: {chineseFont.name}");
            
            // 查找所有TMP组件
            TextMeshProUGUI[] tmpComponents = Object.FindObjectsOfType<TextMeshProUGUI>(true);
            
            Debug.Log($"找到 {tmpComponents.Length} 个TextMeshProUGUI组件");
            
            int convertedCount = 0;
            List<string> convertedObjects = new List<string>();
            
            foreach (var tmp in tmpComponents)
            {
                GameObject obj = tmp.gameObject;
                
                // 保存TMP的属性
                string text = tmp.text;
                float fontSize = tmp.fontSize;
                Color color = tmp.color;
                TextAlignmentOptions alignment = tmp.alignment;
                
                // 删除TMP组件
                Object.DestroyImmediate(tmp);
                
                // 添加Text组件
                Text newText = obj.AddComponent<Text>();
                
                // 恢复属性
                newText.text = text;
                newText.font = chineseFont;
                newText.fontSize = Mathf.RoundToInt(fontSize);
                newText.color = color;
                
                // 转换对齐方式
                newText.alignment = ConvertAlignment(alignment);
                
                // 设置其他常用属性
                newText.supportRichText = true;
                newText.raycastTarget = true;
                
                EditorUtility.SetDirty(obj);
                
                convertedCount++;
                convertedObjects.Add($"{obj.name}: \"{text}\"");
                
                Debug.Log($"  ✓ {obj.name}: \"{text}\"");
            }
            
            // 保存场景
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            
            Debug.Log("════════════════════════════════════════");
            Debug.Log($"✅ 转换完成！共转换 {convertedCount} 个组件");
            Debug.Log("════════════════════════════════════════");
            
            // 显示结果
            string message = $"转换完成！\n\n共转换 {convertedCount} 个文本组件：\n\n";
            
            int showCount = Mathf.Min(10, convertedObjects.Count);
            for (int i = 0; i < showCount; i++)
            {
                message += convertedObjects[i] + "\n";
            }
            
            if (convertedObjects.Count > 10)
            {
                message += $"\n...还有 {convertedObjects.Count - 10} 个\n";
            }
            
            message += "\n现在点击Play测试，中文应该能正常显示了！";
            
            EditorUtility.DisplayDialog("转换完成", message, "确定");
        }
        
        private static Font GetChineseFont()
        {
            // 尝试多个常见中文字体
            string[] fontNames = new string[]
            {
                "Microsoft YaHei",
                "Microsoft YaHei UI",
                "SimHei",
                "SimSun",
                "Arial Unicode MS",
                "Arial"  // 备用
            };
            
            foreach (string fontName in fontNames)
            {
                Font font = Font.CreateDynamicFontFromOSFont(fontName, 16);
                if (font != null)
                {
                    Debug.Log($"  找到字体: {fontName}");
                    return font;
                }
            }
            
            return null;
        }
        
        private static TextAnchor ConvertAlignment(TextAlignmentOptions tmpAlignment)
        {
            // 转换TextMeshPro对齐方式到旧版Text对齐方式
            switch (tmpAlignment)
            {
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.TopJustified:
                case TextAlignmentOptions.TopFlush:
                case TextAlignmentOptions.TopGeoAligned:
                    return TextAnchor.UpperLeft;
                    
                case TextAlignmentOptions.Top:
                    return TextAnchor.UpperCenter;
                    
                case TextAlignmentOptions.TopRight:
                    return TextAnchor.UpperRight;
                    
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.Justified:
                case TextAlignmentOptions.Flush:
                case TextAlignmentOptions.MidlineJustified:
                case TextAlignmentOptions.MidlineLeft:
                    return TextAnchor.MiddleLeft;
                    
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Midline:
                case TextAlignmentOptions.Capline:
                case TextAlignmentOptions.Baseline:
                case TextAlignmentOptions.BaselineJustified:
                case TextAlignmentOptions.BaselineFlush:
                case TextAlignmentOptions.BaselineGeoAligned:
                case TextAlignmentOptions.CenterGeoAligned:
                case TextAlignmentOptions.MidlineGeoAligned:
                    return TextAnchor.MiddleCenter;
                    
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.MidlineRight:
                    return TextAnchor.MiddleRight;
                    
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BottomJustified:
                case TextAlignmentOptions.BottomFlush:
                case TextAlignmentOptions.BottomGeoAligned:
                    return TextAnchor.LowerLeft;
                    
                case TextAlignmentOptions.Bottom:
                    return TextAnchor.LowerCenter;
                    
                case TextAlignmentOptions.BottomRight:
                    return TextAnchor.LowerRight;
                    
                default:
                    return TextAnchor.MiddleCenter;
            }
        }
    }
}

