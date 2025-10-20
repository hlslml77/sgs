using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject gameUIPanel;
        [SerializeField] private GameObject roomListPanel;

        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI phaseText;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Transform handCardsContainer;
        [SerializeField] private Transform skillsContainer;

        [Header("Message")]
        [SerializeField] private GameObject messagePanel;
        [SerializeField] private TextMeshProUGUI messageText;

        private void Start()
        {
            ShowMainMenu();
        }

        /// <summary>
        /// 显示主菜单
        /// </summary>
        public void ShowMainMenu()
        {
            HideAllPanels();
            mainMenuPanel?.SetActive(true);
        }

        /// <summary>
        /// 显示游戏UI
        /// </summary>
        public void ShowGameUI()
        {
            HideAllPanels();
            gameUIPanel?.SetActive(true);
        }

        /// <summary>
        /// 显示房间列表
        /// </summary>
        public void ShowRoomList()
        {
            HideAllPanels();
            roomListPanel?.SetActive(true);
        }

        private void HideAllPanels()
        {
            mainMenuPanel?.SetActive(false);
            gameUIPanel?.SetActive(false);
            roomListPanel?.SetActive(false);
        }

        /// <summary>
        /// 更新游戏UI
        /// </summary>
        public void UpdateGameUI(GameState gameState)
        {
            if (roundText != null)
            {
                roundText.text = $"回合: {gameState.Round}";
            }

            if (phaseText != null)
            {
                string phaseDisplay = gameState.CurrentPhase switch
                {
                    "prepare" => "准备阶段",
                    "deploy" => "部署阶段",
                    "combat" => "战斗阶段",
                    "end_round" => "回合结束",
                    _ => gameState.CurrentPhase
                };
                phaseText.text = $"阶段: {phaseDisplay}";
            }

            // TODO: 更新手牌显示
            // TODO: 更新技能显示
        }

        /// <summary>
        /// 更新玩家列表
        /// </summary>
        public void UpdatePlayerList()
        {
            // TODO: 实现玩家列表更新
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        public void ShowMessage(string message)
        {
            if (messagePanel != null && messageText != null)
            {
                messageText.text = message;
                messagePanel.SetActive(true);
                Invoke(nameof(HideMessage), 2f);
            }
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        public void ShowError(string error)
        {
            ShowMessage($"错误: {error}");
        }

        private void HideMessage()
        {
            messagePanel?.SetActive(false);
        }

        // 按钮事件
        public void OnQuickMatchClicked()
        {
            StartCoroutine(SanguoStrategy.Network.ApiClient.Instance.JoinMatchmaking((success, response) =>
            {
                if (success)
                {
                    ShowMessage("加入匹配成功");
                }
                else
                {
                    ShowError(response);
                }
            }));
        }

        public void OnCustomRoomClicked()
        {
            ShowRoomList();
        }

        public void OnExitClicked()
        {
            Application.Quit();
        }
    }
}

