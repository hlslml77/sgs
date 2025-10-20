.PHONY: help build run test clean docker-build docker-run migrate

help: ## 显示帮助信息
	@echo "Sanguo Strategy Server - 可用命令:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

build: ## 编译服务器
	@echo "Building server..."
	@go build -o bin/server cmd/server/main.go
	@echo "Build complete: bin/server"

run: ## 运行服务器
	@echo "Starting server..."
	@go run cmd/server/main.go

test: ## 运行测试
	@echo "Running tests..."
	@go test -v ./...

clean: ## 清理构建文件
	@echo "Cleaning..."
	@rm -rf bin/
	@rm -rf logs/
	@echo "Clean complete"

deps: ## 安装依赖
	@echo "Installing dependencies..."
	@go mod download
	@go mod tidy

lint: ## 运行代码检查
	@echo "Running linter..."
	@golangci-lint run ./...

format: ## 格式化代码
	@echo "Formatting code..."
	@go fmt ./...
	@goimports -w .

migrate: ## 运行数据库迁移
	@echo "Running database migration..."
	@psql -h localhost -U sanguo_user -d sanguo_strategy -f scripts/init_data.sql

docker-build: ## 构建 Docker 镜像
	@echo "Building Docker image..."
	@docker build -t sanguo-strategy-server:latest .

docker-run: ## 运行 Docker 容器
	@echo "Running Docker container..."
	@docker-compose up -d

docker-stop: ## 停止 Docker 容器
	@echo "Stopping Docker containers..."
	@docker-compose down

dev: ## 开发模式（热重载）
	@echo "Starting development server with hot reload..."
	@air

.DEFAULT_GOAL := help

