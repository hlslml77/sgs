using System;
using System.Collections;
using UnityEngine;
// 在未集成 WebSocketSharp 时，避免编译错误
#if WEBSOCKET_SHARP
using WebSocketSharp;
#endif
using Newtonsoft.Json;

namespace SanguoStrategy.Network
{
    /// <summary>
    /// 网络管理器 - 处理与服务器的 WebSocket 连接
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        // 服务器配置
        private string serverUrl = "ws://localhost:8080/ws";
        
        // 在缺失 WebSocketSharp 的情况下，改为对象引用并在运行时报错提示
#if WEBSOCKET_SHARP
        private WebSocket webSocket;
#else
        private object webSocket;
#endif
        private bool isConnected = false;

        // 事件
#pragma warning disable 0067 // 事件声明但未使用（这些事件在条件编译块中使用）
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnMessageReceived;
#pragma warning restore 0067
        public event Action<string> OnError;
        
        /// <summary>
        /// 设置服务器URL
        /// </summary>
        public void SetServerUrl(string url)
        {
            serverUrl = url;
        }

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
        /// 连接到服务器
        /// </summary>
        public void Connect()
        {
            if (isConnected)
            {
                Debug.LogWarning("Already connected to server");
                return;
            }

            try
            {
#if WEBSOCKET_SHARP
                webSocket = new WebSocket(serverUrl);

                webSocket.OnOpen += (sender, e) =>
                {
                    isConnected = true;
                    Debug.Log("Connected to server");
                    OnConnected?.Invoke();
                };

                webSocket.OnMessage += (sender, e) =>
                {
                    Debug.Log($"Received message: {e.Data}");
                    OnMessageReceived?.Invoke(e.Data);
                };

                webSocket.OnError += (sender, e) =>
                {
                    Debug.LogError($"WebSocket error: {e.Message}");
                    OnError?.Invoke(e.Message);
                };

                webSocket.OnClose += (sender, e) =>
                {
                    isConnected = false;
                    Debug.Log("Disconnected from server");
                    OnDisconnected?.Invoke();
                };

                webSocket.Connect();
#else
                Debug.LogError("WebSocketSharp 未集成：请导入 websocket-sharp.dll 或定义编译符号 WEBSOCKET_SHARP。");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect: {ex.Message}");
                OnError?.Invoke(ex.Message);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (webSocket != null && isConnected)
            {
                
#if WEBSOCKET_SHARP
                webSocket.Close();
#endif
                webSocket = null;
                isConnected = false;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(string type, object data, string roomId = null)
        {
            if (!isConnected)
            {
                Debug.LogError("Not connected to server");
                return;
            }

            var message = new
            {
                type = type,
                room_id = roomId,
                data = data
            };

            string json = JsonConvert.SerializeObject(message);
#if WEBSOCKET_SHARP
            (webSocket as WebSocket)?.Send(json);
#else
            Debug.LogWarning($"发送失败（未集成 WebSocketSharp）：{json}");
#endif
            Debug.Log($"Sent message: {json}");
        }

        /// <summary>
        /// 发送游戏操作
        /// </summary>
        public void SendGameAction(string roomId, string actionType, object actionData)
        {
            SendMessage("game_action", new
            {
                action_type = actionType,
                data = actionData
            }, roomId);
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}

