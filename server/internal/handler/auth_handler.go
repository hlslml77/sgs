package handler

import (
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/golang-jwt/jwt/v5"
	"github.com/google/uuid"
	"github.com/sanguo-strategy/server/internal/config"
	"github.com/sanguo-strategy/server/internal/database"
	"golang.org/x/crypto/bcrypt"
	"gorm.io/gorm"
)

// AuthHandler 认证处理器
type AuthHandler struct {
	config *config.Config
	db     *gorm.DB
}

// NewAuthHandler 创建认证处理器
func NewAuthHandler(cfg *config.Config, db *gorm.DB) *AuthHandler {
	return &AuthHandler{
		config: cfg,
		db:     db,
	}
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

	// 1. 检查用户名是否已存在
	var existingPlayer database.Player
	if err := h.db.Where("username = ?", req.Username).First(&existingPlayer).Error; err == nil {
		c.JSON(http.StatusConflict, gin.H{"error": "用户名已存在"})
		return
	}

	// 2. 检查邮箱是否已存在
	if err := h.db.Where("email = ?", req.Email).First(&existingPlayer).Error; err == nil {
		c.JSON(http.StatusConflict, gin.H{"error": "邮箱已被注册"})
		return
	}

	// 3. 加密密码
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(req.Password), bcrypt.DefaultCost)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "密码加密失败"})
		return
	}

	// 4. 创建用户记录
	player := database.Player{
		ID:           uuid.New(),
		Username:     req.Username,
		Email:        req.Email,
		PasswordHash: string(hashedPassword),
		Level:        1,
		Experience:   0,
		Coins:        1000,
		Rank:         1000,
	}

	if err := h.db.Create(&player).Error; err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "创建用户失败"})
		return
	}

	// 5. 创建玩家统计记录
	stats := database.PlayerStats{
		ID:       uuid.New(),
		PlayerID: player.ID,
	}
	h.db.Create(&stats)

	// 6. 生成 JWT token
	token, err := generateJWT(player.ID.String(), player.Username, h.config.JWT.Secret)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "生成token失败"})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message":  "注册成功",
		"token":    token,
		"user_id":  player.ID.String(),
		"username": player.Username,
	})
}

// Login 登录
func (h *AuthHandler) Login(c *gin.Context) {
	var req LoginRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// 1. 查询用户
	var player database.Player
	if err := h.db.Where("username = ?", req.Username).First(&player).Error; err != nil {
		c.JSON(http.StatusUnauthorized, gin.H{"error": "用户名或密码错误"})
		return
	}

	// 2. 验证密码
	if err := bcrypt.CompareHashAndPassword([]byte(player.PasswordHash), []byte(req.Password)); err != nil {
		c.JSON(http.StatusUnauthorized, gin.H{"error": "用户名或密码错误"})
		return
	}

	// 3. 生成 JWT token
	token, err := generateJWT(player.ID.String(), player.Username, h.config.JWT.Secret)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "生成token失败"})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message":  "登录成功",
		"token":    token,
		"user_id":  player.ID.String(),
		"username": player.Username,
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

// JWTClaims JWT 声明
type JWTClaims struct {
	UserID   string `json:"user_id"`
	Username string `json:"username"`
	jwt.RegisteredClaims
}

// generateJWT 生成 JWT token
func generateJWT(userID, username, secret string) (string, error) {
	claims := JWTClaims{
		UserID:   userID,
		Username: username,
		RegisteredClaims: jwt.RegisteredClaims{
			ExpiresAt: jwt.NewNumericDate(time.Now().Add(24 * time.Hour * 7)), // 7天过期
			IssuedAt:  jwt.NewNumericDate(time.Now()),
			NotBefore: jwt.NewNumericDate(time.Now()),
		},
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	return token.SignedString([]byte(secret))
}

// ValidateJWT 验证 JWT token
func ValidateJWT(tokenString, secret string) (*JWTClaims, error) {
	token, err := jwt.ParseWithClaims(tokenString, &JWTClaims{}, func(token *jwt.Token) (interface{}, error) {
		return []byte(secret), nil
	})

	if err != nil {
		return nil, err
	}

	if claims, ok := token.Claims.(*JWTClaims); ok && token.Valid {
		return claims, nil
	}

	return nil, jwt.ErrSignatureInvalid
}
