package handler

import (
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
	"github.com/sanguo-strategy/server/internal/game"
)

// MatchHandler 匹配处理器
type MatchHandler struct {
	gameManager *game.GameManager
}

// NewMatchHandler 创建匹配处理器
func NewMatchHandler(gm *game.GameManager) *MatchHandler {
	return &MatchHandler{gameManager: gm}
}

// JoinMatchmaking 加入匹配
func (h *MatchHandler) JoinMatchmaking(c *gin.Context) {
	// TODO: 从 JWT 获取玩家信息
	playerID := uuid.New() // Mock
	username := "Player"   // Mock
	rank := 1000           // Mock

	if err := h.gameManager.JoinMatchmaking(playerID, username, rank); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": "加入匹配队列成功",
		"status":  "waiting",
	})
}

// LeaveMatchmaking 离开匹配
func (h *MatchHandler) LeaveMatchmaking(c *gin.Context) {
	// TODO: 从 JWT 获取玩家 ID
	playerID := uuid.New() // Mock

	h.gameManager.LeaveMatchmaking(playerID)

	c.JSON(http.StatusOK, gin.H{
		"message": "离开匹配队列成功",
	})
}

// GetMatchStatus 获取匹配状态
func (h *MatchHandler) GetMatchStatus(c *gin.Context) {
	// TODO: 查询匹配状态
	c.JSON(http.StatusOK, gin.H{
		"status":           "waiting",
		"wait_time":        15, // 秒
		"players_in_queue": 8,
	})
}
