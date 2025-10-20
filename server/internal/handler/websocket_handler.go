package handler

import (
	"encoding/json"
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
	"github.com/gorilla/websocket"
	"github.com/sanguo-strategy/server/internal/game"
	"github.com/sanguo-strategy/server/internal/models"
	"github.com/sanguo-strategy/server/pkg/logger"
)

var upgrader = websocket.Upgrader{
	ReadBufferSize:  1024,
	WriteBufferSize: 1024,
	CheckOrigin: func(r *http.Request) bool {
		return true // 生产环境需要严格检查
	},
}

// WSMessage WebSocket 消息
type WSMessage struct {
	Type     string          `json:"type"`
	RoomID   string          `json:"room_id,omitempty"`
	Data     json.RawMessage `json:"data,omitempty"`
	PlayerID string          `json:"player_id,omitempty"`
}

// HandleWebSocket 处理 WebSocket 连接
func HandleWebSocket(c *gin.Context, gameManager *game.GameManager) {
	conn, err := upgrader.Upgrade(c.Writer, c.Request, nil)
	if err != nil {
		logger.Error("Failed to upgrade websocket: %v", err)
		return
	}
	defer conn.Close()

	// TODO: 从 query 参数或 token 获取玩家 ID
	playerID := uuid.New()
	logger.Info("WebSocket connected: player %s", playerID)

	// 发送连接成功消息
	conn.WriteJSON(WSMessage{
		Type: "connected",
		Data: json.RawMessage(`{"message": "连接成功"}`),
	})

	// 设置心跳
	ticker := time.NewTicker(30 * time.Second)
	defer ticker.Stop()

	// 读取消息
	done := make(chan struct{})
	go func() {
		defer close(done)
		for {
			var msg WSMessage
			if err := conn.ReadJSON(&msg); err != nil {
				if websocket.IsUnexpectedCloseError(err, websocket.CloseGoingAway, websocket.CloseAbnormalClosure) {
					logger.Error("WebSocket error: %v", err)
				}
				return
			}

			// 处理消息
			handleWSMessage(conn, gameManager, playerID, &msg)
		}
	}()

	// 心跳和消息发送
	for {
		select {
		case <-ticker.C:
			if err := conn.WriteJSON(WSMessage{Type: "ping"}); err != nil {
				logger.Error("Failed to send ping: %v", err)
				return
			}
		case <-done:
			logger.Info("WebSocket disconnected: player %s", playerID)
			return
		}
	}
}

// handleWSMessage 处理 WebSocket 消息
func handleWSMessage(conn *websocket.Conn, gameManager *game.GameManager, playerID uuid.UUID, msg *WSMessage) {
	logger.Debug("Received WS message: type=%s, player=%s", msg.Type, playerID)

	switch msg.Type {
	case "pong":
		// 心跳响应
		return

	case "game_action":
		// 游戏操作
		roomID, err := uuid.Parse(msg.RoomID)
		if err != nil {
			sendError(conn, "无效的房间ID")
			return
		}

		var actionReq models.ActionRequest
		if err := json.Unmarshal(msg.Data, &actionReq); err != nil {
			sendError(conn, "无效的操作数据")
			return
		}

		actionReq.PlayerID = playerID
		response, err := gameManager.HandleAction(roomID, &actionReq)
		if err != nil {
			sendError(conn, err.Error())
			return
		}

		// 发送响应
		conn.WriteJSON(WSMessage{
			Type:   "action_response",
			RoomID: msg.RoomID,
			Data:   mustMarshal(response),
		})

	case "chat":
		// 聊天消息
		// TODO: 实现聊天功能
		logger.Debug("Chat message from player %s", playerID)

	default:
		sendError(conn, "未知的消息类型")
	}
}

// sendError 发送错误消息
func sendError(conn *websocket.Conn, message string) {
	conn.WriteJSON(WSMessage{
		Type: "error",
		Data: json.RawMessage(`{"message": "` + message + `"}`),
	})
}

// mustMarshal JSON 序列化（panic on error）
func mustMarshal(v interface{}) json.RawMessage {
	data, err := json.Marshal(v)
	if err != nil {
		panic(err)
	}
	return data
}
