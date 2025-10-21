package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/sanguo-strategy/server/internal/config"
	"github.com/sanguo-strategy/server/internal/database"
	"github.com/sanguo-strategy/server/internal/game"
	"github.com/sanguo-strategy/server/internal/router"
	"github.com/sanguo-strategy/server/pkg/logger"
)

func main() {
	// 加载配置
	cfg, err := config.Load("config/config.yaml")
	if err != nil {
		log.Fatalf("Failed to load config: %v", err)
	}

	// 初始化日志
	logger.Init(cfg.Logging.Level, cfg.Logging.Output)
	logger.Info("Starting Sanguo Strategy Server...")

	// 初始化数据库
	db, err := database.InitDB(cfg)
	if err != nil {
		logger.Fatal("Failed to initialize database: %v", err)
	}
	logger.Info("Database initialized successfully")

	// 初始化 Redis
	rdb := database.InitRedis(cfg)
	logger.Info("Redis initialized successfully")

	// 设置 Gin 模式
	gin.SetMode(cfg.Server.Mode)

	// 初始化游戏管理器
	gameManager := game.NewGameManager(db, rdb, cfg)
	logger.Info("Game manager initialized")

	// 启动游戏管理器
	go gameManager.Start()

	// 设置路由
	r := router.SetupRouter(gameManager, cfg, db)

	// 创建 HTTP 服务器
	srv := &http.Server{
		Addr:    fmt.Sprintf("%s:%d", cfg.Server.Host, cfg.Server.Port),
		Handler: r,
	}

	// 启动服务器
	go func() {
		logger.Info("Server listening on %s:%d", cfg.Server.Host, cfg.Server.Port)
		if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			logger.Fatal("Failed to start server: %v", err)
		}
	}()

	// 优雅关闭
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	logger.Info("Shutting down server...")

	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	if err := srv.Shutdown(ctx); err != nil {
		logger.Fatal("Server forced to shutdown: %v", err)
	}

	// 停止游戏管理器
	gameManager.Stop()

	logger.Info("Server exited")
}
