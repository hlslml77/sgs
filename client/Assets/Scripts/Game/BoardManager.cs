using System.Collections.Generic;
using UnityEngine;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 棋盘管理器 - 管理地形和武将的显示
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int boardWidth = 10;
        [SerializeField] private int boardHeight = 10;
        [SerializeField] private float tileSize = 1.0f;

        [Header("Prefabs")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject generalPrefab;
        [SerializeField] private GameObject terrainPrefab;

        [Header("Containers")]
        [SerializeField] private Transform boardContainer;
        [SerializeField] private Transform generalsContainer;
        [SerializeField] private Transform terrainsContainer;

        private GameObject[,] tiles;
        private Dictionary<string, GameObject> generals = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> terrains = new Dictionary<string, GameObject>();

        private GameObject selectedGeneral;
        private TileController selectedTile;

        /// <summary>
        /// 获取指定位置的瓦片
        /// </summary>
        public TileController GetTileAt(Vector2Int position)
        {
            if (position.x < 0 || position.x >= boardWidth || position.y < 0 || position.y >= boardHeight)
                return null;

            if (tiles == null || tiles[position.x, position.y] == null)
                return null;

            return tiles[position.x, position.y].GetComponent<TileController>();
        }

        /// <summary>
        /// 处理瓦片点击事件
        /// </summary>
        public void OnTileClicked(TileController tile)
        {
            Debug.Log($"瓦片被点击: {tile.GridPosition}");

            if (selectedTile != null)
            {
                selectedTile.SetSelected(false);
            }

            selectedTile = tile;
            tile.SetSelected(true);

            // 如果有选中的武将，移动到点击的瓦片
            if (selectedGeneral != null)
            {
                var controller = selectedGeneral.GetComponent<GeneralController>();
                if (controller != null)
                {
                    GameManager.Instance.MoveGeneral(controller.GeneralId, tile.GridPosition);
                    DeselectGeneral();
                }
            }
        }

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        public void InitializeBoard(GameState gameState)
        {
            ClearBoard();
            CreateTiles();
            SpawnGenerals(gameState.Players);
            SpawnTerrains(gameState.Terrains);
        }

        /// <summary>
        /// 创建棋盘格子
        /// </summary>
        private void CreateTiles()
        {
            tiles = new GameObject[boardWidth, boardHeight];

            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, boardContainer);
                    tile.name = $"Tile_{x}_{y}";
                    tiles[x, y] = tile;

                    // 初始化TileController
                    var tileController = tile.GetComponent<TileController>();
                    if (tileController == null)
                        tileController = tile.AddComponent<TileController>();
                    
                    tileController.Initialize(new Vector2Int(x, y), TileType.Plain);
                }
            }
        }

        /// <summary>
        /// 生成武将
        /// </summary>
        private void SpawnGenerals(List<PlayerState> players)
        {
            foreach (var player in players)
            {
                foreach (var general in player.Generals)
                {
                    if (general.IsAlive)
                    {
                        SpawnGeneral(general);
                    }
                }
            }
        }

        private void SpawnGeneral(GeneralState general)
        {
            Vector3 position = new Vector3(
                general.Position.x * tileSize,
                0.5f,
                general.Position.y * tileSize
            );

            GameObject generalObj = Instantiate(generalPrefab, position, Quaternion.identity, generalsContainer);
            generalObj.name = general.Name;

            var generalController = generalObj.GetComponent<GeneralController>();
            if (generalController != null)
            {
                generalController.Initialize(general);
                generalController.OnGeneralClicked += HandleGeneralClicked;
            }

            generals[general.GeneralId] = generalObj;
        }

        /// <summary>
        /// 生成地形
        /// </summary>
        private void SpawnTerrains(Dictionary<string, TerrainState> terrainStates)
        {
            if (terrainStates == null) return;

            foreach (var kvp in terrainStates)
            {
                SpawnTerrain(kvp.Value);
            }
        }

        private void SpawnTerrain(TerrainState terrain)
        {
            Vector3 position = new Vector3(
                terrain.Position.x * tileSize,
                0.1f,
                terrain.Position.y * tileSize
            );

            GameObject terrainObj = Instantiate(terrainPrefab, position, Quaternion.identity, terrainsContainer);
            terrainObj.name = terrain.Name;

            var terrainController = terrainObj.GetComponent<TerrainController>();
            if (terrainController != null)
            {
                terrainController.Initialize(terrain);
            }

            terrains[terrain.TerrainId] = terrainObj;
        }

        /// <summary>
        /// 更新棋盘
        /// </summary>
        public void UpdateBoard(GameState gameState)
        {
            UpdateGenerals(gameState.Players);
            UpdateTerrains(gameState.Terrains);
        }

        private void UpdateGenerals(List<PlayerState> players)
        {
            foreach (var player in players)
            {
                foreach (var general in player.Generals)
                {
                    if (generals.ContainsKey(general.GeneralId))
                    {
                        var generalObj = generals[general.GeneralId];
                        var controller = generalObj.GetComponent<GeneralController>();
                        controller?.UpdateState(general);

                        // 更新位置
                        Vector3 targetPos = new Vector3(
                            general.Position.x * tileSize,
                            0.5f,
                            general.Position.y * tileSize
                        );
                        generalObj.transform.position = Vector3.Lerp(
                            generalObj.transform.position,
                            targetPos,
                            Time.deltaTime * 5f
                        );
                    }
                    else if (general.IsAlive)
                    {
                        SpawnGeneral(general);
                    }
                }
            }
        }

        private void UpdateTerrains(Dictionary<string, TerrainState> terrainStates)
        {
            // TODO: 实现地形更新逻辑
        }

        /// <summary>
        /// 处理武将点击
        /// </summary>
        private void HandleGeneralClicked(GameObject general)
        {
            Debug.Log($"General clicked: {general.name}");
            
            if (selectedGeneral == general)
            {
                DeselectGeneral();
            }
            else
            {
                SelectGeneral(general);
            }
        }

        private void SelectGeneral(GameObject general)
        {
            DeselectGeneral();
            selectedGeneral = general;
            
            // 显示选中效果
            var controller = general.GetComponent<GeneralController>();
            controller?.ShowSelection(true);
        }

        private void DeselectGeneral()
        {
            if (selectedGeneral != null)
            {
                var controller = selectedGeneral.GetComponent<GeneralController>();
                controller?.ShowSelection(false);
                selectedGeneral = null;
            }
        }

        /// <summary>
        /// 清空棋盘
        /// </summary>
        private void ClearBoard()
        {
            foreach (var general in generals.Values)
            {
                if (general != null) Destroy(general);
            }
            generals.Clear();

            foreach (var terrain in terrains.Values)
            {
                if (terrain != null) Destroy(terrain);
            }
            terrains.Clear();

            if (tiles != null)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    for (int y = 0; y < boardHeight; y++)
                    {
                        if (tiles[x, y] != null) Destroy(tiles[x, y]);
                    }
                }
            }
        }
    }
}

