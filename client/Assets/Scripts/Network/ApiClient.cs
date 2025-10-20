using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SanguoStrategy.Network
{
    /// <summary>
    /// HTTP API 客户端
    /// </summary>
    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }

        [SerializeField] private string apiBaseUrl = "http://localhost:8080/api/v1";

        private string authToken;

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

        /// <summary>
        /// 设置认证 Token
        /// </summary>
        public void SetAuthToken(string token)
        {
            authToken = token;
            PlayerPrefs.SetString("AuthToken", token);
        }

        /// <summary>
        /// 获取认证 Token
        /// </summary>
        public string GetAuthToken()
        {
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = PlayerPrefs.GetString("AuthToken", "");
            }
            return authToken;
        }

        /// <summary>
        /// 注册
        /// </summary>
        public IEnumerator Register(string username, string email, string password, Action<bool, string> callback)
        {
            var requestData = new
            {
                username = username,
                email = email,
                password = password
            };

            yield return PostRequest("/auth/register", requestData, (success, response) =>
            {
                if (success)
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(response);
                    SetAuthToken(data.token.ToString());
                    callback?.Invoke(true, "注册成功");
                }
                else
                {
                    callback?.Invoke(false, response);
                }
            });
        }

        /// <summary>
        /// 登录
        /// </summary>
        public IEnumerator Login(string username, string password, Action<bool, string> callback)
        {
            var requestData = new
            {
                username = username,
                password = password
            };

            yield return PostRequest("/auth/login", requestData, (success, response) =>
            {
                if (success)
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(response);
                    SetAuthToken(data.token.ToString());
                    callback?.Invoke(true, "登录成功");
                }
                else
                {
                    callback?.Invoke(false, response);
                }
            });
        }

        /// <summary>
        /// 获取房间列表
        /// </summary>
        public IEnumerator GetRoomList(Action<bool, string> callback)
        {
            yield return GetRequest("/room/list", callback);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public IEnumerator CreateRoom(string roomName, int maxPlayers, string gameMode, Action<bool, string> callback)
        {
            var requestData = new
            {
                name = roomName,
                max_players = maxPlayers,
                game_mode = gameMode,
                enable_synergy = true,
                enable_random_pick = true
            };

            yield return PostRequest("/room/create", requestData, callback);
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        public IEnumerator JoinRoom(string roomId, Action<bool, string> callback)
        {
            yield return PostRequest($"/room/{roomId}/join", null, callback);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        public IEnumerator LeaveRoom(string roomId, Action<bool, string> callback)
        {
            yield return PostRequest($"/room/{roomId}/leave", null, callback);
        }

        /// <summary>
        /// 加入匹配
        /// </summary>
        public IEnumerator JoinMatchmaking(Action<bool, string> callback)
        {
            yield return PostRequest("/match/join", null, callback);
        }

        /// <summary>
        /// 获取武将数据
        /// </summary>
        public IEnumerator GetGenerals(Action<bool, string> callback)
        {
            yield return GetRequest("/data/generals", callback);
        }

        /// <summary>
        /// 获取地形数据
        /// </summary>
        public IEnumerator GetTerrains(Action<bool, string> callback)
        {
            yield return GetRequest("/data/terrains", callback);
        }

        // HTTP 请求方法
        private IEnumerator GetRequest(string endpoint, Action<bool, string> callback)
        {
            string url = apiBaseUrl + endpoint;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // 添加认证头
                if (!string.IsNullOrEmpty(authToken))
                {
                    request.SetRequestHeader("Authorization", "Bearer " + authToken);
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(true, request.downloadHandler.text);
                }
                else
                {
                    callback?.Invoke(false, request.error);
                }
            }
        }

        private IEnumerator PostRequest(string endpoint, object data, Action<bool, string> callback)
        {
            string url = apiBaseUrl + endpoint;
            string jsonData = data != null ? JsonConvert.SerializeObject(data) : "{}";
            
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // 添加认证头
                if (!string.IsNullOrEmpty(authToken))
                {
                    request.SetRequestHeader("Authorization", "Bearer " + authToken);
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(true, request.downloadHandler.text);
                }
                else
                {
                    callback?.Invoke(false, request.error);
                }
            }
        }
    }
}

