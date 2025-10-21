using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// UI修复器 - 自动检测并修复常见UI问题
    /// </summary>
    [ExecuteInEditMode]
    public class UIFixer : MonoBehaviour
    {
        [Header("自动修复选项")]
        [SerializeField] private bool autoFixOnStart = true;
        [SerializeField] private bool checkEventSystem = true;
        [SerializeField] private bool checkCanvasSettings = true;
        [SerializeField] private bool fixButtonReferences = true;
        
        [Header("诊断信息")]
        [SerializeField] private bool showDebugInfo = true;
        
        private void Start()
        {
            if (autoFixOnStart)
            {
                FixAllIssues();
            }
        }
        
        [ContextMenu("修复所有UI问题")]
        public void FixAllIssues()
        {
            if (checkEventSystem)
                CheckAndFixEventSystem();
                
            if (checkCanvasSettings)
                CheckAndFixCanvas();
                
            if (fixButtonReferences)
                CheckButtonReferences();
                
            LogInfo("UI修复完成！");
        }
        
        /// <summary>
        /// 检查并修复EventSystem
        /// </summary>
        private void CheckAndFixEventSystem()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            
            if (eventSystem == null)
            {
                LogWarning("未找到EventSystem，正在创建...");
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                LogInfo("EventSystem创建成功！");
            }
            else
            {
                LogInfo("EventSystem检查通过");
                
                // 确保InputModule存在
                if (eventSystem.GetComponent<StandaloneInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                    LogInfo("添加了StandaloneInputModule");
                }
            }
        }
        
        /// <summary>
        /// 检查并修复Canvas设置
        /// </summary>
        private void CheckAndFixCanvas()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            
            foreach (Canvas canvas in canvases)
            {
                // 检查CanvasScaler
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler == null)
                {
                    scaler = canvas.gameObject.AddComponent<CanvasScaler>();
                    LogInfo($"为 {canvas.name} 添加了CanvasScaler");
                }
                
                // 设置为屏幕空间覆盖模式
                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    LogInfo($"修正了 {canvas.name} 的渲染模式");
                }
                
                // 配置CanvasScaler为Scale With Screen Size
                if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.matchWidthOrHeight = 0.5f;
                    LogInfo($"配置了 {canvas.name} 的缩放模式");
                }
                
                // 检查GraphicRaycaster
                GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                    LogInfo($"为 {canvas.name} 添加了GraphicRaycaster");
                }
            }
            
            if (canvases.Length == 0)
            {
                LogWarning("场景中未找到Canvas！");
            }
        }
        
        /// <summary>
        /// 检查按钮引用
        /// </summary>
        private void CheckButtonReferences()
        {
            // 检查MainMenuController
            MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
            if (mainMenu != null)
            {
                LogInfo("找到MainMenuController，检查按钮引用...");
                CheckControllerButtons(mainMenu.gameObject);
            }
            
            // 检查RoomListController
            RoomListController roomList = FindObjectOfType<RoomListController>();
            if (roomList != null)
            {
                LogInfo("找到RoomListController，检查按钮引用...");
                CheckControllerButtons(roomList.gameObject);
            }
            
            // 检查HeroSelectionController
            HeroSelectionController heroSelection = FindObjectOfType<HeroSelectionController>();
            if (heroSelection != null)
            {
                LogInfo("找到HeroSelectionController，检查按钮引用...");
                CheckControllerButtons(heroSelection.gameObject);
            }
            
            // 检查所有按钮
            Button[] allButtons = FindObjectsOfType<Button>();
            LogInfo($"场景中共有 {allButtons.Length} 个按钮");
            
            int issueCount = 0;
            foreach (Button button in allButtons)
            {
                if (!button.interactable && button.gameObject.activeInHierarchy)
                {
                    LogWarning($"按钮 {button.name} 未启用交互");
                    issueCount++;
                }
                
                if (button.onClick.GetPersistentEventCount() == 0)
                {
                    LogWarning($"按钮 {button.name} 未绑定点击事件");
                    issueCount++;
                }
            }
            
            if (issueCount == 0)
            {
                LogInfo("所有按钮检查通过！");
            }
        }
        
        private void CheckControllerButtons(GameObject controllerObj)
        {
            Button[] buttons = controllerObj.GetComponentsInChildren<Button>(true);
            LogInfo($"  - 找到 {buttons.Length} 个按钮");
        }
        
        private void LogInfo(string message)
        {
            if (showDebugInfo)
                Debug.Log($"[UI修复器] {message}");
        }
        
        private void LogWarning(string message)
        {
            if (showDebugInfo)
                Debug.LogWarning($"[UI修复器] {message}");
        }
    }
}

