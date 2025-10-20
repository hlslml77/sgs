-- 创建表结构
USE sanguo_strategy;

-- 玩家表
CREATE TABLE IF NOT EXISTS players (
    id CHAR(36) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    steam_id VARCHAR(50),
    level INT DEFAULT 1,
    experience INT DEFAULT 0,
    coins INT DEFAULT 1000,
    `rank` INT DEFAULT 1000,
    avatar VARCHAR(255),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_steam_id (steam_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 武将表
CREATE TABLE IF NOT EXISTS generals (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    faction VARCHAR(20),
    role VARCHAR(20),
    hp INT NOT NULL,
    description TEXT,
    skills_json JSON,
    rarity VARCHAR(20),
    is_enabled BOOLEAN DEFAULT TRUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_faction (faction),
    INDEX idx_role (role),
    INDEX idx_rarity (rarity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 地形表
CREATE TABLE IF NOT EXISTS terrains (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    type VARCHAR(20),
    category VARCHAR(20),
    effects_json JSON,
    description TEXT,
    rarity VARCHAR(20),
    is_enabled BOOLEAN DEFAULT TRUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_type (type),
    INDEX idx_category (category),
    INDEX idx_rarity (rarity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 房间表
CREATE TABLE IF NOT EXISTS rooms (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(100),
    host_id CHAR(36),
    max_players INT DEFAULT 6,
    current_players INT DEFAULT 0,
    status VARCHAR(20),
    game_mode VARCHAR(50),
    config_json JSON,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_host_id (host_id),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 游戏历史表
CREATE TABLE IF NOT EXISTS game_histories (
    id CHAR(36) PRIMARY KEY,
    room_id CHAR(36),
    winner_team VARCHAR(20),
    duration INT,
    players_json JSON,
    events_json JSON,
    terrain_json JSON,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_room_id (room_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 玩家统计表
CREATE TABLE IF NOT EXISTS player_stats (
    id CHAR(36) PRIMARY KEY,
    player_id CHAR(36) UNIQUE,
    total_games INT DEFAULT 0,
    wins INT DEFAULT 0,
    losses INT DEFAULT 0,
    win_rate DOUBLE DEFAULT 0,
    favorite_general VARCHAR(50),
    total_play_time INT DEFAULT 0,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE INDEX idx_player_id (player_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

