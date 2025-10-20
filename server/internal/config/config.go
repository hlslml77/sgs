package config

import (
	"os"

	"gopkg.in/yaml.v3"
)

type Config struct {
	Server   ServerConfig   `yaml:"server"`
	Database DatabaseConfig `yaml:"database"`
	Redis    RedisConfig    `yaml:"redis"`
	Game     GameConfig     `yaml:"game"`
	JWT      JWTConfig      `yaml:"jwt"`
	Steam    SteamConfig    `yaml:"steam"`
	Logging  LoggingConfig  `yaml:"logging"`
}

type ServerConfig struct {
	Host string `yaml:"host"`
	Port int    `yaml:"port"`
	Mode string `yaml:"mode"`
}

type DatabaseConfig struct {
	Host         string `yaml:"host"`
	Port         int    `yaml:"port"`
	User         string `yaml:"user"`
	Password     string `yaml:"password"`
	DBName       string `yaml:"dbname"`
	MaxOpenConns int    `yaml:"max_open_conns"`
	MaxIdleConns int    `yaml:"max_idle_conns"`
}

type RedisConfig struct {
	Host     string `yaml:"host"`
	Port     int    `yaml:"port"`
	Password string `yaml:"password"`
	DB       int    `yaml:"db"`
	PoolSize int    `yaml:"pool_size"`
}

type GameConfig struct {
	MaxRooms                int `yaml:"max_rooms"`
	MaxPlayersPerRoom       int `yaml:"max_players_per_room"`
	MinPlayersPerRoom       int `yaml:"min_players_per_room"`
	RoomTimeoutMinutes      int `yaml:"room_timeout_minutes"`
	MaxRounds               int `yaml:"max_rounds"`
	TurnTimeSeconds         int `yaml:"turn_time_seconds"`
	PrepareTimeSeconds      int `yaml:"prepare_time_seconds"`
	MatchmakingTimeout      int `yaml:"matchmaking_timeout_seconds"`
	RankDifferenceThreshold int `yaml:"rank_difference_threshold"`
}

type JWTConfig struct {
	SecretKey        string `yaml:"secret_key"`
	TokenExpireHours int    `yaml:"token_expire_hours"`
}

type SteamConfig struct {
	Enabled bool   `yaml:"enabled"`
	AppID   int    `yaml:"app_id"`
	APIKey  string `yaml:"api_key"`
}

type LoggingConfig struct {
	Level    string `yaml:"level"`
	Output   string `yaml:"output"`
	FilePath string `yaml:"file_path"`
}

// Load 加载配置文件
func Load(path string) (*Config, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, err
	}

	var cfg Config
	if err := yaml.Unmarshal(data, &cfg); err != nil {
		return nil, err
	}

	return &cfg, nil
}
