using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using SanguoStrategy.Network;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 房间列表控制器 - 显示可用房间、创建自定义房间、地形规则选择
    /// </summary>
    public class RoomListController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Transform roomListContent;
        [SerializeField] private GameObject roomItemPrefab;
        
        [Header("Create Room Panel")]
        [SerializeField] private GameObject createRoomPanel;
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private TMP_Dropdown maxPlayersDropdown;
        [SerializeField] private TMP_Dropdown terrainRuleDropdown;
        [SerializeField] private Toggle randomTerrainToggle;
        [SerializeField] private Button confirmCreateButton;
        [SerializeField] private Button cancelCreateButton;
        
        [Header("Network")]
        [SerializeField] private NetworkManager networkManager;
        
        private List<RoomInfo> rooms = new List<RoomInfo>();
        
        private void Start()
        {
            InitializeButtons();
            InitializeDropdowns();
            RefreshRoomList();
            
            if (createRoomPanel != null)
                createRoomPanel.SetActive(false);
        }
        
        private void InitializeButtons()
        {
            if (createRoomButton != null)
                createRoomButton.onClick.AddListener(OnCreateRoom);
                
            if (refreshButton != null)
                refreshButton.onClick.AddListener(OnRefresh);
                
            if (backButton != null)
                backButton.onClick.AddListener(OnBack);
                
            if (confirmCreateButton != null)
                confirmCreateButton.onClick.AddListener(OnConfirmCreateRoom);
                
            if (cancelCreateButton != null)
                cancelCreateButton.onClick.AddListener(OnCancelCreateRoom);
        }
        
        private void InitializeDropdowns()
        {
            // 初始化最大玩家数下拉框
            if (maxPlayersDropdown != null)
            {
                maxPlayersDropdown.ClearOptions();
                List<string> options = new List<string> { "2人", "3人", "4人", "5人", "6人" };
                maxPlayersDropdown.AddOptions(options);
                maxPlayersDropdown.value = 3; // 默认4人
            }
            
            // 初始化地形规则下拉框
            if (terrainRuleDropdown != null)
            {
                terrainRuleDropdown.ClearOptions();
                List<string> options = new List<string> 
                { 
                    "赤壁水战", 
                    "官渡之战", 
                    "夷陵之战",
                    "五丈原",
                    "全随机地形"
                };
                terrainRuleDropdown.AddOptions(options);
            }
        }
        
        /// <summary>
        /// 刷新房间列表
        /// </summary>
        private void RefreshRoomList()
        {
            Debug.Log("刷新房间列表");
            // TODO: 从服务器获取房间列表
            // networkManager.GetRoomList((roomList) => {
            //     UpdateRoomList(roomList);
            // });
            
            // 测试数据
            rooms.Clear();
            rooms.Add(new RoomInfo { roomName = "赤壁火攻", currentPlayers = 2, maxPlayers = 4, terrainRule = "赤壁水战" });
            rooms.Add(new RoomInfo { roomName = "官渡大战", currentPlayers = 3, maxPlayers = 6, terrainRule = "官渡之战" });
            
            UpdateRoomList();
        }
        
        /// <summary>
        /// 更新房间列表UI
        /// </summary>
        private void UpdateRoomList()
        {
            // 清空现有列表
            if (roomListContent != null)
            {
                foreach (Transform child in roomListContent)
                {
                    Destroy(child.gameObject);
                }
                
                // 创建房间项
                foreach (var room in rooms)
                {
                    GameObject item = Instantiate(roomItemPrefab, roomListContent);
                    // TODO: 设置房间项信息
                    TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();
                    if (texts.Length > 0)
                        texts[0].text = $"{room.roomName} ({room.currentPlayers}/{room.maxPlayers}) - {room.terrainRule}";
                    
                    Button joinButton = item.GetComponentInChildren<Button>();
                    if (joinButton != null)
                    {
                        RoomInfo roomCopy = room;
                        joinButton.onClick.AddListener(() => OnJoinRoom(roomCopy));
                    }
                }
            }
        }
        
        /// <summary>
        /// 创建房间
        /// </summary>
        private void OnCreateRoom()
        {
            Debug.Log("显示创建房间面板");
            if (createRoomPanel != null)
                createRoomPanel.SetActive(true);
        }
        
        /// <summary>
        /// 确认创建房间
        /// </summary>
        private void OnConfirmCreateRoom()
        {
            string roomName = roomNameInput != null ? roomNameInput.text : "新房间";
            int maxPlayers = maxPlayersDropdown != null ? maxPlayersDropdown.value + 2 : 4;
            string terrainRule = terrainRuleDropdown != null ? terrainRuleDropdown.options[terrainRuleDropdown.value].text : "随机";
            bool randomTerrain = randomTerrainToggle != null && randomTerrainToggle.isOn;
            
            Debug.Log($"创建房间: {roomName}, 最大玩家数: {maxPlayers}, 地形规则: {terrainRule}, 随机地形: {randomTerrain}");
            
            // TODO: 调用网络管理器创建房间
            // networkManager.CreateRoom(roomName, maxPlayers, terrainRule, randomTerrain);
            
            if (createRoomPanel != null)
                createRoomPanel.SetActive(false);
                
            // 测试：直接进入选将场景
            SceneManager.LoadScene("HeroSelection");
        }
        
        /// <summary>
        /// 取消创建房间
        /// </summary>
        private void OnCancelCreateRoom()
        {
            if (createRoomPanel != null)
                createRoomPanel.SetActive(false);
        }
        
        /// <summary>
        /// 加入房间
        /// </summary>
        private void OnJoinRoom(RoomInfo room)
        {
            Debug.Log($"加入房间: {room.roomName}");
            // TODO: 调用网络管理器加入房间
            // networkManager.JoinRoom(room.roomId);
            
            // 测试：直接进入选将场景
            SceneManager.LoadScene("HeroSelection");
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        private void OnRefresh()
        {
            RefreshRoomList();
        }
        
        /// <summary>
        /// 返回主菜单
        /// </summary>
        private void OnBack()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// 房间信息结构
        /// </summary>
        [System.Serializable]
        public class RoomInfo
        {
            public string roomId;
            public string roomName;
            public int currentPlayers;
            public int maxPlayers;
            public string terrainRule;
        }
    }
}

