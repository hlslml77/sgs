-- 三国策略游戏数据库初始化脚本
-- 警告：此脚本将删除现有数据库并重新创建！
-- 如果只想创建表结构而不删除数据，请注释掉DROP DATABASE行

-- 删除旧数据库（如果存在）
DROP DATABASE IF EXISTS sanguo_strategy;

-- 创建新数据库
CREATE DATABASE sanguo_strategy CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE sanguo_strategy;

-- 玩家表
CREATE TABLE IF NOT EXISTS players (
    id CHAR(36) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    steam_id VARCHAR(50),
    `level` INT DEFAULT 1,
    experience INT DEFAULT 0,
    coins INT DEFAULT 1000,
    `rank` INT DEFAULT 1000,
    avatar VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_steam_id (steam_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 武将表
CREATE TABLE IF NOT EXISTS generals (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    faction VARCHAR(20) NOT NULL,
    role VARCHAR(20) NOT NULL,
    hp INT NOT NULL,
    description TEXT,
    skills_json JSON,
    rarity VARCHAR(20),
    is_enabled BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_faction (faction),
    INDEX idx_role (role),
    INDEX idx_rarity (rarity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 地形表
CREATE TABLE IF NOT EXISTS terrains (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    type VARCHAR(20) NOT NULL,
    category VARCHAR(20) NOT NULL,
    effects_json JSON,
    description TEXT,
    rarity VARCHAR(20),
    is_enabled BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_type (type),
    INDEX idx_category (category),
    INDEX idx_rarity (rarity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 房间表
CREATE TABLE IF NOT EXISTS rooms (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(100),
    host_id CHAR(36) NOT NULL,
    max_players INT DEFAULT 6,
    current_players INT DEFAULT 0,
    status VARCHAR(20) NOT NULL,
    game_mode VARCHAR(50),
    config_json JSON,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_host_id (host_id),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 游戏历史表
CREATE TABLE IF NOT EXISTS game_histories (
    id CHAR(36) PRIMARY KEY,
    room_id CHAR(36) NOT NULL,
    winner_team VARCHAR(20),
    duration INT,
    players_json JSON,
    events_json JSON,
    terrain_json JSON,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_room_id (room_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 玩家统计表
CREATE TABLE IF NOT EXISTS player_stats (
    id CHAR(36) PRIMARY KEY,
    player_id CHAR(36) NOT NULL UNIQUE,
    total_games INT DEFAULT 0,
    wins INT DEFAULT 0,
    losses INT DEFAULT 0,
    win_rate DOUBLE DEFAULT 0,
    favorite_general VARCHAR(50),
    total_play_time INT DEFAULT 0,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_player_id (player_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 插入测试武将数据
INSERT INTO generals (id, name, faction, `role`, hp, description, skills_json, rarity, is_enabled) VALUES
(UUID(), '曹操', '魏', '输出', 4, '魏武帝，乱世之奸雄，治世之能臣', '["奸雄","护驾"]', '传说', TRUE),
(UUID(), '刘备', '蜀', '辅助', 4, '蜀汉昭烈帝，仁德之君', '["仁德","激将"]', '传说', TRUE),
(UUID(), '孙权', '吴', '控制', 4, '东吴大帝，江东霸主', '["制衡","救援"]', '传说', TRUE),
(UUID(), '诸葛亮', '蜀', '控制', 3, '卧龙，蜀汉丞相，智谋无双', '["观星","空城"]', '传说', TRUE),
(UUID(), '司马懿', '魏', '特殊', 3, '仲达，魏国重臣，隐忍待时', '["反馈","鬼才"]', '传说', TRUE),
(UUID(), '周瑜', '吴', '输出', 3, '公瑾，东吴都督，雄姿英发', '["英姿","反间"]', '传说', TRUE),
(UUID(), '吕布', '群', '输出', 4, '飞将军，三国第一猛将', '["无双","马术"]', '传说', TRUE),
(UUID(), '貂蝉', '群', '控制', 3, '四大美女之一，离间之计', '["离间","闭月"]', '史诗', TRUE),
(UUID(), '关羽', '蜀', '输出', 4, '武圣，义薄云天', '["武圣","义绝"]', '传说', TRUE),
(UUID(), '张飞', '蜀', '输出', 4, '燕人张翼德，虎牢咆哮', '["嫉恶","咆哮"]', '史诗', TRUE);

-- 插入测试地形数据
INSERT INTO terrains (id, name, `type`, category, effects_json, description, rarity, is_enabled) VALUES
(UUID(), '草原', '基础', '平原', '{"move_cost": 1, "defense_bonus": 0}', '平坦的草原，适合骑兵奔驰', '普通', TRUE),
(UUID(), '山地', '基础', '山地', '{"move_cost": 2, "defense_bonus": 1}', '崎岖的山地，易守难攻', '普通', TRUE),
(UUID(), '森林', '基础', '森林', '{"move_cost": 2, "hide_bonus": 1}', '茂密的森林，可以隐蔽行踪', '普通', TRUE),
(UUID(), '河流', '基础', '河流', '{"move_cost": 3, "defense_bonus": 1}', '奔腾的河流，难以跨越', '普通', TRUE),
(UUID(), '城池', '交互', '建筑', '{"defense_bonus": 2, "heal_per_turn": 1}', '坚固的城池，可以恢复兵力', '稀有', TRUE),
(UUID(), '粮仓', '交互', '建筑', '{"resource_bonus": 2}', '储存粮草的仓库', '稀有', TRUE),
(UUID(), '兵营', '交互', '建筑', '{"recruit_bonus": 1}', '可以招募士兵的营地', '稀有', TRUE),
(UUID(), '火焰陷阱', '事件', '陷阱', '{"damage": 1, "trigger": "enter"}', '隐藏的火焰陷阱，触发后造成伤害', '史诗', TRUE),
(UUID(), '迷雾', '事件', '天气', '{"vision_range": -1}', '浓雾弥漫，视野受限', '稀有', TRUE),
(UUID(), '宝箱', '交互', '道具', '{"reward": "random"}', '神秘的宝箱，可能获得意外收获', '史诗', TRUE);

-- 创建测试用户
INSERT INTO players (id, username, email, password_hash, `level`, experience, coins, `rank`) VALUES
(UUID(), 'admin', 'admin@sanguo.com', '$2a$10$5tYWxZ7KjYxGJZqKqY9QXu5yYx9YxJxJxJxJxJxJxJxJxJxJxJxJx', 10, 5000, 10000, 2000),
(UUID(), 'test', 'test@sanguo.com', '$2a$10$5tYWxZ7KjYxGJZqKqY9QXu5yYx9YxJxJxJxJxJxJxJxJxJxJxJxJx', 5, 1000, 5000, 1500);

-- 为测试用户创建统计数据
INSERT INTO player_stats (id, player_id, total_games, wins, losses, win_rate)
SELECT UUID(), id, 0, 0, 0, 0 FROM players;

COMMIT;

