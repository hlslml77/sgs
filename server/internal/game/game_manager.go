package game

import (
	"sync"

	"github.com/google/uuid"
	"github.com/redis/go-redis/v9"
	"github.com/sanguo-strategy/server/internal/config"
	"github.com/sanguo-strategy/server/internal/models"
	"github.com/sanguo-strategy/server/pkg/logger"
	"gorm.io/gorm"
)

// GameManager 游戏管理器
type GameManager struct {
	db         *gorm.DB
	redis      *redis.Client
	config     *config.Config
	rooms      map[uuid.UUID]*Room
	roomsMutex sync.RWMutex
	matchQueue *MatchmakingQueue
	running    bool
	stopChan   chan struct{}
}

// NewGameManager 创建游戏管理器
func NewGameManager(db *gorm.DB, redis *redis.Client, cfg *config.Config) *GameManager {
	return &GameManager{
		db:         db,
		redis:      redis,
		config:     cfg,
		rooms:      make(map[uuid.UUID]*Room),
		matchQueue: NewMatchmakingQueue(),
		stopChan:   make(chan struct{}),
	}
}

// Start 启动游戏管理器
func (gm *GameManager) Start() {
	gm.running = true
	logger.Info("Game manager started")

	// 启动匹配队列处理
	go gm.matchQueue.Start()

	<-gm.stopChan
}

// Stop 停止游戏管理器
func (gm *GameManager) Stop() {
	if gm.running {
		gm.running = false
		close(gm.stopChan)
		gm.matchQueue.Stop()
		logger.Info("Game manager stopped")
	}
}

// CreateRoom 创建房间
func (gm *GameManager) CreateRoom(hostID uuid.UUID, config *RoomConfig) (*Room, error) {
	gm.roomsMutex.Lock()
	defer gm.roomsMutex.Unlock()

	if len(gm.rooms) >= gm.config.Game.MaxRooms {
		return nil, ErrTooManyRooms
	}

	room := NewRoom(hostID, config, gm.config)
	gm.rooms[room.ID] = room

	logger.Info("Room created: %s by player %s", room.ID, hostID)
	return room, nil
}

// GetRoom 获取房间
func (gm *GameManager) GetRoom(roomID uuid.UUID) (*Room, error) {
	gm.roomsMutex.RLock()
	defer gm.roomsMutex.RUnlock()

	room, exists := gm.rooms[roomID]
	if !exists {
		return nil, ErrRoomNotFound
	}
	return room, nil
}

// JoinRoom 加入房间
func (gm *GameManager) JoinRoom(roomID, playerID uuid.UUID, username string) error {
	room, err := gm.GetRoom(roomID)
	if err != nil {
		return err
	}

	return room.AddPlayer(playerID, username)
}

// LeaveRoom 离开房间
func (gm *GameManager) LeaveRoom(roomID, playerID uuid.UUID) error {
	room, err := gm.GetRoom(roomID)
	if err != nil {
		return err
	}

	room.RemovePlayer(playerID)

	// 如果房间空了，删除房间
	if len(room.GetPlayers()) == 0 {
		gm.roomsMutex.Lock()
		delete(gm.rooms, roomID)
		gm.roomsMutex.Unlock()
		logger.Info("Room deleted: %s (empty)", roomID)
	}

	return nil
}

// StartGame 开始游戏
func (gm *GameManager) StartGame(roomID uuid.UUID) error {
	room, err := gm.GetRoom(roomID)
	if err != nil {
		return err
	}

	return room.StartGame()
}

// HandleAction 处理玩家操作
func (gm *GameManager) HandleAction(roomID uuid.UUID, action *models.ActionRequest) (*models.ActionResponse, error) {
	room, err := gm.GetRoom(roomID)
	if err != nil {
		return nil, err
	}

	return room.HandleAction(action)
}

// ListRooms 列出所有房间
func (gm *GameManager) ListRooms() []*RoomInfo {
	gm.roomsMutex.RLock()
	defer gm.roomsMutex.RUnlock()

	rooms := make([]*RoomInfo, 0, len(gm.rooms))
	for _, room := range gm.rooms {
		rooms = append(rooms, room.GetInfo())
	}
	return rooms
}

// JoinMatchmaking 加入匹配队列
func (gm *GameManager) JoinMatchmaking(playerID uuid.UUID, username string, rank int) error {
	return gm.matchQueue.AddPlayer(playerID, username, rank)
}

// LeaveMatchmaking 离开匹配队列
func (gm *GameManager) LeaveMatchmaking(playerID uuid.UUID) {
	gm.matchQueue.RemovePlayer(playerID)
}

// RoomInfo 房间信息
type RoomInfo struct {
	ID             uuid.UUID
	Name           string
	HostID         uuid.UUID
	MaxPlayers     int
	CurrentPlayers int
	Status         string
	GameMode       string
}

// 错误定义
var (
	ErrTooManyRooms  = &GameError{"too_many_rooms", "已达到最大房间数量"}
	ErrRoomNotFound  = &GameError{"room_not_found", "房间不存在"}
	ErrRoomFull      = &GameError{"room_full", "房间已满"}
	ErrGameNotReady  = &GameError{"game_not_ready", "游戏未准备好"}
	ErrInvalidAction = &GameError{"invalid_action", "无效的操作"}
)

// GameError 游戏错误
type GameError struct {
	Code    string
	Message string
}

func (e *GameError) Error() string {
	return e.Message
}
