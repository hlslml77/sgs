using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 地形编辑器控制器 - 允许玩家自定义地形规则和地形包
    /// </summary>
    public class TerrainEditorController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField packNameInput;
        [SerializeField] private TMP_InputField packDescriptionInput;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button backButton;
        
        [Header("Terrain Selection")]
        [SerializeField] private Transform terrainPaletteContainer;
        [SerializeField] private GameObject terrainPaletteItemPrefab;
        [SerializeField] private TMP_Dropdown terrainTypeDropdown;
        
        [Header("Board")]
        [SerializeField] private Transform boardContainer;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private int boardWidth = 12;
        [SerializeField] private int boardHeight = 12;
        
        [Header("Terrain Effects Panel")]
        [SerializeField] private GameObject terrainEffectPanel;
        [SerializeField] private TMP_InputField terrainNameInput;
        [SerializeField] private TMP_InputField terrainDescriptionInput;
        [SerializeField] private TMP_InputField damageInput;
        [SerializeField] private TMP_InputField attackBonusInput;
        [SerializeField] private TMP_InputField defenseBonusInput;
        [SerializeField] private Toggle blockMovementToggle;
        [SerializeField] private Button confirmTerrainButton;
        
        private Dictionary<Vector2Int, Game.TerrainEffect> placedTerrains = new Dictionary<Vector2Int, Game.TerrainEffect>();
        private Game.TerrainType selectedTerrainType = Game.TerrainType.None;
        private GameObject[,] boardTiles;
        
        private void Start()
        {
            InitializeButtons();
            InitializeBoard();
            InitializeTerrainPalette();
            
            if (terrainEffectPanel != null)
                terrainEffectPanel.SetActive(false);
        }
        
        private void InitializeButtons()
        {
            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveTerrainPack);
                
            if (loadButton != null)
                loadButton.onClick.AddListener(OnLoadTerrainPack);
                
            if (clearButton != null)
                clearButton.onClick.AddListener(OnClearBoard);
                
            if (backButton != null)
                backButton.onClick.AddListener(OnBack);
                
            if (confirmTerrainButton != null)
                confirmTerrainButton.onClick.AddListener(OnConfirmTerrainEffect);
        }
        
        /// <summary>
        /// 初始化编辑器棋盘
        /// </summary>
        private void InitializeBoard()
        {
            if (boardContainer == null || tilePrefab == null) return;
            
            boardTiles = new GameObject[boardWidth, boardHeight];
            
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 position = new Vector3(x, 0, y);
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, boardContainer);
                    tile.name = $"Tile_{x}_{y}";
                    
                    boardTiles[x, y] = tile;
                    
                    // 添加点击事件
                    int capturedX = x;
                    int capturedY = y;
                    Button button = tile.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnTileClicked(new Vector2Int(capturedX, capturedY)));
                    }
                }
            }
        }
        
        /// <summary>
        /// 初始化地形调色板
        /// </summary>
        private void InitializeTerrainPalette()
        {
            if (terrainPaletteContainer == null || terrainPaletteItemPrefab == null) return;
            
            // 创建所有地形类型的按钮
            foreach (Game.TerrainType terrainType in System.Enum.GetValues(typeof(Game.TerrainType)))
            {
                if (terrainType == Game.TerrainType.None) continue;
                
                GameObject item = Instantiate(terrainPaletteItemPrefab, terrainPaletteContainer);
                
                TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = GetTerrainTypeName(terrainType);
                
                Button button = item.GetComponent<Button>();
                if (button != null)
                {
                    Game.TerrainType capturedType = terrainType;
                    button.onClick.AddListener(() => OnSelectTerrainType(capturedType));
                }
            }
        }
        
        /// <summary>
        /// 选择地形类型
        /// </summary>
        private void OnSelectTerrainType(Game.TerrainType terrainType)
        {
            selectedTerrainType = terrainType;
            Debug.Log($"选择地形类型: {terrainType}");
        }
        
        /// <summary>
        /// 点击瓦片
        /// </summary>
        private void OnTileClicked(Vector2Int position)
        {
            if (selectedTerrainType == Game.TerrainType.None)
            {
                Debug.LogWarning("请先选择地形类型");
                return;
            }
            
            // 显示地形效果编辑面板
            ShowTerrainEffectPanel(position);
        }
        
        /// <summary>
        /// 显示地形效果编辑面板
        /// </summary>
        private void ShowTerrainEffectPanel(Vector2Int position)
        {
            if (terrainEffectPanel != null)
            {
                terrainEffectPanel.SetActive(true);
                
                // 设置默认值
                if (terrainNameInput != null)
                    terrainNameInput.text = GetTerrainTypeName(selectedTerrainType);
            }
        }
        
        /// <summary>
        /// 确认地形效果
        /// </summary>
        private void OnConfirmTerrainEffect()
        {
            // 创建地形效果
            Game.TerrainEffect effect = new Game.TerrainEffect
            {
                terrainType = selectedTerrainType,
                terrainName = terrainNameInput != null ? terrainNameInput.text : "",
                description = terrainDescriptionInput != null ? terrainDescriptionInput.text : "",
                damagePerTurn = ParseInt(damageInput),
                attackBonus = ParseInt(attackBonusInput),
                defenseBonus = ParseInt(defenseBonusInput),
                blocksMovement = blockMovementToggle != null && blockMovementToggle.isOn
            };
            
            // TODO: 应用到选中的瓦片
            
            if (terrainEffectPanel != null)
                terrainEffectPanel.SetActive(false);
        }
        
        /// <summary>
        /// 保存地形包
        /// </summary>
        private void OnSaveTerrainPack()
        {
            string packName = packNameInput != null ? packNameInput.text : "自定义地形包";
            string description = packDescriptionInput != null ? packDescriptionInput.text : "";
            
            Debug.Log($"保存地形包: {packName}");
            
            // TODO: 序列化地形包数据并保存到文件
            Game.TerrainPack pack = new Game.TerrainPack
            {
                packName = packName,
                description = description,
                initialTerrains = new List<Game.TerrainPlacement>()
            };
            
            foreach (var kvp in placedTerrains)
            {
                pack.initialTerrains.Add(new Game.TerrainPlacement
                {
                    position = kvp.Key,
                    terrainEffect = kvp.Value
                });
            }
            
            // 保存到Resources或PlayerPrefs
            string json = JsonUtility.ToJson(pack);
            PlayerPrefs.SetString($"TerrainPack_{packName}", json);
            PlayerPrefs.Save();
            
            Debug.Log("地形包保存成功！");
        }
        
        /// <summary>
        /// 加载地形包
        /// </summary>
        private void OnLoadTerrainPack()
        {
            Debug.Log("加载地形包");
            // TODO: 显示地形包列表供选择
        }
        
        /// <summary>
        /// 清空棋盘
        /// </summary>
        private void OnClearBoard()
        {
            placedTerrains.Clear();
            
            // 重置所有瓦片
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (boardTiles[x, y] != null)
                    {
                        // 重置瓦片外观
                    }
                }
            }
            
            Debug.Log("棋盘已清空");
        }
        
        /// <summary>
        /// 返回
        /// </summary>
        private void OnBack()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// 获取地形类型名称
        /// </summary>
        private string GetTerrainTypeName(Game.TerrainType terrainType)
        {
            switch (terrainType)
            {
                case Game.TerrainType.Fire: return "火焰";
                case Game.TerrainType.Ice: return "冰霜";
                case Game.TerrainType.Poison: return "毒沼";
                case Game.TerrainType.Fortress: return "堡垒";
                case Game.TerrainType.Altar: return "祭坛";
                case Game.TerrainType.Trap: return "陷阱";
                case Game.TerrainType.Heal: return "医庐";
                default: return "未知";
            }
        }
        
        /// <summary>
        /// 解析整数输入
        /// </summary>
        private int ParseInt(TMP_InputField input)
        {
            if (input == null || string.IsNullOrEmpty(input.text))
                return 0;
            
            int result;
            if (int.TryParse(input.text, out result))
                return result;
            
            return 0;
        }
    }
}

