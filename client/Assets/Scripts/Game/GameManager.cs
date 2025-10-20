using System.Collections.Generic;
using UnityEngine;
using SanguoStrategy.Network;
using Newtonsoft.Json;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public string CurrentRoomId;
        public GameState CurrentGameState;
        public bool IsInGame = false;

        [Header("Managers")]
        public BoardManager BoardManager;
        public CardManager CardManager;
        public UIManager UIManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 订阅网络事件
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnMessageReceived += HandleServerMessage;
            }
        }

        /// <summary>
        /// 处理服务器消息
        /// </summary>
        private void HandleServerMessage(string message)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<ServerMessage>(message);

                switch (msg.Type)
                {
                    case "connected":
                        Debug.Log("Successfully connected to server");
                        break;

                    case "game_start":
                        HandleGameStart(msg.Data);
                        break;

                    case "game_state_update":
                        HandleGameStateUpdate(msg.Data);
                        break;

                    case "action_response":
                        HandleActionResponse(msg.Data);
                        break;

                    case "player_joined":
                        HandlePlayerJoined(msg.Data);
                        break;

                    case "player_left":
                        HandlePlayerLeft(msg.Data);
                        break;

                    case "error":
                        HandleError(msg.Data);
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to parse server message: {e.Message}");
            }
        }

        private void HandleGameStart(string data)
        {
            Debug.Log("Game started!");
            IsInGame = true;
            
            CurrentGameState = JsonConvert.DeserializeObject<GameState>(data);
            
            // 初始化游戏界面
            BoardManager?.InitializeBoard(CurrentGameState);
            UIManager?.ShowGameUI();
        }

        private void HandleGameStateUpdate(string data)
        {
            CurrentGameState = JsonConvert.DeserializeObject<GameState>(data);
            
            // 更新游戏界面
            BoardManager?.UpdateBoard(CurrentGameState);
            UIManager?.UpdateGameUI(CurrentGameState);
        }

        private void HandleActionResponse(string data)
        {
            var response = JsonConvert.DeserializeObject<ActionResponse>(data);
            
            if (response.Success)
            {
                Debug.Log($"Action successful: {response.Message}");
            }
            else
            {
                Debug.LogWarning($"Action failed: {response.Message}");
                UIManager?.ShowMessage(response.Message);
            }
        }

        private void HandlePlayerJoined(string data)
        {
            Debug.Log($"Player joined: {data}");
            UIManager?.UpdatePlayerList();
        }

        private void HandlePlayerLeft(string data)
        {
            Debug.Log($"Player left: {data}");
            UIManager?.UpdatePlayerList();
        }

        private void HandleError(string data)
        {
            Debug.LogError($"Server error: {data}");
            UIManager?.ShowError(data);
        }

        /// <summary>
        /// 发送游戏操作
        /// </summary>
        public void SendAction(string actionType, object actionData)
        {
            if (!IsInGame || string.IsNullOrEmpty(CurrentRoomId))
            {
                Debug.LogError("Not in a game");
                return;
            }

            NetworkManager.Instance.SendGameAction(CurrentRoomId, actionType, actionData);
        }

        /// <summary>
        /// 出牌
        /// </summary>
        public void PlayCard(string cardId, List<string> targetIds)
        {
            SendAction("play_card", new
            {
                card_id = cardId,
                target_ids = targetIds
            });
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        public void UseSkill(string generalId, string skillId, List<string> targetIds)
        {
            SendAction("use_skill", new
            {
                general_id = generalId,
                skill_id = skillId,
                target_ids = targetIds
            });
        }

        /// <summary>
        /// 移动武将
        /// </summary>
        public void MoveGeneral(string generalId, Vector2Int position)
        {
            SendAction("move", new
            {
                general_id = generalId,
                target_pos = new { x = position.x, y = position.y }
            });
        }

        /// <summary>
        /// 部署地形
        /// </summary>
        public void DeployTerrain(string terrainId, Vector2Int position)
        {
            SendAction("deploy_terrain", new
            {
                terrain_id = terrainId,
                pos = new { x = position.x, y = position.y }
            });
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnMessageReceived -= HandleServerMessage;
            }
        }
    }

    // 数据模型
    [System.Serializable]
    public class ServerMessage
    {
        public string Type;
        public string RoomId;
        public string Data;
    }

    [System.Serializable]
    public class GameState
    {
        public string RoomId;
        public int Round;
        public string CurrentPhase;
        public List<PlayerState> Players;
        public Dictionary<string, TerrainState> Terrains;
        public bool IsFinished;
        public string Winner;
    }

    [System.Serializable]
    public class PlayerState
    {
        public string PlayerId;
        public string Username;
        public string Team;
        public List<GeneralState> Generals;
        public List<Card> HandCards;
        public bool IsAlive;
        public int ActionPoints;
    }

    [System.Serializable]
    public class GeneralState
    {
        public string GeneralId;
        public string Name;
        public string Faction;
        public string Role;
        public int CurrentHP;
        public int MaxHP;
        public Vector2Int Position;
        public List<Skill> Skills;
        public bool IsAlive;
    }

    [System.Serializable]
    public class TerrainState
    {
        public string TerrainId;
        public string Name;
        public string Type;
        public Vector2Int Position;
        public int Duration;
        public bool IsActivated;
    }

    [System.Serializable]
    public class Card
    {
        public string Id;
        public string Type;
        public string Name;
        public string Suit;
        public int Value;
    }

    [System.Serializable]
    public class Skill
    {
        public string Id;
        public string Name;
        public string Type;
        public string Description;
        public int Cooldown;
        public int CurrentCD;
    }

    [System.Serializable]
    public class ActionResponse
    {
        public bool Success;
        public string Message;
    }
}

