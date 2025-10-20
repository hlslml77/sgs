using System;
using System.Collections;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

namespace SanguoStrategy.Network
{
    /// <summary>
    /// 网络管理器 - 处理与服务器的 WebSocket 连接
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [SerializeField] private string serverUrl = "ws://localhost:8080/ws";
        
        private WebSocket webSocket;
        private bool isConnected = false;

        // 事件
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnMessageReceived;
        public event Action<string> OnError;

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
                webSocket.Close();
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
            webSocket.Send(json);
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

