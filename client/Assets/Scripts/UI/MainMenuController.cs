using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SanguoStrategy.Network;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 主菜单控制器 - 处理快速匹配、房间列表、玩家资料等功能
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button quickMatchButton;
        [SerializeField] private Button roomListButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        
        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private TextMeshProUGUI playerCoinsText;
        
        [Header("Network")]
        [SerializeField] private NetworkManager networkManager;
        
        private void Start()
        {
            InitializeButtons();
            LoadPlayerInfo();
            
            // 确保网络连接
            if (networkManager != null)
            {
                networkManager.Connect();
            }
        }
        
        private void InitializeButtons()
        {
            if (quickMatchButton != null)
                quickMatchButton.onClick.AddListener(OnQuickMatch);
                
            if (roomListButton != null)
                roomListButton.onClick.AddListener(OnRoomList);
                
            if (profileButton != null)
                profileButton.onClick.AddListener(OnProfile);
                
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettings);
                
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExit);
        }
        
        /// <summary>
        /// 快速匹配 - 自动匹配2-6人游戏
        /// </summary>
        private void OnQuickMatch()
        {
            Debug.Log("快速匹配...");
            // TODO: 调用网络管理器进行快速匹配
            // networkManager.QuickMatch();
            
            // 暂时直接进入选将场景测试
            SceneManager.LoadScene("HeroSelection");
        }
        
        /// <summary>
        /// 打开房间列表
        /// </summary>
        private void OnRoomList()
        {
            Debug.Log("打开房间列表");
            SceneManager.LoadScene("RoomList");
        }
        
        /// <summary>
        /// 打开玩家资料
        /// </summary>
        private void OnProfile()
        {
            Debug.Log("打开玩家资料");
            // TODO: 显示玩家资料面板
        }
        
        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnSettings()
        {
            Debug.Log("打开设置");
            // TODO: 显示设置面板
        }
        
        /// <summary>
        /// 退出游戏
        /// </summary>
        private void OnExit()
        {
            Debug.Log("退出游戏");
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        
        /// <summary>
        /// 加载玩家信息
        /// </summary>
        private void LoadPlayerInfo()
        {
            // TODO: 从服务器加载玩家信息
            if (playerNameText != null)
                playerNameText.text = "玩家名称";
                
            if (playerLevelText != null)
                playerLevelText.text = "等级: 1";
                
            if (playerCoinsText != null)
                playerCoinsText.text = "欢乐豆: 1000";
        }
        
        private void OnDestroy()
        {
            // 清理事件监听
            if (quickMatchButton != null)
                quickMatchButton.onClick.RemoveAllListeners();
                
            if (roomListButton != null)
                roomListButton.onClick.RemoveAllListeners();
                
            if (profileButton != null)
                profileButton.onClick.RemoveAllListeners();
                
            if (settingsButton != null)
                settingsButton.onClick.RemoveAllListeners();
                
            if (exitButton != null)
                exitButton.onClick.RemoveAllListeners();
        }
    }
}

