-- MySQL 初始化游戏数据脚本
-- 注意：MySQL 在 Docker 初始化时会自动创建数据库，这里只需要插入数据

-- 蜀国武将
INSERT INTO generals (id, name, faction, role, hp, skills_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '关羽', '蜀', '输出', 4, '[{"id":"wusheng","name":"武圣","type":"主动","description":"你可以将一张红色牌当【杀】使用或打出"}]', '传说', '忠义无双的武圣', 1, NOW(), NOW()),
(UUID(), '张飞', '蜀', '控制', 4, '[{"id":"paoxiao","name":"咆哮","type":"锁定","description":"你使用【杀】无次数限制"}]', '史诗', '万夫不当的猛将', 1, NOW(), NOW()),
(UUID(), '诸葛亮', '蜀', '辅助', 3, '[{"id":"guanxing","name":"观星","type":"主动","description":"准备阶段观看牌堆顶X张牌"},{"id":"kongcheng","name":"空城","type":"锁定","description":"无手牌时不能成为【杀】或【决斗】目标"}]', '传说', '神机妙算的卧龙', 1, NOW(), NOW()),
(UUID(), '赵云', '蜀', '输出', 4, '[{"id":"longdan","name":"龙胆","type":"主动","description":"【杀】和【闪】可以互相转化"}]', '史诗', '一身是胆的常山赵子龙', 1, NOW(), NOW()),
(UUID(), '马超', '蜀', '输出', 4, '[{"id":"tieqi","name":"铁骑","type":"主动","description":"使用【杀】指定目标后，可以判定，若为红色目标不能使用【闪】"}]', '史诗', '西凉猛将', 1, NOW(), NOW()),
(UUID(), '黄月英', '蜀', '辅助', 3, '[{"id":"jizhi","name":"集智","type":"主动","description":"使用锦囊牌后可以摸一张牌"}]', '稀有', '诸葛亮之妻', 1, NOW(), NOW()),
(UUID(), '魏延', '蜀', '输出', 4, '[{"id":"kuanggu","name":"狂骨","type":"主动","description":"造成伤害后可以回复1点体力"}]', '稀有', '反骨仔', 1, NOW(), NOW());

-- 魏国武将
INSERT INTO generals (id, name, faction, role, hp, skills_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '曹操', '魏', '输出', 4, '[{"id":"jianxiong","name":"奸雄","type":"主动","description":"受到伤害后，可以获得造成伤害的牌"}]', '传说', '治世之能臣，乱世之奸雄', 1, NOW(), NOW()),
(UUID(), '司马懿', '魏', '控制', 3, '[{"id":"fankui","name":"反馈","type":"主动","description":"受到伤害后，可以获得伤害来源的一张牌"},{"id":"guicai","name":"鬼才","type":"主动","description":"判定时可以打出手牌代替判定牌"}]', '传说', '深谋远虑的冢虎', 1, NOW(), NOW()),
(UUID(), '夏侯惇', '魏', '输出', 4, '[{"id":"ganglie","name":"刚烈","type":"主动","description":"受到伤害后，可以判定，若为红色伤害来源弃两张牌或受到1点伤害"}]', '史诗', '盲夏侯', 1, NOW(), NOW()),
(UUID(), '张辽', '魏', '输出', 4, '[{"id":"tuxi","name":"突袭","type":"主动","description":"摸牌阶段可以少摸牌并获得对手的手牌"}]', '史诗', '威震逍遥津', 1, NOW(), NOW()),
(UUID(), '许褚', '魏', '输出', 4, '[{"id":"luoyi","name":"裸衣","type":"主动","description":"摸牌阶段可以少摸牌，使用【杀】或【决斗】伤害+1"}]', '稀有', '虎痴', 1, NOW(), NOW()),
(UUID(), '郭嘉', '魏', '辅助', 3, '[{"id":"tiandu","name":"天妒","type":"主动","description":"判定后可以获得判定牌"},{"id":"yiji","name":"遗计","type":"主动","description":"受到伤害后可以观看两张牌"}]', '传说', '鬼才郭奉孝', 1, NOW(), NOW()),
(UUID(), '甄姬', '魏', '辅助', 3, '[{"id":"luoshen","name":"洛神","type":"主动","description":"准备阶段可以判定，为黑色则获得之"}]', '史诗', '洛神赋', 1, NOW(), NOW());

-- 吴国武将
INSERT INTO generals (id, name, faction, role, hp, skills_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '孙权', '吴', '控制', 4, '[{"id":"zhiheng","name":"制衡","type":"主动","description":"出牌阶段可以弃牌重新摸等量的牌"}]', '传说', '年轻的江东之主', 1, NOW(), NOW()),
(UUID(), '周瑜', '吴', '输出', 3, '[{"id":"yingzi","name":"英姿","type":"锁定","description":"摸牌阶段多摸一张牌"},{"id":"fanjian","name":"反间","type":"主动","description":"令对手选花色并获得你的手牌"}]', '传说', '大都督公瑾', 1, NOW(), NOW()),
(UUID(), '甘宁', '吴', '输出', 4, '[{"id":"qixi","name":"奇袭","type":"主动","description":"黑色牌可以当【过河拆桥】使用"}]', '史诗', '锦帆游侠', 1, NOW(), NOW()),
(UUID(), '陆逊', '吴', '控制', 3, '[{"id":"qianxun","name":"谦逊","type":"锁定","description":"不能成为【顺手牵羊】和【乐不思蜀】的目标"},{"id":"lianying","name":"连营","type":"主动","description":"失去最后的手牌时可以摸一张牌"}]', '史诗', '儒生陆伯言', 1, NOW(), NOW()),
(UUID(), '孙尚香', '吴', '输出', 3, '[{"id":"jieyin","name":"结姻","type":"主动","description":"出牌阶段可以弃两张牌令自己和另一名男性角色各回复1点体力"}]', '稀有', '弓腰姬', 1, NOW(), NOW()),
(UUID(), '大乔', '吴', '辅助', 3, '[{"id":"guose","name":"国色","type":"主动","description":"方块牌可以当【乐不思蜀】使用"}]', '稀有', '江东二乔', 1, NOW(), NOW()),
(UUID(), '黄盖', '吴', '输出', 4, '[{"id":"kurou","name":"苦肉","type":"主动","description":"出牌阶段可以失去1点体力并摸两张牌"}]', '稀有', '苦肉计', 1, NOW(), NOW());

-- 群雄武将
INSERT INTO generals (id, name, faction, role, hp, skills_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '吕布', '群', '输出', 4, '[{"id":"wushuang","name":"无双","type":"锁定","description":"使用【杀】或【决斗】需要两张【闪】或【杀】响应"}]', '传说', '三国第一猛将', 1, NOW(), NOW()),
(UUID(), '貂蝉', '群', '控制', 3, '[{"id":"lijian","name":"离间","type":"主动","description":"出牌阶段可以弃一张牌令两名男性角色【决斗】"}]', '传说', '闭月', 1, NOW(), NOW()),
(UUID(), '华佗', '群', '辅助', 3, '[{"id":"jijiu","name":"急救","type":"主动","description":"红色牌可以当【桃】使用"},{"id":"qingnang","name":"青囊","type":"主动","description":"出牌阶段可以弃一张牌令任意角色回复1点体力"}]', '史诗', '神医', 1, NOW(), NOW()),
(UUID(), '吕蒙', '吴', '控制', 3, '[{"id":"keji","name":"克己","type":"锁定","description":"若在弃牌阶段没有弃牌，跳过此阶段"}]', '史诗', '士别三日当刮目相待', 1, NOW(), NOW()),
(UUID(), '袁绍', '群', '控制', 4, '[{"id":"luanji","name":"乱击","type":"主动","description":"出牌阶段可以将两张同花色手牌当【万箭齐发】使用"}]', '稀有', '四世三公', 1, NOW(), NOW()),
(UUID(), '颜良文丑', '群', '输出', 4, '[{"id":"shuangxiong","name":"双雄","type":"主动","description":"摸牌阶段可以放弃摸牌并进行判定"}]', '稀有', '河北双雄', 1, NOW(), NOW());

-- 插入地形数据
INSERT INTO terrains (id, name, type, category, effects_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '山地', '基础', '山地', '[{"type":"range_bonus","value":1,"trigger":"stay","description":"远程攻击范围+1"}]', '普通', '高耸的山地，适合远程攻击', 1, NOW(), NOW()),
(UUID(), '平原', '基础', '平原', '[{"type":"movement_bonus","value":1,"trigger":"stay","description":"移动力+1"}]', '普通', '开阔的平原，便于移动', 1, NOW(), NOW()),
(UUID(), '森林', '基础', '森林', '[{"type":"fire_amplify","value":1,"trigger":"stay","description":"火焰伤害+1"}]', '普通', '茂密的森林，易燃', 1, NOW(), NOW()),
(UUID(), '河流', '基础', '河流', '[{"type":"movement_cost","value":1,"trigger":"enter","description":"进入需额外消耗1行动点"}]', '普通', '湍急的河流，阻碍移动', 1, NOW(), NOW()),
(UUID(), '沼泽', '基础', '沼泽', '[{"type":"damage","value":1,"trigger":"stay","description":"停留时受到1点伤害"}]', '普通', '危险的沼泽地', 1, NOW(), NOW()),
(UUID(), '城墙', '基础', '防御', '[{"type":"defense_bonus","value":1,"trigger":"stay","description":"受到伤害-1"}]', '普通', '坚固的城墙', 1, NOW(), NOW());

INSERT INTO terrains (id, name, type, category, effects_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '祭坛', '事件', '特殊', '[{"type":"revive","trigger":"action","description":"可消耗资源复活阵亡武将"}]', '稀有', '神秘的祭坛，可以复活武将', 1, NOW(), NOW()),
(UUID(), '烽火台', '交互', '特殊', '[{"type":"fire_range","value":2,"trigger":"action","description":"点燃后对直线造成火焰伤害"}]', '稀有', '烽火台，可以点燃攻击敌人', 1, NOW(), NOW()),
(UUID(), '兵营', '基础', '特殊', '[{"type":"recruit","trigger":"control","description":"控制方每回合可招募小兵"}]', '稀有', '军事要地', 1, NOW(), NOW()),
(UUID(), '粮仓', '交互', '特殊', '[{"type":"supply","trigger":"action","description":"可获得额外手牌"}]', '稀有', '储存粮草的粮仓', 1, NOW(), NOW());

INSERT INTO terrains (id, name, type, category, effects_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '血宴祭坛', '事件', '特殊', '[{"type":"aoe_judgment","trigger":"round_start","description":"回合开始时全体判定，失败者受伤"}]', '史诗', '恐怖的血宴祭坛', 1, NOW(), NOW()),
(UUID(), '七星台', '事件', '特殊', '[{"type":"observation","trigger":"action","description":"诸葛亮可额外观星一次"}]', '史诗', '诸葛亮的专属地形', 1, NOW(), NOW()),
(UUID(), '子午谷', '基础', '山地', '[{"type":"ambush","trigger":"action","description":"张辽突袭无视距离"}]', '稀有', '适合伏击的险要地形', 1, NOW(), NOW()),
(UUID(), '铜雀台', '事件', '特殊', '[{"type":"charm","trigger":"synergy","description":"貂蝉在此地形技能增强"}]', '史诗', '曹操的铜雀台', 1, NOW(), NOW());

INSERT INTO terrains (id, name, type, category, effects_json, rarity, description, is_enabled, created_at, updated_at) VALUES
(UUID(), '赤壁', '事件', '水域', '[{"type":"fire_water","trigger":"synergy","description":"火攻威力大增"}]', '传说', '著名的赤壁之战地点', 1, NOW(), NOW()),
(UUID(), '长坂坡', '基础', '平原', '[{"type":"breakthrough","trigger":"synergy","description":"赵云在此地形战斗力增强"}]', '稀有', '赵云七进七出之地', 1, NOW(), NOW()),
(UUID(), '五丈原', '事件', '平原', '[{"type":"star_array","trigger":"synergy","description":"诸葛亮可布置星象阵"}]', '传说', '诸葛亮陨落之地', 1, NOW(), NOW()),
(UUID(), '邺城', '基础', '城池', '[{"type":"resource_bonus","trigger":"control","description":"控制方每回合获得额外资源"}]', '稀有', '魏国重要城池', 1, NOW(), NOW()),
(UUID(), '虎牢关', '基础', '关隘', '[{"type":"defense","trigger":"control","description":"吕布在此地形防御力增强"}]', '史诗', '吕布三英战之地', 1, NOW(), NOW());

-- 创建初始管理员账号（密码: admin123，实际使用时需要在应用层加密）
-- INSERT INTO players (id, username, email, password_hash, level, coins, rank, created_at, updated_at) VALUES
-- (UUID(), 'admin', 'admin@sanguo-strategy.com', '$2a$10$...', 99, 999999, 9999, NOW(), NOW());
