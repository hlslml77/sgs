package game

import (
	"github.com/google/uuid"
	"github.com/sanguo-strategy/server/internal/models"
	"github.com/sanguo-strategy/server/pkg/logger"
)

// GameLogic 游戏逻辑处理器
type GameLogic struct {
	state *models.GameState
}

// NewGameLogic 创建游戏逻辑处理器
func NewGameLogic(state *models.GameState) *GameLogic {
	return &GameLogic{state: state}
}

// ProcessTurn 处理回合
func (gl *GameLogic) ProcessTurn() {
	logger.Debug("Processing turn %d", gl.state.Round)

	// 回合阶段流程
	switch gl.state.CurrentPhase {
	case models.PhasePrepare:
		gl.processPreparePhase()
	case models.PhaseDeploy:
		gl.processDeployPhase()
	case models.PhaseCombat:
		gl.processCombatPhase()
	case models.PhaseEndRound:
		gl.processEndRoundPhase()
	}
}

// processPreparePhase 准备阶段
func (gl *GameLogic) processPreparePhase() {
	logger.Debug("Prepare phase - Round %d", gl.state.Round)

	// 1. 地形效果生效
	gl.applyTerrainEffects()

	// 2. 回复行动点
	for _, player := range gl.state.Players {
		player.ActionPoints = 3 // 默认 3 行动点
	}

	// 3. 检查羁绊
	gl.checkSynergies()

	// 进入部署阶段
	gl.state.CurrentPhase = models.PhaseDeploy
}

// processDeployPhase 部署阶段
func (gl *GameLogic) processDeployPhase() {
	logger.Debug("Deploy phase - Round %d", gl.state.Round)

	// 玩家可以部署地形、移动武将等
	// 这个阶段通过 WebSocket 接收玩家操作
	// 当所有玩家完成部署后，进入战斗阶段
}

// processCombatPhase 战斗阶段
func (gl *GameLogic) processCombatPhase() {
	logger.Debug("Combat phase - Round %d", gl.state.Round)

	// 玩家依次进行回合
	// 使用技能、打牌、攻击等
}

// processEndRoundPhase 回合结束阶段
func (gl *GameLogic) processEndRoundPhase() {
	logger.Debug("End round phase - Round %d", gl.state.Round)

	// 1. 地形持续效果结算
	gl.updateTerrains()

	// 2. Buff/Debuff 持续时间递减
	gl.updateBuffs()

	// 3. 检查胜利条件
	if gl.checkVictoryCondition() {
		gl.state.IsFinished = true
		return
	}

	// 4. 进入下一回合
	gl.state.Round++
	gl.state.CurrentPhase = models.PhasePrepare

	logger.Info("Starting round %d", gl.state.Round)
}

// applyTerrainEffects 应用地形效果
func (gl *GameLogic) applyTerrainEffects() {
	for pos, terrain := range gl.state.Terrains {
		if !terrain.IsActivated {
			continue
		}

		logger.Debug("Applying terrain effect: %s at (%d, %d)", terrain.Name, pos.X, pos.Y)

		// 查找在该地形上的武将
		for _, player := range gl.state.Players {
			for _, general := range player.Generals {
				if general.Position.X == pos.X && general.Position.Y == pos.Y {
					gl.applyTerrainEffectToGeneral(terrain, general)
				}
			}
		}
	}
}

// applyTerrainEffectToGeneral 对武将应用地形效果
func (gl *GameLogic) applyTerrainEffectToGeneral(terrain *models.TerrainState, general *models.GeneralState) {
	for _, effect := range terrain.Effects {
		switch effect.Type {
		case "damage":
			general.CurrentHP -= effect.Value
			logger.Debug("General %s takes %d damage from terrain %s", general.Name, effect.Value, terrain.Name)
		case "heal":
			general.CurrentHP += effect.Value
			if general.CurrentHP > general.MaxHP {
				general.CurrentHP = general.MaxHP
			}
			logger.Debug("General %s heals %d HP from terrain %s", general.Name, effect.Value, terrain.Name)
		}
	}
}

// updateTerrains 更新地形状态
func (gl *GameLogic) updateTerrains() {
	for pos, terrain := range gl.state.Terrains {
		if terrain.Duration > 0 {
			terrain.Duration--
			if terrain.Duration == 0 {
				// 地形消失
				delete(gl.state.Terrains, pos)
				logger.Debug("Terrain %s at (%d, %d) expired", terrain.Name, pos.X, pos.Y)
			}
		}
	}
}

// updateBuffs 更新增益/减益效果
func (gl *GameLogic) updateBuffs() {
	for _, player := range gl.state.Players {
		for _, general := range player.Generals {
			newBuffs := make([]*models.Buff, 0)
			for _, buff := range general.Buffs {
				buff.Duration--
				if buff.Duration > 0 {
					newBuffs = append(newBuffs, buff)
				} else {
					logger.Debug("Buff %s expired on general %s", buff.Name, general.Name)
				}
			}
			general.Buffs = newBuffs
		}
	}
}

// checkSynergies 检查羁绊
func (gl *GameLogic) checkSynergies() {
	// TODO: 实现羁绊检查逻辑
	// 根据武将组合、地形等条件激活羁绊效果
	logger.Debug("Checking synergies...")
}

// checkVictoryCondition 检查胜利条件
func (gl *GameLogic) checkVictoryCondition() bool {
	// 检查是否有队伍全灭
	redAlive := false
	blueAlive := false

	for _, player := range gl.state.Players {
		if player.IsAlive {
			if player.Team == "red" {
				redAlive = true
			} else if player.Team == "blue" {
				blueAlive = true
			}
		}
	}

	if !redAlive {
		gl.state.Winner = "blue"
		logger.Info("Blue team wins!")
		return true
	}
	if !blueAlive {
		gl.state.Winner = "red"
		logger.Info("Red team wins!")
		return true
	}

	// 检查是否达到最大回合数
	if gl.state.Round >= 15 {
		// 平局或根据资源/地形占领判断胜负
		logger.Info("Game ends - max rounds reached")
		return true
	}

	return false
}

// PlayCard 打牌
func (gl *GameLogic) PlayCard(playerID, cardID uuid.UUID, targets []uuid.UUID) error {
	// TODO: 实现打牌逻辑
	logger.Debug("Player %s plays card %s", playerID, cardID)
	return nil
}

// UseSkill 使用技能
func (gl *GameLogic) UseSkill(generalID uuid.UUID, skillID string, targets []uuid.UUID) error {
	// TODO: 实现技能使用逻辑
	logger.Debug("General %s uses skill %s", generalID, skillID)
	return nil
}

// MoveGeneral 移动武将
func (gl *GameLogic) MoveGeneral(generalID uuid.UUID, targetPos models.Position) error {
	// TODO: 实现移动逻辑，包括移动消耗、地形限制等
	logger.Debug("Moving general %s to (%d, %d)", generalID, targetPos.X, targetPos.Y)
	return nil
}

// DeployTerrain 部署地形
func (gl *GameLogic) DeployTerrain(playerID uuid.UUID, terrainID uuid.UUID, pos models.Position) error {
	// TODO: 实现地形部署逻辑
	logger.Debug("Player %s deploys terrain %s at (%d, %d)", playerID, terrainID, pos.X, pos.Y)
	return nil
}
