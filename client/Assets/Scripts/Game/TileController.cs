using UnityEngine;
using UnityEngine.EventSystems;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 地形瓦片控制器 - 处理单个地形格子的逻辑和交互
    /// </summary>
    public class TileController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Tile Data")]
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private TileType tileType = TileType.Plain;
        [SerializeField] private TerrainEffect currentTerrain;
        
        [Header("Visual")]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private GameObject highlightEffect;
        [SerializeField] private GameObject terrainEffectPrefab;
        
        [Header("Occupancy")]
        [SerializeField] private bool isOccupied = false;
        [SerializeField] private GameObject occupyingUnit;
        
        private GameObject currentTerrainEffect;
        private bool isSelected = false;
        
        public Vector2Int GridPosition => gridPosition;
        public TileType Type => tileType;
        public TerrainEffect Terrain => currentTerrain;
        public bool IsOccupied => isOccupied;
        public GameObject OccupyingUnit => occupyingUnit;
        
        private void Awake()
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
        }
        
        /// <summary>
        /// 初始化瓦片
        /// </summary>
        public void Initialize(Vector2Int position, TileType type)
        {
            gridPosition = position;
            tileType = type;
            UpdateVisual();
        }
        
        /// <summary>
        /// 设置地形效果
        /// </summary>
        public void SetTerrain(TerrainEffect terrain)
        {
            currentTerrain = terrain;
            
            // 清除旧的地形效果
            if (currentTerrainEffect != null)
                Destroy(currentTerrainEffect);
            
            // 创建新的地形效果
            if (terrain != null && terrainEffectPrefab != null)
            {
                currentTerrainEffect = Instantiate(terrainEffectPrefab, transform);
                // TODO: 根据地形类型配置效果
            }
            
            UpdateVisual();
        }
        
        /// <summary>
        /// 设置占用状态
        /// </summary>
        public void SetOccupied(GameObject unit)
        {
            isOccupied = unit != null;
            occupyingUnit = unit;
        }
        
        /// <summary>
        /// 更新视觉效果
        /// </summary>
        private void UpdateVisual()
        {
            if (meshRenderer == null) return;
            
            // 根据地形类型改变颜色
            Color tileColor = GetTileColor();
            if (meshRenderer.material != null)
            {
                meshRenderer.material.color = tileColor;
            }
        }
        
        /// <summary>
        /// 获取瓦片颜色
        /// </summary>
        private Color GetTileColor()
        {
            if (currentTerrain != null)
            {
                return GetTerrainColor(currentTerrain.terrainType);
            }
            
            switch (tileType)
            {
                case TileType.Plain:
                    return new Color(0.8f, 0.8f, 0.6f);
                case TileType.Mountain:
                    return new Color(0.5f, 0.5f, 0.5f);
                case TileType.Water:
                    return new Color(0.3f, 0.5f, 0.8f);
                case TileType.Forest:
                    return new Color(0.3f, 0.6f, 0.3f);
                default:
                    return Color.white;
            }
        }
        
        /// <summary>
        /// 获取地形效果颜色
        /// </summary>
        private Color GetTerrainColor(TerrainType terrainType)
        {
            switch (terrainType)
            {
                case TerrainType.Fire:
                    return new Color(1f, 0.5f, 0f);
                case TerrainType.Ice:
                    return new Color(0.5f, 0.8f, 1f);
                case TerrainType.Poison:
                    return new Color(0.5f, 0f, 0.8f);
                case TerrainType.Fortress:
                    return new Color(0.7f, 0.7f, 0.7f);
                case TerrainType.Altar:
                    return new Color(0.8f, 0.2f, 0.2f);
                default:
                    return Color.white;
            }
        }
        
        /// <summary>
        /// 鼠标进入
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isSelected && hoverMaterial != null && meshRenderer != null)
            {
                // meshRenderer.material = hoverMaterial;
                meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, Color.white, 0.3f);
            }
            
            if (highlightEffect != null)
                highlightEffect.SetActive(true);
                
            // 显示提示信息
            ShowTooltip();
        }
        
        /// <summary>
        /// 鼠标离开
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isSelected)
            {
                UpdateVisual();
            }
            
            if (highlightEffect != null)
                highlightEffect.SetActive(false);
                
            HideTooltip();
        }
        
        /// <summary>
        /// 点击瓦片
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"点击瓦片: {gridPosition}, 类型: {tileType}, 地形: {currentTerrain?.terrainType}");
            
            // 通知BoardManager
            BoardManager boardManager = FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                boardManager.OnTileClicked(this);
            }
            
            SetSelected(!isSelected);
        }
        
        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (selected && selectedMaterial != null && meshRenderer != null)
            {
                // meshRenderer.material = selectedMaterial;
                meshRenderer.material.color = Color.yellow;
            }
            else
            {
                UpdateVisual();
            }
        }
        
        /// <summary>
        /// 显示提示信息
        /// </summary>
        private void ShowTooltip()
        {
            // TODO: 显示瓦片信息UI
            string info = $"位置: ({gridPosition.x}, {gridPosition.y})\n";
            info += $"类型: {tileType}\n";
            if (currentTerrain != null)
            {
                info += $"地形: {currentTerrain.terrainType}\n";
                info += $"效果: {currentTerrain.description}";
            }
            
            Debug.Log(info);
        }
        
        /// <summary>
        /// 隐藏提示信息
        /// </summary>
        private void HideTooltip()
        {
            // TODO: 隐藏提示UI
        }
        
        /// <summary>
        /// 检查是否可通行
        /// </summary>
        public bool IsWalkable()
        {
            if (isOccupied) return false;
            if (tileType == TileType.Mountain) return false;
            if (currentTerrain != null && currentTerrain.blocksMovement) return false;
            
            return true;
        }
    }
    
    /// <summary>
    /// 瓦片类型
    /// </summary>
    public enum TileType
    {
        Plain,      // 平原
        Mountain,   // 山地
        Water,      // 水域
        Forest      // 森林
    }
    
    /// <summary>
    /// 地形效果
    /// </summary>
    [System.Serializable]
    public class TerrainEffect
    {
        public TerrainType terrainType;
        public string terrainName;
        public string description;
        public bool blocksMovement;
        public int damagePerTurn;
        public int attackBonus;
        public int defenseBonus;
        public int rangeBonus;
    }
    
    /// <summary>
    /// 地形类型（动态地形）
    /// </summary>
    public enum TerrainType
    {
        None,       // 无
        Fire,       // 火焰
        Ice,        // 冰霜
        Poison,     // 毒沼
        Fortress,   // 堡垒
        Altar,      // 祭坛
        Trap,       // 陷阱
        Heal        // 医庐
    }
}

