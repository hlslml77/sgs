-- 创建测试用户脚本
-- 密码都是: test123
-- 使用bcrypt加密，cost=10

USE sanguo_strategy;

-- 清空现有测试数据（可选）
-- DELETE FROM player_stats WHERE player_id IN (SELECT id FROM players WHERE username LIKE 'test%');
-- DELETE FROM players WHERE username LIKE 'test%';

-- 创建测试用户
-- 注意：密码 "test123" 的 bcrypt hash
-- 你需要在实际使用前通过API或Go代码生成真实的hash
INSERT INTO players (id, username, email, password_hash, `level`, experience, coins, `rank`) VALUES
('11111111-1111-1111-1111-111111111111', 'test', 'test@example.com', '$2a$10$rS7jQkB5R.5iLB/HZw5JJ.xGxGW0D0D5w5R7B1iP.3Y5D5w5R7B1i', 1, 0, 1000, 1000),
('22222222-2222-2222-2222-222222222222', 'player1', 'player1@example.com', '$2a$10$rS7jQkB5R.5iLB/HZw5JJ.xGxGW0D0D5w5R7B1iP.3Y5D5w5R7B1i', 5, 2000, 5000, 1500),
('33333333-3333-3333-3333-333333333333', 'player2', 'player2@example.com', '$2a$10$rS7jQkB5R.5iLB/HZw5JJ.xGxGW0D0D5w5R7B1iP.3Y5D5w5R7B1i', 3, 800, 3000, 1200);

-- 为测试用户创建统计数据
INSERT INTO player_stats (id, player_id, total_games, wins, losses, win_rate) VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 0, 0, 0, 0),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '22222222-2222-2222-2222-222222222222', 10, 6, 4, 0.6),
('cccccccc-cccc-cccc-cccc-cccccccccccc', '33333333-3333-3333-3333-333333333333', 5, 3, 2, 0.6);

-- 提示
SELECT '✅ 测试用户创建成功！' as message;
SELECT 
    '测试账号' as `type`,
    username,
    email,
    'test123' as `password`,
    `level`,
    coins
FROM players 
WHERE username IN ('test', 'player1', 'player2');

