package router

import (
	"github.com/gin-gonic/gin"
	"github.com/sanguo-strategy/server/internal/config"
	"github.com/sanguo-strategy/server/internal/game"
	"github.com/sanguo-strategy/server/internal/handler"
	"gorm.io/gorm"
)

// SetupRouter 设置路由
func SetupRouter(gameManager *game.GameManager, cfg *config.Config, db *gorm.DB) *gin.Engine {
	r := gin.Default()

	// 中间件
	r.Use(gin.Recovery())
	r.Use(corsMiddleware())

	// 健康检查
	r.GET("/health", func(c *gin.Context) {
		c.JSON(200, gin.H{
			"status":  "ok",
			"version": "1.0.0",
		})
	})

	// API v1
	v1 := r.Group("/api/v1")
	{
		// 认证相关
		auth := v1.Group("/auth")
		{
			authHandler := handler.NewAuthHandler(cfg, db)
			auth.POST("/register", authHandler.Register)
			auth.POST("/login", authHandler.Login)
			auth.POST("/steam-auth", authHandler.SteamAuth)
		}

		// 玩家相关
		player := v1.Group("/player")
		player.Use(authMiddleware(cfg))
		{
			playerHandler := handler.NewPlayerHandler()
			player.GET("/profile", playerHandler.GetProfile)
			player.PUT("/profile", playerHandler.UpdateProfile)
			player.GET("/stats", playerHandler.GetStats)
		}

		// 房间相关
		room := v1.Group("/room")
		room.Use(authMiddleware(cfg))
		{
			roomHandler := handler.NewRoomHandler(gameManager)
			room.GET("/list", roomHandler.ListRooms)
			room.POST("/create", roomHandler.CreateRoom)
			room.POST("/:id/join", roomHandler.JoinRoom)
			room.POST("/:id/leave", roomHandler.LeaveRoom)
			room.POST("/:id/ready", roomHandler.SetReady)
			room.POST("/:id/start", roomHandler.StartGame)
			room.GET("/:id/info", roomHandler.GetRoomInfo)
		}

		// 匹配相关
		match := v1.Group("/match")
		match.Use(authMiddleware(cfg))
		{
			matchHandler := handler.NewMatchHandler(gameManager)
			match.POST("/join", matchHandler.JoinMatchmaking)
			match.POST("/leave", matchHandler.LeaveMatchmaking)
			match.GET("/status", matchHandler.GetMatchStatus)
		}

		// 游戏数据相关
		data := v1.Group("/data")
		{
			dataHandler := handler.NewDataHandler()
			data.GET("/generals", dataHandler.GetGenerals)
			data.GET("/terrains", dataHandler.GetTerrains)
			data.GET("/skills", dataHandler.GetSkills)
		}
	}

	// WebSocket 连接
	r.GET("/ws", func(c *gin.Context) {
		handler.HandleWebSocket(c, gameManager)
	})

	return r
}

// corsMiddleware CORS 中间件
func corsMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		c.Writer.Header().Set("Access-Control-Allow-Origin", "*")
		c.Writer.Header().Set("Access-Control-Allow-Credentials", "true")
		c.Writer.Header().Set("Access-Control-Allow-Headers", "Content-Type, Content-Length, Accept-Encoding, X-CSRF-Token, Authorization, accept, origin, Cache-Control, X-Requested-With")
		c.Writer.Header().Set("Access-Control-Allow-Methods", "POST, OPTIONS, GET, PUT, DELETE")

		if c.Request.Method == "OPTIONS" {
			c.AbortWithStatus(204)
			return
		}

		c.Next()
	}
}

// authMiddleware 认证中间件
func authMiddleware(cfg *config.Config) gin.HandlerFunc {
	return func(c *gin.Context) {
		// 获取 Authorization header
		authHeader := c.GetHeader("Authorization")
		if authHeader == "" {
			c.JSON(401, gin.H{"error": "未提供认证token"})
			c.Abort()
			return
		}

		// 解析 Bearer token
		tokenString := authHeader
		if len(authHeader) > 7 && authHeader[:7] == "Bearer " {
			tokenString = authHeader[7:]
		}

		// 验证 JWT
		claims, err := handler.ValidateJWT(tokenString, cfg.JWT.Secret)
		if err != nil {
			c.JSON(401, gin.H{"error": "无效的token"})
			c.Abort()
			return
		}

		// 将用户信息存入 context
		c.Set("user_id", claims.UserID)
		c.Set("username", claims.Username)
		c.Next()
	}
}
