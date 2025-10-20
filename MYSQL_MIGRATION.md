# MySQL 迁移说明

## 概述

本项目已从 PostgreSQL 迁移到 MySQL 8.0。本文档说明了主要变化和迁移步骤。

## 主要变化

### 1. 数据库驱动

**之前（PostgreSQL）：**
```go
import "gorm.io/driver/postgres"
```

**现在（MySQL）：**
```go
import "gorm.io/driver/mysql"
```

### 2. 连接字符串（DSN）

**PostgreSQL DSN：**
```go
dsn := fmt.Sprintf(
    "host=%s user=%s password=%s dbname=%s port=%d sslmode=%s",
    host, user, password, dbname, port, sslmode,
)
```

**MySQL DSN：**
```go
dsn := fmt.Sprintf(
    "%s:%s@tcp(%s:%d)/%s?charset=utf8mb4&parseTime=True&loc=Local",
    user, password, host, port, dbname,
)
```

### 3. UUID 类型

**PostgreSQL：**
```go
ID uuid.UUID `gorm:"type:uuid;primary_key;default:gen_random_uuid()"`
```

**MySQL：**
```go
ID uuid.UUID `gorm:"type:char(36);primary_key"`
```

MySQL 不原生支持 UUID 类型，使用 CHAR(36) 存储。UUID 生成在应用层通过 BeforeCreate 钩子完成。

### 4. JSON 类型

**PostgreSQL：**
```go
SkillsJSON string `gorm:"type:jsonb"`
```

**MySQL：**
```go
SkillsJSON string `gorm:"type:json"`
```

MySQL 8.0 支持原生 JSON 类型（注意是 `json` 而不是 `jsonb`）。

### 5. 字段长度

MySQL 需要为 VARCHAR 字段指定长度：

```go
// PostgreSQL
Username string `gorm:"uniqueIndex;not null"`

// MySQL
Username string `gorm:"uniqueIndex;not null;size:50"`
```

### 6. 默认端口

- PostgreSQL: 5432
- MySQL: 3306

## 依赖变化

### go.mod

**移除：**
```go
github.com/lib/pq v1.10.9
gorm.io/driver/postgres v1.5.4
github.com/jackc/pgpassfile v1.0.0
github.com/jackc/pgservicefile v0.0.0-20221227161230-091c0ba34f0a
github.com/jackc/pgx/v5 v5.4.3
```

**添加：**
```go
gorm.io/driver/mysql v1.5.2
github.com/go-sql-driver/mysql v1.7.1
```

## Docker 配置变化

### docker-compose.yml

**之前：**
```yaml
postgres:
  image: postgres:14-alpine
  environment:
    POSTGRES_DB: sanguo_strategy
    POSTGRES_USER: sanguo_user
    POSTGRES_PASSWORD: sanguo_password
  ports:
    - "5432:5432"
```

**现在：**
```yaml
mysql:
  image: mysql:8.0
  environment:
    MYSQL_ROOT_PASSWORD: root_password
    MYSQL_DATABASE: sanguo_strategy
    MYSQL_USER: sanguo_user
    MYSQL_PASSWORD: sanguo_password
  ports:
    - "3306:3306"
  command: --default-authentication-plugin=mysql_native_password --character-set-server=utf8mb4 --collation-server=utf8mb4_unicode_ci
```

## SQL 脚本变化

### PostgreSQL 特定语法

**之前：**
```sql
INSERT INTO generals (name, faction, ...) VALUES
('关羽', '蜀', ...);
```

**现在：**
```sql
-- 添加 UUID 生成和时间戳
INSERT INTO generals (id, name, faction, ..., created_at, updated_at) VALUES
(UUID(), '关羽', '蜀', ..., NOW(), NOW());
```

### 主要差异

1. **UUID 生成**：PostgreSQL 使用 `gen_random_uuid()`，MySQL 使用 `UUID()`
2. **时间戳**：PostgreSQL 可以使用默认值，MySQL 需要显式指定 `NOW()`
3. **字符集**：MySQL 需要在创建数据库时指定 `CHARACTER SET utf8mb4`

## 配置文件变化

### config.example.yaml

**移除的字段：**
```yaml
sslmode: "disable"  # MySQL 不需要此配置
```

**端口变化：**
```yaml
port: 3306  # 之前是 5432
```

## 迁移步骤

### 1. 更新依赖

```bash
cd server
go mod tidy
```

### 2. 启动 MySQL 容器

```bash
docker-compose up -d mysql
```

### 3. 验证数据库

```bash
# 连接到 MySQL
docker exec -it sanguo_mysql mysql -u sanguo_user -p

# 查看数据库
SHOW DATABASES;
USE sanguo_strategy;
SHOW TABLES;

# 查看数据
SELECT * FROM generals;
SELECT * FROM terrains;
```

### 4. 启动服务器

```bash
docker-compose up -d server
```

### 5. 测试 API

```bash
# 健康检查
curl http://localhost:8080/health

# 获取武将列表
curl http://localhost:8080/api/v1/data/generals
```

## 从 PostgreSQL 数据迁移（可选）

如果您有现有的 PostgreSQL 数据需要迁移：

### 1. 导出 PostgreSQL 数据

```bash
# 导出为 SQL
pg_dump -U sanguo_user -d sanguo_strategy -a -t generals > generals.sql
pg_dump -U sanguo_user -d sanguo_strategy -a -t terrains > terrains.sql
pg_dump -U sanguo_user -d sanguo_strategy -a -t players > players.sql
```

### 2. 转换 SQL 语法

需要手动或使用脚本转换：
- UUID 生成函数
- 数据类型差异
- 函数和操作符差异

### 3. 导入 MySQL

```bash
mysql -u sanguo_user -p sanguo_strategy < converted_data.sql
```

## 性能考虑

### MySQL 优化

1. **索引优化**
```sql
-- 为常用查询创建索引
CREATE INDEX idx_generals_faction_role ON generals(faction, role);
CREATE INDEX idx_terrains_type_category ON terrains(type, category);
```

2. **连接池配置**
```yaml
database:
  max_open_conns: 50
  max_idle_conns: 10
```

3. **查询缓存**（MySQL 8.0 已移除查询缓存，使用 Redis）

### MySQL vs PostgreSQL 性能对比

| 特性 | PostgreSQL | MySQL |
|------|-----------|-------|
| 读取性能 | 优秀 | 优秀 |
| 写入性能 | 良好 | 优秀 |
| 复杂查询 | 优秀 | 良好 |
| JSON 支持 | JSONB | JSON |
| 全文搜索 | 内置 | InnoDB FTS |

## 常见问题

### Q: 为什么要从 PostgreSQL 迁移到 MySQL？

A: 
- MySQL 在游戏服务器中更常用
- 更简单的部署和维护
- 更好的写入性能
- 更广泛的云服务支持

### Q: UUID 性能影响？

A: 在 MySQL 中，UUID 作为 CHAR(36) 可能略慢于整数主键，但对于本项目的规模影响不大。如果需要优化，可以考虑：
- 使用二进制 UUID（BINARY(16)）
- 使用自增 ID + UUID 组合

### Q: JSON 字段查询？

A: MySQL 8.0 支持 JSON 函数：
```sql
-- 查询 JSON 字段
SELECT * FROM generals WHERE JSON_EXTRACT(skills_json, '$[0].id') = 'wusheng';

-- 在 GORM 中
db.Where("JSON_EXTRACT(skills_json, '$[0].id') = ?", "wusheng").Find(&generals)
```

### Q: 事务隔离级别？

A: MySQL InnoDB 默认使用 REPEATABLE READ，与 PostgreSQL 的 READ COMMITTED 略有不同。如需调整：

```go
db.Exec("SET SESSION TRANSACTION ISOLATION LEVEL READ COMMITTED")
```

## 兼容性注意事项

### 1. 字符集

始终使用 `utf8mb4`：
```sql
CREATE DATABASE sanguo_strategy 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;
```

### 2. 时区

MySQL DSN 中设置时区：
```go
dsn := "...?parseTime=True&loc=Local"
```

### 3. SQL 模式

严格模式下可能遇到的问题：
```sql
-- 查看当前模式
SELECT @@sql_mode;

-- 如需调整（不推荐）
SET sql_mode = 'TRADITIONAL';
```

## 回滚到 PostgreSQL

如果需要回滚到 PostgreSQL：

1. 恢复 `server/go.mod` 中的 PostgreSQL 驱动
2. 恢复 `server/internal/database/database.go` 中的连接代码
3. 恢复 `server/internal/database/models.go` 中的类型定义
4. 恢复 `server/docker-compose.yml` 中的 PostgreSQL 服务
5. 运行 `go mod tidy`

## 总结

MySQL 迁移已完成，主要变化包括：
- ✅ 数据库驱动更新
- ✅ 连接字符串格式变化
- ✅ UUID 类型适配
- ✅ JSON 类型调整
- ✅ Docker 配置更新
- ✅ SQL 初始化脚本适配
- ✅ 文档全面更新

所有功能保持不变，API 接口完全兼容。

---

*迁移完成日期：2024年1月*
*MySQL 版本：8.0*
*GORM 版本：1.25.5*

