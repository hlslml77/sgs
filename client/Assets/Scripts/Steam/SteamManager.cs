using UnityEngine;
#if UNITY_STANDALONE
using Steamworks;
#endif

namespace SanguoStrategy.Steam
{
    /// <summary>
    /// Steam 管理器
    /// </summary>
    public class SteamManager : MonoBehaviour
    {
        public static SteamManager Instance { get; private set; }

        public bool IsSteamInitialized { get; private set; }
        public string SteamUsername { get; private set; }
        public ulong SteamId { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSteam();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSteam()
        {
#if UNITY_STANDALONE
            try
            {
                if (SteamAPI.RestartAppIfNecessary((AppId_t)480)) // 480是测试AppID，需要替换
                {
                    Application.Quit();
                    return;
                }

                IsSteamInitialized = SteamAPI.Init();
                
                if (IsSteamInitialized)
                {
                    SteamUsername = SteamFriends.GetPersonaName();
                    SteamId = SteamUser.GetSteamID().m_SteamID;
                    Debug.Log($"Steam initialized. User: {SteamUsername} (ID: {SteamId})");
                }
                else
                {
                    Debug.LogError("Steam initialization failed");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Steam initialization error: {e.Message}");
                IsSteamInitialized = false;
            }
#else
            Debug.LogWarning("Steam is only available on standalone platforms");
            IsSteamInitialized = false;
#endif
        }

        private void Update()
        {
#if UNITY_STANDALONE
            if (IsSteamInitialized)
            {
                SteamAPI.RunCallbacks();
            }
#endif
        }

        /// <summary>
        /// 获取 Steam 认证票据
        /// </summary>
        public byte[] GetAuthSessionTicket()
        {
#if UNITY_STANDALONE
            if (!IsSteamInitialized) return null;

            byte[] ticket = new byte[1024];
            uint ticketSize;
            HAuthTicket authTicket = SteamUser.GetAuthSessionTicket(ticket, ticket.Length, out ticketSize);
            
            if (authTicket != HAuthTicket.Invalid)
            {
                byte[] result = new byte[ticketSize];
                System.Array.Copy(ticket, result, ticketSize);
                return result;
            }
#endif
            return null;
        }

        /// <summary>
        /// 解锁成就
        /// </summary>
        public void UnlockAchievement(string achievementId)
        {
#if UNITY_STANDALONE
            if (!IsSteamInitialized) return;

            bool success = SteamUserStats.SetAchievement(achievementId);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log($"Achievement unlocked: {achievementId}");
            }
#endif
        }

        /// <summary>
        /// 检查是否已解锁成就
        /// </summary>
        public bool IsAchievementUnlocked(string achievementId)
        {
#if UNITY_STANDALONE
            if (!IsSteamInitialized) return false;

            bool achieved;
            SteamUserStats.GetAchievement(achievementId, out achieved);
            return achieved;
#else
            return false;
#endif
        }

        /// <summary>
        /// 设置统计数据
        /// </summary>
        public void SetStat(string statName, int value)
        {
#if UNITY_STANDALONE
            if (!IsSteamInitialized) return;

            SteamUserStats.SetStat(statName, value);
            SteamUserStats.StoreStats();
#endif
        }

        /// <summary>
        /// 显示 Steam 好友列表
        /// </summary>
        public void ShowFriendsList()
        {
#if UNITY_STANDALONE
            if (!IsSteamInitialized) return;

            SteamFriends.ActivateGameOverlay("Friends");
#endif
        }

        /// <summary>
        /// 邀请好友加入游戏
        /// </summary>
        public void InviteFriend(ulong friendSteamId)
        {
#if UNITY_STANDALONE
            if (!IsSteamInitialized) return;

            // TODO: 实现好友邀请逻辑
            Debug.Log($"Inviting friend: {friendSteamId}");
#endif
        }

        private void OnDestroy()
        {
#if UNITY_STANDALONE
            if (IsSteamInitialized)
            {
                SteamAPI.Shutdown();
            }
#endif
        }

        private void OnApplicationQuit()
        {
#if UNITY_STANDALONE
            if (IsSteamInitialized)
            {
                SteamAPI.Shutdown();
            }
#endif
        }
    }
}

