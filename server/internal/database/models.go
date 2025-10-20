package database

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

// Player 玩家模型
type Player struct {
	ID           uuid.UUID `gorm:"type:char(36);primary_key"`
	Username     string    `gorm:"uniqueIndex;not null;size:50"`
	Email        string    `gorm:"uniqueIndex;not null;size:100"`
	PasswordHash string    `gorm:"not null;size:255"`
	SteamID      string    `gorm:"index;size:50"`
	Level        int       `gorm:"default:1"`
	Experience   int       `gorm:"default:0"`
	Coins        int       `gorm:"default:1000"` // 欢乐豆
	Rank         int       `gorm:"default:1000"` // 排位分数
	Avatar       string    `gorm:"size:255"`
	CreatedAt    time.Time
	UpdatedAt    time.Time
}

// General 武将模型
type General struct {
	ID          uuid.UUID `gorm:"type:char(36);primary_key"`
	Name        string    `gorm:"uniqueIndex;not null;size:50"`
	Faction     string    `gorm:"index;size:20"` // 魏/蜀/吴/群
	Role        string    `gorm:"index;size:20"` // 输出/控制/辅助/特殊
	HP          int       `gorm:"not null"`
	Description string    `gorm:"type:text"`
	SkillsJSON  string    `gorm:"type:json"`     // 技能配置 JSON
	Rarity      string    `gorm:"index;size:20"` // 稀有度：普通/稀有/史诗/传说
	IsEnabled   bool      `gorm:"default:true"`
	CreatedAt   time.Time
	UpdatedAt   time.Time
}

// Terrain 地形模型
type Terrain struct {
	ID          uuid.UUID `gorm:"type:char(36);primary_key"`
	Name        string    `gorm:"uniqueIndex;not null;size:50"`
	Type        string    `gorm:"index;size:20"` // 基础/事件/交互
	Category    string    `gorm:"index;size:20"` // 山地/平原/河流/森林等
	EffectsJSON string    `gorm:"type:json"`     // 效果配置 JSON
	Description string    `gorm:"type:text"`
	Rarity      string    `gorm:"index;size:20"` // 稀有度
	IsEnabled   bool      `gorm:"default:true"`
	CreatedAt   time.Time
	UpdatedAt   time.Time
}

// Room 房间模型
type Room struct {
	ID             uuid.UUID `gorm:"type:char(36);primary_key"`
	Name           string    `gorm:"size:100"`
	HostID         uuid.UUID `gorm:"type:char(36);index"`
	MaxPlayers     int       `gorm:"default:6"`
	CurrentPlayers int       `gorm:"default:0"`
	Status         string    `gorm:"index;size:20"` // waiting/playing/finished
	GameMode       string    `gorm:"size:50"`       // 地形规则模式
	ConfigJSON     string    `gorm:"type:json"`     // 房间配置 JSON
	CreatedAt      time.Time
	UpdatedAt      time.Time
}

// GameHistory 游戏历史记录
type GameHistory struct {
	ID          uuid.UUID `gorm:"type:char(36);primary_key"`
	RoomID      uuid.UUID `gorm:"type:char(36);index"`
	WinnerTeam  string    `gorm:"size:20"` // 胜利阵营
	Duration    int       // 游戏时长（秒）
	PlayersJSON string    `gorm:"type:json"` // 玩家数据 JSON
	EventsJSON  string    `gorm:"type:json"` // 游戏事件 JSON
	TerrainJSON string    `gorm:"type:json"` // 地形布局 JSON
	CreatedAt   time.Time
}

// PlayerStats 玩家统计
type PlayerStats struct {
	ID              uuid.UUID `gorm:"type:char(36);primary_key"`
	PlayerID        uuid.UUID `gorm:"type:char(36);uniqueIndex"`
	TotalGames      int       `gorm:"default:0"`
	Wins            int       `gorm:"default:0"`
	Losses          int       `gorm:"default:0"`
	WinRate         float64   `gorm:"default:0"`
	FavoriteGeneral string    `gorm:"size:50"`
	TotalPlayTime   int       `gorm:"default:0"` // 总游戏时长（秒）
	UpdatedAt       time.Time
}

// BeforeCreate GORM 钩子
func (p *Player) BeforeCreate(tx *gorm.DB) error {
	if p.ID == uuid.Nil {
		p.ID = uuid.New()
	}
	return nil
}

func (g *General) BeforeCreate(tx *gorm.DB) error {
	if g.ID == uuid.Nil {
		g.ID = uuid.New()
	}
	return nil
}

func (t *Terrain) BeforeCreate(tx *gorm.DB) error {
	if t.ID == uuid.Nil {
		t.ID = uuid.New()
	}
	return nil
}

func (r *Room) BeforeCreate(tx *gorm.DB) error {
	if r.ID == uuid.Nil {
		r.ID = uuid.New()
	}
	return nil
}

func (gh *GameHistory) BeforeCreate(tx *gorm.DB) error {
	if gh.ID == uuid.Nil {
		gh.ID = uuid.New()
	}
	return nil
}

func (ps *PlayerStats) BeforeCreate(tx *gorm.DB) error {
	if ps.ID == uuid.Nil {
		ps.ID = uuid.New()
	}
	return nil
}
