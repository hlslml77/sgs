using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SanguoStrategy.Network
{
    /// <summary>
    /// 简单的 WebSocket 客户端实现
    /// 使用 UnityWebRequest 作为后备方案
    /// 如果安装了 WebSocket-Sharp，NetworkManager 会使用那个库
    /// </summary>
    public class SimpleWebSocketClient : MonoBehaviour
    {
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnMessageReceived;
        public event Action<string> OnError;
        
        private string serverUrl;
        private bool isConnected = false;
        private bool shouldRun = false;
        private Queue<string> messageQueue = new Queue<string>();
        private Queue<string> sendQueue = new Queue<string>();
        
        /// <summary>
        /// 连接到服务器（使用长轮询模拟 WebSocket）
        /// </summary>
        public void Connect(string url)
        {
            if (isConnected)
            {
                Debug.LogWarning("[SimpleWebSocket] Already connected");
                return;
            }
            
            // 将 ws:// 转换为 http://
            serverUrl = url.Replace("ws://", "http://").Replace("wss://", "https://");
            
            shouldRun = true;
            StartCoroutine(ConnectionRoutine());
        }
        
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            shouldRun = false;
            isConnected = false;
            OnDisconnected?.Invoke();
        }
        
        /// <summary>
        /// 发送消息
        /// </summary>
        public void Send(string message)
        {
            if (!isConnected)
            {
                Debug.LogWarning("[SimpleWebSocket] Not connected, cannot send message");
                return;
            }
            
            sendQueue.Enqueue(message);
        }
        
        /// <summary>
        /// 检查是否已连接
        /// </summary>
        public bool IsConnected()
        {
            return isConnected;
        }
        
        private IEnumerator ConnectionRoutine()
        {
            Debug.Log($"[SimpleWebSocket] Connecting to {serverUrl}...");
            
            // 尝试连接（ping 服务器）
            string healthUrl = serverUrl.Replace("/ws", "/health");
            
            using (UnityWebRequest request = UnityWebRequest.Get(healthUrl))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    isConnected = true;
                    Debug.Log("[SimpleWebSocket] Connected successfully");
                    OnConnected?.Invoke();
                    
                    // 启动消息循环
                    StartCoroutine(ReceiveLoop());
                    StartCoroutine(SendLoop());
                }
                else
                {
                    Debug.LogError($"[SimpleWebSocket] Connection failed: {request.error}");
                    OnError?.Invoke(request.error);
                    shouldRun = false;
                }
            }
        }
        
        private IEnumerator ReceiveLoop()
        {
            while (shouldRun && isConnected)
            {
                // 处理接收到的消息
                while (messageQueue.Count > 0)
                {
                    string message = messageQueue.Dequeue();
                    OnMessageReceived?.Invoke(message);
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private IEnumerator SendLoop()
        {
            while (shouldRun && isConnected)
            {
                if (sendQueue.Count > 0)
                {
                    string message = sendQueue.Dequeue();
                    yield return StartCoroutine(SendMessage(message));
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private IEnumerator SendMessage(string message)
        {
            // 使用 HTTP POST 发送消息到服务器
            string apiUrl = serverUrl.Replace("/ws", "/api/v1/game/action");
            
            byte[] bodyRaw = Encoding.UTF8.GetBytes(message);
            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                // 添加认证 token（如果有）
                string token = PlayerPrefs.GetString("AuthToken", "");
                if (!string.IsNullOrEmpty(token))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {token}");
                }
                
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[SimpleWebSocket] Send failed: {request.error}");
                    OnError?.Invoke(request.error);
                }
                else
                {
                    // 服务器响应
                    string response = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(response))
                    {
                        messageQueue.Enqueue(response);
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            Disconnect();
        }
    }
}

