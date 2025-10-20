package handler

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

// PlayerHandler 玩家处理器
type PlayerHandler struct{}

// NewPlayerHandler 创建玩家处理器
func NewPlayerHandler() *PlayerHandler {
	return &PlayerHandler{}
}

// GetProfile 获取玩家资料
func (h *PlayerHandler) GetProfile(c *gin.Context) {
	// TODO: 从 JWT 获取玩家 ID，从数据库查询资料
	c.JSON(http.StatusOK, gin.H{
		"id":         "mock_user_id",
		"username":   "Player1",
		"level":      10,
		"experience": 2500,
		"rank":       1250,
		"coins":      5000,
		"wins":       45,
		"losses":     38,
		"win_rate":   54.2,
	})
}

// UpdateProfile 更新玩家资料
func (h *PlayerHandler) UpdateProfile(c *gin.Context) {
	var req struct {
		Avatar string `json:"avatar"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// TODO: 更新数据库

	c.JSON(http.StatusOK, gin.H{
		"message": "资料更新成功",
	})
}

// GetStats 获取玩家统计
func (h *PlayerHandler) GetStats(c *gin.Context) {
	// TODO: 从数据库查询统计数据
	c.JSON(http.StatusOK, gin.H{
		"total_games":       83,
		"wins":              45,
		"losses":            38,
		"win_rate":          54.2,
		"favorite_general":  "关羽",
		"total_play_time":   125400, // 秒
		"average_game_time": 1512,   // 秒
		"rank_history":      []int{1000, 1050, 1100, 1180, 1250},
	})
}
