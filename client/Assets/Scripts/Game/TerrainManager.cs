using UnityEngine;
using System.Collections.Generic;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 地形管理器 - 管理动态地形系统、地形生成和地形效果
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        [Header("Terrain Settings")]
        [SerializeField] private bool enableDynamicTerrain = true;
        [SerializeField] private int maxTerrainPerRound = 3;
        [SerializeField] private float terrainDuration = 3f; // 地形持续回合数
        
        [Header("Terrain Prefabs")]
        [SerializeField] private GameObject[] terrainPrefabs;
        
        [Header("Terrain Packs")]
        [SerializeField] private TerrainPack currentTerrainPack;
        [SerializeField] private List<TerrainPack> availableTerrainPacks;
        
        private BoardManager boardManager;
        private Dictionary<Vector2Int, TerrainEffect> activeTerrains = new Dictionary<Vector2Int, TerrainEffect>();
        private Dictionary<Vector2Int, int> terrainDurations = new Dictionary<Vector2Int, int>();
        
        public static TerrainManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            boardManager = FindObjectOfType<BoardManager>();
            
            // 加载默认地形包
            LoadTerrainPack(currentTerrainPack);
        }
        
        /// <summary>
        /// 加载地形包
        /// </summary>
        public void LoadTerrainPack(TerrainPack pack)
        {
            if (pack == null)
            {
                Debug.LogWarning("地形包为空");
                return;
            }
            
            currentTerrainPack = pack;
            Debug.Log($"加载地形包: {pack.packName}");
            
            // 根据地形包设置初始地形
            if (pack.initialTerrains != null && pack.initialTerrains.Count > 0)
            {
                foreach (var terrainPlacement in pack.initialTerrains)
                {
                    PlaceTerrain(terrainPlacement.position, terrainPlacement.terrainEffect);
                }
            }
        }
        
        /// <summary>
        /// 放置地形
        /// </summary>
        public bool PlaceTerrain(Vector2Int position, TerrainEffect terrain, int duration = -1)
        {
            if (boardManager == null)
            {
                Debug.LogWarning("BoardManager未找到");
                return false;
            }
            
            TileController tile = boardManager.GetTileAt(position);
            if (tile == null)
            {
                Debug.LogWarning($"位置 {position} 没有瓦片");
                return false;
            }
            
            // 设置地形效果
            tile.SetTerrain(terrain);
            activeTerrains[position] = terrain;
            
            // 设置持续时间
            if (duration < 0)
                duration = Mathf.RoundToInt(terrainDuration);
            terrainDurations[position] = duration;
            
            Debug.Log($"在 {position} 放置地形: {terrain.terrainType}，持续 {duration} 回合");
            
            // 触发地形放置事件
            OnTerrainPlaced(position, terrain);
            
            return true;
        }
        
        /// <summary>
        /// 移除地形
        /// </summary>
        public void RemoveTerrain(Vector2Int position)
        {
            if (!activeTerrains.ContainsKey(position))
                return;
            
            if (boardManager == null)
                return;
            
            TileController tile = boardManager.GetTileAt(position);
            if (tile != null)
            {
                tile.SetTerrain(null);
            }
            
            TerrainEffect removedTerrain = activeTerrains[position];
            activeTerrains.Remove(position);
            terrainDurations.Remove(position);
            
            Debug.Log($"移除 {position} 的地形: {removedTerrain.terrainType}");
            
            // 触发地形移除事件
            OnTerrainRemoved(position, removedTerrain);
        }
        
        /// <summary>
        /// 回合结束时更新地形
        /// </summary>
        public void OnRoundEnd()
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();
            
            // 更新地形持续时间
            foreach (var kvp in terrainDurations)
            {
                terrainDurations[kvp.Key]--;
                
                if (terrainDurations[kvp.Key] <= 0)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            
            // 移除过期地形
            foreach (var pos in toRemove)
            {
                RemoveTerrain(pos);
            }
            
            // 动态生成新地形
            if (enableDynamicTerrain && currentTerrainPack != null)
            {
                GenerateRandomTerrains();
            }
        }
        
        /// <summary>
        /// 随机生成地形
        /// </summary>
        private void GenerateRandomTerrains()
        {
            if (currentTerrainPack == null || currentTerrainPack.randomTerrains == null)
                return;
            
            int count = Random.Range(1, maxTerrainPerRound + 1);
            
            for (int i = 0; i < count; i++)
            {
                // 随机选择地形类型
                TerrainEffect terrain = currentTerrainPack.randomTerrains[Random.Range(0, currentTerrainPack.randomTerrains.Count)];
                
                // 随机选择位置
                Vector2Int randomPos = GetRandomEmptyPosition();
                if (randomPos != Vector2Int.zero)
                {
                    PlaceTerrain(randomPos, terrain);
                }
            }
        }
        
        /// <summary>
        /// 获取随机空位置
        /// </summary>
        private Vector2Int GetRandomEmptyPosition()
        {
            if (boardManager == null) return Vector2Int.zero;
            
            // TODO: 从BoardManager获取所有空瓦片
            // 这里简单返回随机位置
            int x = Random.Range(0, 12);
            int y = Random.Range(0, 12);
            return new Vector2Int(x, y);
        }
        
        /// <summary>
        /// 获取指定位置的地形效果
        /// </summary>
        public TerrainEffect GetTerrainAt(Vector2Int position)
        {
            if (activeTerrains.ContainsKey(position))
                return activeTerrains[position];
            return null;
        }
        
        /// <summary>
        /// 检查是否有地形
        /// </summary>
        public bool HasTerrainAt(Vector2Int position)
        {
            return activeTerrains.ContainsKey(position);
        }
        
        /// <summary>
        /// 地形放置事件
        /// </summary>
        private void OnTerrainPlaced(Vector2Int position, TerrainEffect terrain)
        {
            // TODO: 通知其他系统
            // 例如：检查武将是否在此位置，触发地形效果
        }
        
        /// <summary>
        /// 地形移除事件
        /// </summary>
        private void OnTerrainRemoved(Vector2Int position, TerrainEffect terrain)
        {
            // TODO: 清理地形效果
        }
        
        /// <summary>
        /// 应用地形效果到武将
        /// </summary>
        public void ApplyTerrainEffects(Vector2Int position, GeneralController general)
        {
            TerrainEffect terrain = GetTerrainAt(position);
            if (terrain == null) return;
            
            // 应用伤害
            if (terrain.damagePerTurn > 0)
            {
                Debug.Log($"{general.name} 受到地形伤害: {terrain.damagePerTurn}");
                // general.TakeDamage(terrain.damagePerTurn);
            }
            
            // 应用增益
            if (terrain.attackBonus != 0 || terrain.defenseBonus != 0 || terrain.rangeBonus != 0)
            {
                Debug.Log($"{general.name} 获得地形增益");
                // general.ApplyBonus(terrain);
            }
        }
    }
    
    /// <summary>
    /// 地形包 - 定义一套地形规则
    /// </summary>
    [System.Serializable]
    public class TerrainPack
    {
        public string packName;
        public string description;
        public List<TerrainPlacement> initialTerrains;
        public List<TerrainEffect> randomTerrains;
    }
    
    /// <summary>
    /// 地形放置
    /// </summary>
    [System.Serializable]
    public class TerrainPlacement
    {
        public Vector2Int position;
        public TerrainEffect terrainEffect;
    }
}

