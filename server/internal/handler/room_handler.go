package handler

import (
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
	"github.com/sanguo-strategy/server/internal/game"
)

// RoomHandler 房间处理器
type RoomHandler struct {
	gameManager *game.GameManager
}

// NewRoomHandler 创建房间处理器
func NewRoomHandler(gm *game.GameManager) *RoomHandler {
	return &RoomHandler{gameManager: gm}
}

// CreateRoomRequest 创建房间请求
type CreateRoomRequest struct {
	Name             string `json:"name" binding:"required"`
	MaxPlayers       int    `json:"max_players" binding:"required,min=2,max=6"`
	GameMode         string `json:"game_mode" binding:"required"`
	VictoryCondition string `json:"victory_condition"`
	EnableSynergy    bool   `json:"enable_synergy"`
	EnableRandomPick bool   `json:"enable_random_pick"`
}

// ListRooms 列出所有房间
func (h *RoomHandler) ListRooms(c *gin.Context) {
	rooms := h.gameManager.ListRooms()
	c.JSON(http.StatusOK, gin.H{
		"rooms": rooms,
		"total": len(rooms),
	})
}

// CreateRoom 创建房间
func (h *RoomHandler) CreateRoom(c *gin.Context) {
	var req CreateRoomRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// TODO: 从 JWT 获取玩家 ID
	playerID := uuid.New() // Mock

	roomConfig := &game.RoomConfig{
		Name:             req.Name,
		MaxPlayers:       req.MaxPlayers,
		GameMode:         req.GameMode,
		VictoryCondition: req.VictoryCondition,
		EnableSynergy:    req.EnableSynergy,
		EnableRandomPick: req.EnableRandomPick,
	}

	room, err := h.gameManager.CreateRoom(playerID, roomConfig)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": "房间创建成功",
		"room":    room.GetInfo(),
	})
}

// JoinRoom 加入房间
func (h *RoomHandler) JoinRoom(c *gin.Context) {
	roomID, err := uuid.Parse(c.Param("id"))
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "无效的房间ID"})
		return
	}

	// TODO: 从 JWT 获取玩家信息
	playerID := uuid.New() // Mock
	username := "Player"   // Mock

	if err := h.gameManager.JoinRoom(roomID, playerID, username); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": "加入房间成功",
	})
}

// LeaveRoom 离开房间
func (h *RoomHandler) LeaveRoom(c *gin.Context) {
	roomID, err := uuid.Parse(c.Param("id"))
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "无效的房间ID"})
		return
	}

	// TODO: 从 JWT 获取玩家 ID
	playerID := uuid.New() // Mock

	if err := h.gameManager.LeaveRoom(roomID, playerID); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": "离开房间成功",
	})
}

// SetReady 设置准备状态
func (h *RoomHandler) SetReady(c *gin.Context) {
	roomID, err := uuid.Parse(c.Param("id"))
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "无效的房间ID"})
		return
	}

	var req struct {
		Ready bool `json:"ready"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	room, err := h.gameManager.GetRoom(roomID)
	if err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "房间不存在"})
		return
	}

	// TODO: 从 JWT 获取玩家 ID
	playerID := uuid.New() // Mock

	if err := room.SetPlayerReady(playerID, req.Ready); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": "状态更新成功",
	})
}

// StartGame 开始游戏
func (h *RoomHandler) StartGame(c *gin.Context) {
	roomID, err := uuid.Parse(c.Param("id"))
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "无效的房间ID"})
		return
	}

	if err := h.gameManager.StartGame(roomID); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": "游戏开始",
	})
}

// GetRoomInfo 获取房间信息
func (h *RoomHandler) GetRoomInfo(c *gin.Context) {
	roomID, err := uuid.Parse(c.Param("id"))
	if err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "无效的房间ID"})
		return
	}

	room, err := h.gameManager.GetRoom(roomID)
	if err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "房间不存在"})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"room": room.GetInfo(),
	})
}
