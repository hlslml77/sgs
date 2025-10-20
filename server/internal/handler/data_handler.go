package handler

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

// DataHandler 游戏数据处理器
type DataHandler struct{}

// NewDataHandler 创建数据处理器
func NewDataHandler() *DataHandler {
	return &DataHandler{}
}

// GetGenerals 获取武将列表
func (h *DataHandler) GetGenerals(c *gin.Context) {
	// TODO: 从数据库查询武将数据
	generals := []gin.H{
		{
			"id":      "1",
			"name":    "关羽",
			"faction": "蜀",
			"role":    "输出",
			"hp":      4,
			"rarity":  "传说",
			"skills": []gin.H{
				{"id": "wusheng", "name": "武圣", "description": "你可以将一张红色牌当【杀】使用或打出"},
			},
		},
		{
			"id":      "2",
			"name":    "张飞",
			"faction": "蜀",
			"role":    "控制",
			"hp":      4,
			"rarity":  "史诗",
			"skills": []gin.H{
				{"id": "paoxiao", "name": "咆哮", "description": "锁定技，你使用【杀】无次数限制"},
			},
		},
		{
			"id":      "3",
			"name":    "诸葛亮",
			"faction": "蜀",
			"role":    "辅助",
			"hp":      3,
			"rarity":  "传说",
			"skills": []gin.H{
				{"id": "guanxing", "name": "观星", "description": "准备阶段，你可以观看牌堆顶的X张牌"},
				{"id": "kongcheng", "name": "空城", "description": "锁定技，当你没有手牌时，你不能成为【杀】或【决斗】的目标"},
			},
		},
		{
			"id":      "4",
			"name":    "曹操",
			"faction": "魏",
			"role":    "输出",
			"hp":      4,
			"rarity":  "传说",
			"skills": []gin.H{
				{"id": "jianxiong", "name": "奸雄", "description": "当你受到伤害后，你可以获得造成伤害的牌"},
			},
		},
		{
			"id":      "5",
			"name":    "司马懿",
			"faction": "魏",
			"role":    "控制",
			"hp":      3,
			"rarity":  "传说",
			"skills": []gin.H{
				{"id": "fankui", "name": "反馈", "description": "当你受到伤害后，你可以获得伤害来源的一张牌"},
				{"id": "guicai", "name": "鬼才", "description": "当一名角色判定时，你可以打出一张手牌代替判定牌"},
			},
		},
		{
			"id":      "6",
			"name":    "周瑜",
			"faction": "吴",
			"role":    "输出",
			"hp":      3,
			"rarity":  "传说",
			"skills": []gin.H{
				{"id": "yingzi", "name": "英姿", "description": "摸牌阶段，你可以多摸一张牌"},
				{"id": "fanjian", "name": "反间", "description": "出牌阶段限一次，你可以令一名其他角色选择一种花色，然后获得你的一张手牌"},
			},
		},
	}

	c.JSON(http.StatusOK, gin.H{
		"generals": generals,
		"total":    len(generals),
	})
}

// GetTerrains 获取地形列表
func (h *DataHandler) GetTerrains(c *gin.Context) {
	// TODO: 从数据库查询地形数据
	terrains := []gin.H{
		{
			"id":       "1",
			"name":     "山地",
			"type":     "基础",
			"category": "山地",
			"rarity":   "普通",
			"effects": []gin.H{
				{"type": "range_bonus", "value": 1, "description": "远程攻击范围+1"},
			},
		},
		{
			"id":       "2",
			"name":     "平原",
			"type":     "基础",
			"category": "平原",
			"rarity":   "普通",
			"effects": []gin.H{
				{"type": "movement_bonus", "value": 1, "description": "移动力+1"},
			},
		},
		{
			"id":       "3",
			"name":     "森林",
			"type":     "基础",
			"category": "森林",
			"rarity":   "普通",
			"effects": []gin.H{
				{"type": "fire_amplify", "value": 1, "description": "火焰伤害+1"},
			},
		},
		{
			"id":       "4",
			"name":     "河流",
			"type":     "基础",
			"category": "河流",
			"rarity":   "普通",
			"effects": []gin.H{
				{"type": "movement_limit", "value": -1, "description": "移动需额外消耗1行动点"},
			},
		},
		{
			"id":       "5",
			"name":     "祭坛",
			"type":     "事件",
			"category": "特殊",
			"rarity":   "稀有",
			"effects": []gin.H{
				{"type": "revive", "description": "可消耗资源复活阵亡武将"},
			},
		},
		{
			"id":       "6",
			"name":     "烽火台",
			"type":     "交互",
			"category": "特殊",
			"rarity":   "稀有",
			"effects": []gin.H{
				{"type": "fire_range", "description": "点燃后对直线范围造成火焰伤害"},
			},
		},
		{
			"id":       "7",
			"name":     "血宴祭坛",
			"type":     "事件",
			"category": "特殊",
			"rarity":   "史诗",
			"effects": []gin.H{
				{"type": "aoe_damage", "value": 1, "description": "触发全体判定，失败者受到1点伤害"},
			},
		},
	}

	c.JSON(http.StatusOK, gin.H{
		"terrains": terrains,
		"total":    len(terrains),
	})
}

// GetSkills 获取技能列表
func (h *DataHandler) GetSkills(c *gin.Context) {
	// TODO: 从数据库查询技能数据
	skills := []gin.H{
		{
			"id":          "wusheng",
			"name":        "武圣",
			"type":        "主动",
			"description": "你可以将一张红色牌当【杀】使用或打出",
			"cooldown":    0,
		},
		{
			"id":          "guanxing",
			"name":        "观星",
			"type":        "主动",
			"description": "准备阶段，你可以观看牌堆顶的X张牌，然后以任意顺序放回",
			"cooldown":    0,
		},
		{
			"id":          "huogong",
			"name":        "火攻地形联动",
			"type":        "地形技",
			"description": "在森林地形中使用火攻时，可连锁点燃相邻地形",
			"cooldown":    1,
		},
	}

	c.JSON(http.StatusOK, gin.H{
		"skills": skills,
		"total":  len(skills),
	})
}
