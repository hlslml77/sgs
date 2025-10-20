package game

import (
	"sync"
	"time"

	"github.com/google/uuid"
	"github.com/sanguo-strategy/server/internal/config"
	"github.com/sanguo-strategy/server/internal/models"
	"github.com/sanguo-strategy/server/pkg/logger"
)

// Room 游戏房间
type Room struct {
	ID          uuid.UUID
	HostID      uuid.UUID
	Config      *RoomConfig
	GameState   *models.GameState
	Players     map[uuid.UUID]*RoomPlayer
	playerMutex sync.RWMutex
	Status      RoomStatus
	CreatedAt   time.Time
	gameConfig  *config.Config
}

// RoomStatus 房间状态
type RoomStatus string

const (
	StatusWaiting  RoomStatus = "waiting"
	StatusPlaying  RoomStatus = "playing"
	StatusFinished RoomStatus = "finished"
)

// RoomConfig 房间配置
type RoomConfig struct {
	Name             string
	MaxPlayers       int
	GameMode         string // fixed_terrain/random_terrain
	TerrainPackID    string
	VictoryCondition string // kill_lord/terrain_control/resource
	EnableSynergy    bool   // 是否启用羁绊
	EnableRandomPick bool   // 是否强制随机选将
	ProtectedSlots   int    // 保护位数量
}

// RoomPlayer 房间中的玩家
type RoomPlayer struct {
	PlayerID  uuid.UUID
	Username  string
	IsReady   bool
	Team      string
	JoinedAt  time.Time
	Connected bool
}

// NewRoom 创建新房间
func NewRoom(hostID uuid.UUID, config *RoomConfig, gameConfig *config.Config) *Room {
	return &Room{
		ID:         uuid.New(),
		HostID:     hostID,
		Config:     config,
		Players:    make(map[uuid.UUID]*RoomPlayer),
		Status:     StatusWaiting,
		CreatedAt:  time.Now(),
		gameConfig: gameConfig,
	}
}

// AddPlayer 添加玩家
func (r *Room) AddPlayer(playerID uuid.UUID, username string) error {
	r.playerMutex.Lock()
	defer r.playerMutex.Unlock()

	if len(r.Players) >= r.Config.MaxPlayers {
		return ErrRoomFull
	}

	if _, exists := r.Players[playerID]; exists {
		return nil // 玩家已在房间中
	}

	r.Players[playerID] = &RoomPlayer{
		PlayerID:  playerID,
		Username:  username,
		IsReady:   playerID == r.HostID, // 房主自动准备
		JoinedAt:  time.Now(),
		Connected: true,
	}

	logger.Info("Player %s joined room %s", username, r.ID)
	r.BroadcastRoomUpdate()
	return nil
}

// RemovePlayer 移除玩家
func (r *Room) RemovePlayer(playerID uuid.UUID) {
	r.playerMutex.Lock()
	defer r.playerMutex.Unlock()

	if player, exists := r.Players[playerID]; exists {
		delete(r.Players, playerID)
		logger.Info("Player %s left room %s", player.Username, r.ID)
		r.BroadcastRoomUpdate()
	}
}

// GetPlayers 获取所有玩家
func (r *Room) GetPlayers() []*RoomPlayer {
	r.playerMutex.RLock()
	defer r.playerMutex.RUnlock()

	players := make([]*RoomPlayer, 0, len(r.Players))
	for _, p := range r.Players {
		players = append(players, p)
	}
	return players
}

// SetPlayerReady 设置玩家准备状态
func (r *Room) SetPlayerReady(playerID uuid.UUID, ready bool) error {
	r.playerMutex.Lock()
	defer r.playerMutex.Unlock()

	player, exists := r.Players[playerID]
	if !exists {
		return ErrRoomNotFound
	}

	player.IsReady = ready
	r.BroadcastRoomUpdate()
	return nil
}

// StartGame 开始游戏
func (r *Room) StartGame() error {
	r.playerMutex.Lock()
	defer r.playerMutex.Unlock()

	// 检查所有玩家是否准备
	if len(r.Players) < 2 {
		return &GameError{"not_enough_players", "玩家人数不足"}
	}

	for _, player := range r.Players {
		if !player.IsReady {
			return &GameError{"players_not_ready", "有玩家未准备"}
		}
	}

	// 初始化游戏状态
	r.GameState = r.initGameState()
	r.Status = StatusPlaying

	logger.Info("Game started in room %s", r.ID)
	r.BroadcastGameStart()
	return nil
}

// HandleAction 处理玩家操作
func (r *Room) HandleAction(action *models.ActionRequest) (*models.ActionResponse, error) {
	if r.Status != StatusPlaying {
		return &models.ActionResponse{
			Success: false,
			Message: "游戏未开始",
		}, nil
	}

	// TODO: 实现具体的游戏逻辑
	// 这里是核心游戏逻辑的入口
	switch action.ActionType {
	case "play_card":
		return r.handlePlayCard(action)
	case "use_skill":
		return r.handleUseSkill(action)
	case "move":
		return r.handleMove(action)
	case "deploy_terrain":
		return r.handleDeployTerrain(action)
	default:
		return &models.ActionResponse{
			Success: false,
			Message: "未知的操作类型",
		}, nil
	}
}

// initGameState 初始化游戏状态
func (r *Room) initGameState() *models.GameState {
	state := &models.GameState{
		RoomID:             r.ID,
		Round:              1,
		CurrentPhase:       models.PhasePrepare,
		CurrentPlayerIndex: 0,
		Players:            make([]*models.PlayerState, 0),
		Terrains:           make(map[models.Position]*models.TerrainState),
		Events:             make([]models.GameEvent, 0),
		IsFinished:         false,
	}

	// 初始化玩家状态
	teamIndex := 0
	teams := []string{"red", "blue"}
	for _, player := range r.Players {
		playerState := &models.PlayerState{
			PlayerID:     player.PlayerID,
			Username:     player.Username,
			Team:         teams[teamIndex%len(teams)],
			Generals:     make([]*models.GeneralState, 0),
			HandCards:    make([]*models.Card, 0),
			IsAlive:      true,
			IsReady:      true,
			ActionPoints: 3,
		}
		state.Players = append(state.Players, playerState)
		teamIndex++
	}

	return state
}

// GetInfo 获取房间信息
func (r *Room) GetInfo() *RoomInfo {
	r.playerMutex.RLock()
	defer r.playerMutex.RUnlock()

	return &RoomInfo{
		ID:             r.ID,
		Name:           r.Config.Name,
		HostID:         r.HostID,
		MaxPlayers:     r.Config.MaxPlayers,
		CurrentPlayers: len(r.Players),
		Status:         string(r.Status),
		GameMode:       r.Config.GameMode,
	}
}

// BroadcastRoomUpdate 广播房间更新
func (r *Room) BroadcastRoomUpdate() {
	// TODO: 通过 WebSocket 广播房间更新
	logger.Debug("Broadcasting room update for room %s", r.ID)
}

// BroadcastGameStart 广播游戏开始
func (r *Room) BroadcastGameStart() {
	// TODO: 通过 WebSocket 广播游戏开始
	logger.Debug("Broadcasting game start for room %s", r.ID)
}

// 游戏操作处理函数
func (r *Room) handlePlayCard(action *models.ActionRequest) (*models.ActionResponse, error) {
	// TODO: 实现打牌逻辑
	return &models.ActionResponse{Success: true, Message: "出牌成功"}, nil
}

func (r *Room) handleUseSkill(action *models.ActionRequest) (*models.ActionResponse, error) {
	// TODO: 实现技能使用逻辑
	return &models.ActionResponse{Success: true, Message: "技能使用成功"}, nil
}

func (r *Room) handleMove(action *models.ActionRequest) (*models.ActionResponse, error) {
	// TODO: 实现移动逻辑
	return &models.ActionResponse{Success: true, Message: "移动成功"}, nil
}

func (r *Room) handleDeployTerrain(action *models.ActionRequest) (*models.ActionResponse, error) {
	// TODO: 实现地形部署逻辑
	return &models.ActionResponse{Success: true, Message: "地形部署成功"}, nil
}
