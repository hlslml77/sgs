using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 运行时输入框修复器
    /// 自动修复缺少 GraphicRaycaster 和 StandaloneInputModule 的问题
    /// </summary>
    public class RuntimeInputFixer : MonoBehaviour
    {
        [SerializeField] private bool autoFixOnAwake = true;
        [SerializeField] private bool showDebugLogs = true;

        private void Awake()
        {
            if (autoFixOnAwake)
            {
                FixInputSystem();
            }
        }

        [ContextMenu("修复输入系统")]
        public void FixInputSystem()
        {
            int fixCount = 0;

            // 1. 检查并修复 EventSystem
            fixCount += FixEventSystem();

            // 2. 检查并修复 GraphicRaycaster
            fixCount += FixGraphicRaycaster();

            if (showDebugLogs)
            {
                if (fixCount > 0)
                {
                    Debug.Log($"<color=green>✅ 输入系统修复完成！修复了 {fixCount} 个问题</color>");
                }
                else
                {
                    Debug.Log("<color=green>✓ 输入系统正常</color>");
                }
            }
        }

        /// <summary>
        /// 修复 EventSystem
        /// </summary>
        private int FixEventSystem()
        {
            int fixCount = 0;
            EventSystem eventSystem = FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();

                if (showDebugLogs)
                    Debug.Log("<color=yellow>✅ 已创建 EventSystem 和 StandaloneInputModule</color>");

                fixCount += 2;
            }
            else
            {
                // 检查 StandaloneInputModule
                if (eventSystem.GetComponent<StandaloneInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<StandaloneInputModule>();

                    if (showDebugLogs)
                        Debug.Log("<color=yellow>✅ 已添加 StandaloneInputModule</color>");

                    fixCount++;
                }
            }

            return fixCount;
        }

        /// <summary>
        /// 修复 GraphicRaycaster
        /// </summary>
        private int FixGraphicRaycaster()
        {
            int fixCount = 0;

            // 查找所有 Canvas
            Canvas[] canvases = FindObjectsOfType<Canvas>(true);

            foreach (Canvas canvas in canvases)
            {
                // 检查是否缺少 GraphicRaycaster
                if (canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();

                    if (showDebugLogs)
                        Debug.Log($"<color=yellow>✅ 为 Canvas '{canvas.name}' 添加了 GraphicRaycaster</color>");

                    fixCount++;
                }
            }

            return fixCount;
        }
    }
}

