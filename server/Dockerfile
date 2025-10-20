# 多阶段构建
FROM golang:1.21-alpine AS builder

# 安装依赖
RUN apk add --no-cache git make

# 设置工作目录
WORKDIR /app

# 复制 go mod 文件
COPY go.mod go.sum ./
RUN go mod download

# 复制源代码
COPY . .

# 编译
RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o server cmd/server/main.go

# 运行阶段
FROM alpine:latest

RUN apk --no-cache add ca-certificates tzdata

WORKDIR /root/

# 从构建阶段复制二进制文件
COPY --from=builder /app/server .
COPY --from=builder /app/config ./config

# 暴露端口
EXPOSE 8080

# 运行
CMD ["./server"]

