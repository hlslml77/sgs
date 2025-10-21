using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 输入框修复工具
    /// 修复 TMP_InputField 的文本组件引用问题
    /// </summary>
    public class InputFieldFixer : EditorWindow
    {
        [MenuItem("工具/修复/修复输入框组件")]
        public static void ShowWindow()
        {
            GetWindow<InputFieldFixer>("输入框组件修复");
        }

        private void OnGUI()
        {
            GUILayout.Label("输入框组件修复工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "这个工具会：\n" +
                "1. 查找所有 TMP_InputField 组件\n" +
                "2. 检查其文本组件引用\n" +
                "3. 如果使用了旧的 UI.Text，会替换为 TextMeshProUGUI\n" +
                "4. 修复缺失的引用",
                MessageType.Info);

            GUILayout.Space(10);

            if (GUILayout.Button("扫描并修复当前场景", GUILayout.Height(30)))
            {
                FixInputFieldsInScene();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("仅扫描当前场景", GUILayout.Height(30)))
            {
                ScanInputFieldsInScene();
            }
        }

        private void ScanInputFieldsInScene()
        {
            TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>(true);
            Debug.Log($"找到 {inputFields.Length} 个 TMP_InputField 组件");

            int issueCount = 0;
            foreach (var inputField in inputFields)
            {
                bool hasIssue = false;

                if (inputField.textComponent == null)
                {
                    Debug.LogWarning($"❌ {GetGameObjectPath(inputField.gameObject)} - 缺失文本组件引用", inputField);
                    hasIssue = true;
                }
                else if (!(inputField.textComponent is TMP_Text))
                {
                    Debug.LogWarning($"⚠️ {GetGameObjectPath(inputField.gameObject)} - 文本组件不是 TextMeshPro", inputField);
                    hasIssue = true;
                }

                if (inputField.placeholder != null && !(inputField.placeholder is TMP_Text))
                {
                    Debug.LogWarning($"⚠️ {GetGameObjectPath(inputField.gameObject)} - 占位符不是 TextMeshPro", inputField);
                    hasIssue = true;
                }

                if (hasIssue)
                    issueCount++;
            }

            if (issueCount > 0)
            {
                EditorUtility.DisplayDialog("扫描完成", $"发现 {issueCount} 个有问题的输入框\n请查看Console了解详情", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("扫描完成", "所有输入框都正常！", "确定");
            }
        }

        private void FixInputFieldsInScene()
        {
            if (!EditorUtility.DisplayDialog("确认修复",
                "这将修改当前场景中的输入框组件\n是否继续？",
                "确定", "取消"))
            {
                return;
            }

            TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>(true);
            Debug.Log($"开始修复 {inputFields.Length} 个 TMP_InputField 组件...");

            int fixedCount = 0;

            foreach (var inputField in inputFields)
            {
                bool needsFix = false;

                // 检查并修复文本组件
                if (inputField.textComponent == null || !(inputField.textComponent is TMP_Text))
                {
                    Debug.Log($"修复 {GetGameObjectPath(inputField.gameObject)} 的文本组件...");
                    FixTextComponent(inputField);
                    needsFix = true;
                }

                // 检查并修复占位符
                if (inputField.placeholder != null && !(inputField.placeholder is TMP_Text))
                {
                    Debug.Log($"修复 {GetGameObjectPath(inputField.gameObject)} 的占位符组件...");
                    FixPlaceholder(inputField);
                    needsFix = true;
                }

                if (needsFix)
                {
                    fixedCount++;
                    EditorUtility.SetDirty(inputField);
                }
            }

            if (fixedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                Debug.Log($"✅ 修复完成！共修复 {fixedCount} 个输入框");
                EditorUtility.DisplayDialog("修复完成", $"已修复 {fixedCount} 个输入框\n请保存场景", "确定");
            }
            else
            {
                Debug.Log("✅ 所有输入框都正常，无需修复");
                EditorUtility.DisplayDialog("修复完成", "所有输入框都正常！", "确定");
            }
        }

        private void FixTextComponent(TMP_InputField inputField)
        {
            // 查找 Text Area 下的 Text 对象
            Transform textArea = inputField.transform.Find("Text Area");
            if (textArea == null)
            {
                Debug.LogError($"未找到 Text Area: {GetGameObjectPath(inputField.gameObject)}", inputField);
                return;
            }

            Transform textTransform = textArea.Find("Text");
            if (textTransform == null)
            {
                // 如果没有 Text 对象，创建一个
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(textArea, false);
                textTransform = textObj.transform;

                RectTransform rectTransform = textObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                Debug.Log($"创建了新的 Text 对象: {GetGameObjectPath(textObj)}", textObj);
            }

            // 检查是否有旧的 UI.Text 组件
            Text oldText = textTransform.GetComponent<Text>();
            if (oldText != null)
            {
                // 保存旧的设置
                string text = oldText.text;
                int fontSize = oldText.fontSize;
                Color color = oldText.color;
                FontStyle fontStyle = oldText.fontStyle;

                // 移除旧组件
                DestroyImmediate(oldText);
                Debug.Log($"移除了旧的 UI.Text 组件: {GetGameObjectPath(textTransform.gameObject)}");
            }

            // 添加或获取 TextMeshProUGUI 组件
            TextMeshProUGUI tmpText = textTransform.GetComponent<TextMeshProUGUI>();
            if (tmpText == null)
            {
                tmpText = textTransform.gameObject.AddComponent<TextMeshProUGUI>();
                tmpText.fontSize = 18;
                tmpText.color = Color.white;
                Debug.Log($"添加了 TextMeshProUGUI 组件: {GetGameObjectPath(textTransform.gameObject)}");
            }

            // 设置引用
            inputField.textComponent = tmpText;
            Debug.Log($"✅ 已设置文本组件引用");
        }

        private void FixPlaceholder(TMP_InputField inputField)
        {
            // 查找 Text Area 下的 Placeholder 对象
            Transform textArea = inputField.transform.Find("Text Area");
            if (textArea == null)
            {
                Debug.LogError($"未找到 Text Area: {GetGameObjectPath(inputField.gameObject)}", inputField);
                return;
            }

            Transform placeholderTransform = textArea.Find("Placeholder");
            if (placeholderTransform == null)
            {
                // 如果没有 Placeholder 对象，创建一个
                GameObject placeholderObj = new GameObject("Placeholder");
                placeholderObj.transform.SetParent(textArea, false);
                placeholderTransform = placeholderObj.transform;

                RectTransform rectTransform = placeholderObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                Debug.Log($"创建了新的 Placeholder 对象: {GetGameObjectPath(placeholderObj)}", placeholderObj);
            }

            // 检查是否有旧的 UI.Text 组件
            Text oldText = placeholderTransform.GetComponent<Text>();
            if (oldText != null)
            {
                // 保存旧的设置
                string text = oldText.text;
                int fontSize = oldText.fontSize;
                Color color = oldText.color;
                FontStyle fontStyle = oldText.fontStyle;

                // 移除旧组件
                DestroyImmediate(oldText);
                Debug.Log($"移除了旧的 UI.Text 组件: {GetGameObjectPath(placeholderTransform.gameObject)}");
            }

            // 添加或获取 TextMeshProUGUI 组件
            TextMeshProUGUI tmpText = placeholderTransform.GetComponent<TextMeshProUGUI>();
            if (tmpText == null)
            {
                tmpText = placeholderTransform.gameObject.AddComponent<TextMeshProUGUI>();
                tmpText.text = "请输入...";
                tmpText.fontSize = 18;
                tmpText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                Debug.Log($"添加了 TextMeshProUGUI 组件: {GetGameObjectPath(placeholderTransform.gameObject)}");
            }

            // 设置引用
            inputField.placeholder = tmpText;
            Debug.Log($"✅ 已设置占位符引用");
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}

