package logger

import (
	"fmt"
	"log"
	"os"
	"strings"
	"time"
)

type LogLevel int

const (
	DEBUG LogLevel = iota
	INFO
	WARN
	ERROR
	FATAL
)

var (
	currentLevel LogLevel = INFO
	logger       *log.Logger
)

func Init(level, output string) {
	// 设置日志级别
	switch strings.ToLower(level) {
	case "debug":
		currentLevel = DEBUG
	case "info":
		currentLevel = INFO
	case "warn":
		currentLevel = WARN
	case "error":
		currentLevel = ERROR
	default:
		currentLevel = INFO
	}

	// 设置输出
	if output == "file" {
		// TODO: 实现文件输出
		logger = log.New(os.Stdout, "", 0)
	} else {
		logger = log.New(os.Stdout, "", 0)
	}
}

func formatMessage(level LogLevel, format string, args ...interface{}) string {
	timestamp := time.Now().Format("2006-01-02 15:04:05")
	levelStr := ""
	switch level {
	case DEBUG:
		levelStr = "DEBUG"
	case INFO:
		levelStr = "INFO"
	case WARN:
		levelStr = "WARN"
	case ERROR:
		levelStr = "ERROR"
	case FATAL:
		levelStr = "FATAL"
	}
	message := fmt.Sprintf(format, args...)
	return fmt.Sprintf("[%s] [%s] %s", timestamp, levelStr, message)
}

func Debug(format string, args ...interface{}) {
	if currentLevel <= DEBUG {
		logger.Println(formatMessage(DEBUG, format, args...))
	}
}

func Info(format string, args ...interface{}) {
	if currentLevel <= INFO {
		logger.Println(formatMessage(INFO, format, args...))
	}
}

func Warn(format string, args ...interface{}) {
	if currentLevel <= WARN {
		logger.Println(formatMessage(WARN, format, args...))
	}
}

func Error(format string, args ...interface{}) {
	if currentLevel <= ERROR {
		logger.Println(formatMessage(ERROR, format, args...))
	}
}

func Fatal(format string, args ...interface{}) {
	logger.Println(formatMessage(FATAL, format, args...))
	os.Exit(1)
}
