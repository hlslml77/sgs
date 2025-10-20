package game

import (
	"sync"
	"time"

	"github.com/google/uuid"
	"github.com/sanguo-strategy/server/pkg/logger"
)

// MatchmakingQueue 匹配队列
type MatchmakingQueue struct {
	players      map[uuid.UUID]*MatchPlayer
	playersMutex sync.RWMutex
	running      bool
	stopChan     chan struct{}
}

// MatchPlayer 匹配中的玩家
type MatchPlayer struct {
	PlayerID  uuid.UUID
	Username  string
	Rank      int
	JoinTime  time.Time
	Preferred MatchPreference
}

// MatchPreference 匹配偏好
type MatchPreference struct {
	Role         string // 偏好角色：输出/控制/辅助
	MaxPlayers   int    // 期望房间人数
	AcceptRandom bool   // 是否接受随机匹配
}

// NewMatchmakingQueue 创建匹配队列
func NewMatchmakingQueue() *MatchmakingQueue {
	return &MatchmakingQueue{
		players:  make(map[uuid.UUID]*MatchPlayer),
		stopChan: make(chan struct{}),
	}
}

// Start 启动匹配队列
func (mq *MatchmakingQueue) Start() {
	mq.running = true
	logger.Info("Matchmaking queue started")

	ticker := time.NewTicker(2 * time.Second)
	defer ticker.Stop()

	for {
		select {
		case <-ticker.C:
			mq.processMatches()
		case <-mq.stopChan:
			logger.Info("Matchmaking queue stopped")
			return
		}
	}
}

// Stop 停止匹配队列
func (mq *MatchmakingQueue) Stop() {
	if mq.running {
		mq.running = false
		close(mq.stopChan)
	}
}

// AddPlayer 添加玩家到匹配队列
func (mq *MatchmakingQueue) AddPlayer(playerID uuid.UUID, username string, rank int) error {
	mq.playersMutex.Lock()
	defer mq.playersMutex.Unlock()

	if _, exists := mq.players[playerID]; exists {
		return &GameError{"already_in_queue", "已在匹配队列中"}
	}

	mq.players[playerID] = &MatchPlayer{
		PlayerID: playerID,
		Username: username,
		Rank:     rank,
		JoinTime: time.Now(),
	}

	logger.Info("Player %s added to matchmaking queue (rank: %d)", username, rank)
	return nil
}

// RemovePlayer 从匹配队列移除玩家
func (mq *MatchmakingQueue) RemovePlayer(playerID uuid.UUID) {
	mq.playersMutex.Lock()
	defer mq.playersMutex.Unlock()

	if player, exists := mq.players[playerID]; exists {
		delete(mq.players, playerID)
		logger.Info("Player %s removed from matchmaking queue", player.Username)
	}
}

// processMatches 处理匹配
func (mq *MatchmakingQueue) processMatches() {
	mq.playersMutex.Lock()
	defer mq.playersMutex.Unlock()

	if len(mq.players) < 2 {
		return // 人数不足，无法匹配
	}

	// 简单匹配算法：按排位分相近的玩家匹配
	matches := mq.findMatches()
	for _, match := range matches {
		mq.createMatchedRoom(match)
	}
}

// findMatches 查找可匹配的玩家组
func (mq *MatchmakingQueue) findMatches() [][]*MatchPlayer {
	matches := make([][]*MatchPlayer, 0)

	// 将玩家转为切片
	players := make([]*MatchPlayer, 0, len(mq.players))
	for _, p := range mq.players {
		players = append(players, p)
	}

	// 简单匹配：每2-6个玩家匹配一组
	if len(players) >= 2 {
		// 取前6个玩家（如果有的话）
		matchSize := len(players)
		if matchSize > 6 {
			matchSize = 6
		}

		match := players[:matchSize]
		matches = append(matches, match)

		// 从队列中移除已匹配的玩家
		for _, p := range match {
			delete(mq.players, p.PlayerID)
		}
	}

	return matches
}

// createMatchedRoom 为匹配的玩家创建房间
func (mq *MatchmakingQueue) createMatchedRoom(players []*MatchPlayer) {
	// TODO: 调用 GameManager 创建房间并添加玩家
	logger.Info("Creating matched room for %d players", len(players))

	// 这里需要与 GameManager 交互
	// 暂时只记录日志
	for _, p := range players {
		logger.Debug("Matched player: %s (rank: %d)", p.Username, p.Rank)
	}
}
