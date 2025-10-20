package models

import "github.com/google/uuid"

// 游戏内核心数据结构

// GameState 游戏状态
type GameState struct {
	RoomID             uuid.UUID
	Round              int
	CurrentPhase       GamePhase
	CurrentPlayerIndex int
	Players            []*PlayerState
	Terrains           map[Position]*TerrainState
	Events             []GameEvent
	Winner             string
	IsFinished         bool
}

// GamePhase 游戏阶段
type GamePhase string

const (
	PhasePrepare  GamePhase = "prepare"   // 准备阶段
	PhaseDeploy   GamePhase = "deploy"    // 部署阶段
	PhaseCombat   GamePhase = "combat"    // 战斗阶段
	PhaseEndRound GamePhase = "end_round" // 回合结束
)

// PlayerState 玩家状态
type PlayerState struct {
	PlayerID     uuid.UUID
	Username     string
	Team         string // red/blue
	Position     int
	Generals     []*GeneralState
	HandCards    []*Card
	IsAlive      bool
	IsReady      bool
	ActionPoints int
}

// GeneralState 武将状态
type GeneralState struct {
	GeneralID uuid.UUID
	Name      string
	Faction   string
	Role      string
	CurrentHP int
	MaxHP     int
	Position  Position
	Skills    []*Skill
	Buffs     []*Buff
	IsAlive   bool
	HasActed  bool
	Equipment []*Equipment
}

// Position 位置（棋盘坐标）
type Position struct {
	X int `json:"x"`
	Y int `json:"y"`
}

// TerrainState 地形状态
type TerrainState struct {
	TerrainID   uuid.UUID
	Name        string
	Type        string
	Position    Position
	Effects     []*TerrainEffect
	Owner       uuid.UUID // 控制该地形的玩家
	Duration    int       // 剩余回合数（-1 表示永久）
	IsActivated bool
}

// Skill 技能
type Skill struct {
	ID          string
	Name        string
	Type        string // 主动/被动/锁定技
	Description string
	Cooldown    int
	CurrentCD   int
	Effects     []SkillEffect
}

// SkillEffect 技能效果
type SkillEffect struct {
	Type      string // damage/heal/buff/terrain_modify
	Value     int
	Target    string      // self/ally/enemy/terrain
	Condition string      // 触发条件
	ExtraData interface{} // 额外数据
}

// Buff 增益/减益效果
type Buff struct {
	ID          string
	Name        string
	Type        string // buff/debuff
	Description string
	Duration    int // 剩余回合数
	Effects     map[string]int
	Source      uuid.UUID // 来源
}

// Card 卡牌
type Card struct {
	ID      uuid.UUID
	Type    string // 杀/闪/桃/地形牌
	Suit    string // 花色
	Value   int    // 点数
	Name    string
	Effects []CardEffect
}

// CardEffect 卡牌效果
type CardEffect struct {
	Type      string
	Value     int
	Target    string
	ExtraData interface{}
}

// Equipment 装备
type Equipment struct {
	ID          uuid.UUID
	Name        string
	Type        string // weapon/armor/mount
	Effects     []EquipmentEffect
	Description string
}

// EquipmentEffect 装备效果
type EquipmentEffect struct {
	Type  string
	Value int
}

// TerrainEffect 地形效果
type TerrainEffect struct {
	Type        string // damage/heal/buff/movement_limit
	Trigger     string // enter/stay/leave/action
	Value       int
	Condition   string
	Description string
}

// GameEvent 游戏事件
type GameEvent struct {
	Type      string
	PlayerID  uuid.UUID
	GeneralID uuid.UUID
	Action    string
	Data      interface{}
	Timestamp int64
}

// Synergy 羁绊
type Synergy struct {
	ID          string
	Name        string
	Generals    []uuid.UUID // 需要的武将
	Terrain     string      // 需要的地形（可选）
	Description string
	Effects     []SynergyEffect
	IsActivated bool
}

// SynergyEffect 羁绊效果
type SynergyEffect struct {
	Type        string
	Value       int
	Target      string
	Description string
}

// ActionRequest 玩家操作请求
type ActionRequest struct {
	PlayerID   uuid.UUID
	ActionType string // play_card/use_skill/move/deploy_terrain
	TargetID   uuid.UUID
	TargetPos  Position
	CardID     uuid.UUID
	SkillID    string
	ExtraData  map[string]interface{}
}

// ActionResponse 操作响应
type ActionResponse struct {
	Success bool
	Message string
	Events  []GameEvent
}
