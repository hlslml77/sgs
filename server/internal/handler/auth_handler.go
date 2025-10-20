package handler

import (
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/sanguo-strategy/server/internal/config"
)

// AuthHandler 认证处理器
type AuthHandler struct {
	config *config.Config
}

// NewAuthHandler 创建认证处理器
func NewAuthHandler(cfg *config.Config) *AuthHandler {
	return &AuthHandler{config: cfg}
}

// RegisterRequest 注册请求
type RegisterRequest struct {
	Username string `json:"username" binding:"required,min=3,max=20"`
	Email    string `json:"email" binding:"required,email"`
	Password string `json:"password" binding:"required,min=6"`
}

// LoginRequest 登录请求
type LoginRequest struct {
	Username string `json:"username" binding:"required"`
	Password string `json:"password" binding:"required"`
}

// SteamAuthRequest Steam 认证请求
type SteamAuthRequest struct {
	SteamID     string `json:"steam_id" binding:"required"`
	SteamTicket string `json:"steam_ticket" binding:"required"`
}

// Register 注册
func (h *AuthHandler) Register(c *gin.Context) {
	var req RegisterRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// TODO: 实现注册逻辑
	// 1. 检查用户名/邮箱是否已存在
	// 2. 加密密码
	// 3. 创建用户记录
	// 4. 生成 JWT token

	c.JSON(http.StatusOK, gin.H{
		"message": "注册成功",
		"token":   "mock_jwt_token",
		"user": gin.H{
			"id":       "mock_user_id",
			"username": req.Username,
			"email":    req.Email,
		},
	})
}

// Login 登录
func (h *AuthHandler) Login(c *gin.Context) {
	var req LoginRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// TODO: 实现登录逻辑
	// 1. 查询用户
	// 2. 验证密码
	// 3. 生成 JWT token

	c.JSON(http.StatusOK, gin.H{
		"message": "登录成功",
		"token":   "mock_jwt_token",
		"user": gin.H{
			"id":       "mock_user_id",
			"username": req.Username,
		},
	})
}

// SteamAuth Steam 认证
func (h *AuthHandler) SteamAuth(c *gin.Context) {
	var req SteamAuthRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// TODO: 实现 Steam 认证逻辑
	// 1. 验证 Steam Ticket
	// 2. 获取 Steam 用户信息
	// 3. 创建/更新用户记录
	// 4. 生成 JWT token

	c.JSON(http.StatusOK, gin.H{
		"message": "Steam 认证成功",
		"token":   "mock_jwt_token",
		"user": gin.H{
			"id":       "mock_user_id",
			"steam_id": req.SteamID,
		},
	})
}
