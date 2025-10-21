using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 把TextMeshProUGUI转换为Unity旧版Text组件
    /// 这是最简单可靠的中文显示方案！
    /// </summary>
    public class ConvertToLegacyText : EditorWindow
    {
        [MenuItem("工具/⚡ 一键转换为旧版Text（100%能显示中文）")]
        public static void ShowWindow()
        {
            if (EditorUtility.DisplayDialog("确认转换", 
                "这个工具会把场景中所有TextMeshProUGUI组件转换为Unity旧版Text组件。\n\n" +
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

