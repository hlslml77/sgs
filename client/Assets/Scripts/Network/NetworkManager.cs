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
    /// 自动检测并使用最佳连接方式：
    /// - 如果有 WebSocket-Sharp: 使用原生 WebSocket
    /// - 如果没有: 使用 SimpleWebSocketClient 后备方案
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        // 服务器配置
        private string serverUrl = "ws://localhost:8080/ws";
        
#if WEBSOCKET_SHARP
        private WebSocket webSocket;
        private bool useNativeWebSocket = true;
#else
        private bool useNativeWebSocket = false;
#endif
        
        // 后备客户端组件（动态添加）
        private MonoBehaviour simpleClientComponent;
        private bool isConnected = false;

        // 事件
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnMessageReceived;
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
                Debug.Log("🚀 使用 WebSocket-Sharp 连接到服务器...");
                webSocket = new WebSocket(serverUrl);

                webSocket.OnOpen += (sender, e) =>
                {
                    isConnected = true;
                    Debug.Log("✅ 已连接到服务器 (WebSocket-Sharp)");
                    OnConnected?.Invoke();
                };

                webSocket.OnMessage += (sender, e) =>
                {
                    Debug.Log($"📨 收到消息: {e.Data}");
                    OnMessageReceived?.Invoke(e.Data);
                };

                webSocket.OnError += (sender, e) =>
                {
                    Debug.LogError($"❌ WebSocket 错误: {e.Message}");
                    OnError?.Invoke(e.Message);
                };

                webSocket.OnClose += (sender, e) =>
                {
                    isConnected = false;
                    Debug.Log("🔌 已断开服务器连接");
                    OnDisconnected?.Invoke();
                };

                webSocket.Connect();
#else
                Debug.LogWarning("⚠️ WebSocket-Sharp 未安装，使用后备连接方式...");
                Debug.LogWarning("💡 建议：使用 Unity 菜单「三国策略 -> 网络设置向导」安装 WebSocket-Sharp 以获得最佳性能");
                
                // 使用简单客户端作为后备（通过反射避免编译时依赖）
                if (simpleClientComponent == null)
                {
                    GameObject clientObj = new GameObject("SimpleWebSocketClient");
                    clientObj.transform.SetParent(transform);
                    
                    // 通过类型名称动态添加组件
                    var clientType = System.Type.GetType("SanguoStrategy.Network.SimpleWebSocketClient");
                    if (clientType != null)
                    {
                        simpleClientComponent = (MonoBehaviour)clientObj.AddComponent(clientType);
                        
                        // 使用反射订阅事件
                        var onConnectedEvent = clientType.GetEvent("OnConnected");
                        var onDisconnectedEvent = clientType.GetEvent("OnDisconnected");
                        var onMessageReceivedEvent = clientType.GetEvent("OnMessageReceived");
                        var onErrorEvent = clientType.GetEvent("OnError");
                        
                        if (onConnectedEvent != null)
                        {
                            onConnectedEvent.AddEventHandler(simpleClientComponent, (Action)(() =>
                            {
                                isConnected = true;
                                Debug.Log("✅ 已连接到服务器 (后备模式)");
                                OnConnected?.Invoke();
                            }));
                        }
                        
                        if (onDisconnectedEvent != null)
                        {
                            onDisconnectedEvent.AddEventHandler(simpleClientComponent, (Action)(() =>
                            {
                                isConnected = false;
                                Debug.Log("🔌 已断开服务器连接");
                                OnDisconnected?.Invoke();
                            }));
                        }
                        
                        if (onMessageReceivedEvent != null)
                        {
                            onMessageReceivedEvent.AddEventHandler(simpleClientComponent, (Action<string>)((msg) =>
                            {
                                Debug.Log($"📨 收到消息: {msg}");
                                OnMessageReceived?.Invoke(msg);
                            }));
                        }
                        
                        if (onErrorEvent != null)
                        {
                            onErrorEvent.AddEventHandler(simpleClientComponent, (Action<string>)((error) =>
                            {
                                Debug.LogError($"❌ 连接错误: {error}");
                                OnError?.Invoke(error);
                            }));
                        }
                    }
                    else
                    {
                        Debug.LogError("❌ 无法找到 SimpleWebSocketClient 类型");
                        OnError?.Invoke("SimpleWebSocketClient not found");
                        return;
                    }
                }
                
                // 调用 Connect 方法
                if (simpleClientComponent != null)
                {
                    var connectMethod = simpleClientComponent.GetType().GetMethod("Connect");
                    if (connectMethod != null)
                    {
                        connectMethod.Invoke(simpleClientComponent, new object[] { serverUrl });
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ 连接失败: {ex.Message}");
                OnError?.Invoke(ex.Message);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
#if WEBSOCKET_SHARP
            if (webSocket != null && isConnected)
            {
                webSocket.Close();
                webSocket = null;
                isConnected = false;
            }
#else
            if (simpleClientComponent != null)
            {
                var disconnectMethod = simpleClientComponent.GetType().GetMethod("Disconnect");
                if (disconnectMethod != null)
                {
                    disconnectMethod.Invoke(simpleClientComponent, null);
                }
                isConnected = false;
            }
#endif
        }

        /// <summary>
        /// 检查是否已连接
        /// </summary>
        public bool IsConnected()
        {
            return isConnected;
        }
        
        /// <summary>
        /// 获取连接类型
        /// </summary>
        public string GetConnectionType()
        {
#if WEBSOCKET_SHARP
            return "WebSocket-Sharp (推荐)";
#else
            return "后备模式 (建议安装 WebSocket-Sharp)";
#endif
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
            webSocket?.Send(json);
            Debug.Log($"📤 发送消息 (WebSocket): {json}");
#else
            if (simpleClientComponent != null)
            {
                var sendMethod = simpleClientComponent.GetType().GetMethod("Send");
                if (sendMethod != null)
                {
                    sendMethod.Invoke(simpleClientComponent, new object[] { json });
                    Debug.Log($"📤 发送消息 (后备模式): {json}");
                }
            }
#endif
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

