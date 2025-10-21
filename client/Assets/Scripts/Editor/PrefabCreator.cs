using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// 预制件创建工具
    /// </summary>
    public class PrefabCreator : EditorWindow
    {
        [MenuItem("三国策略/创建预制件")]
        public static void ShowWindow()
        {
            GetWindow<PrefabCreator>("预制件创建工具");
        }

        private void OnGUI()
        {
            GUILayout.Label("预制件创建工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("创建武将卡片预制件", GUILayout.Height(30)))
            {
                CreateHeroCardPrefab();
            }

            if (GUILayout.Button("创建房间列表项预制件", GUILayout.Height(30)))
            {
                CreateRoomListItemPrefab();
            }

            if (GUILayout.Button("创建卡牌预制件", GUILayout.Height(30)))
            {
                CreateCardPrefab();
            }

            if (GUILayout.Button("创建地形块预制件", GUILayout.Height(30)))
            {
                CreateTerrainTilePrefab();
            }

            if (GUILayout.Button("创建技能按钮预制件", GUILayout.Height(30)))
            {
                CreateSkillButtonPrefab();
            }
        }

        /// <summary>
        /// 创建武将卡片预制件
        /// </summary>
        private void CreateHeroCardPrefab()
        {
            // 创建根对象
            GameObject heroCard = new GameObject("HeroCard");
            
            RectTransform rt = heroCard.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 280);
            
            Image bg = heroCard.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f, 1f);
            
            Button button = heroCard.AddComponent<Button>();

            // 创建职能颜色条
            GameObject roleBar = new GameObject("RoleBar");
            roleBar.transform.SetParent(heroCard.transform);
            
            RectTransform roleBarRt = roleBar.AddComponent<RectTransform>();
            roleBarRt.anchorMin = new Vector2(0, 1);
            roleBarRt.anchorMax = new Vector2(1, 1);
            roleBarRt.sizeDelta = new Vector2(0, 10);
            roleBarRt.anchoredPosition = new Vector2(0, -5);
            
            Image roleBarImg = roleBar.AddComponent<Image>();
            roleBarImg.color = Color.red; // 默认红色，运行时根据职能修改

            // 创建武将头像区域
            GameObject avatarArea = new GameObject("AvatarArea");
            avatarArea.transform.SetParent(heroCard.transform);
            
            RectTransform avatarRt = avatarArea.AddComponent<RectTransform>();
            avatarRt.anchorMin = new Vector2(0.5f, 1);
            avatarRt.anchorMax = new Vector2(0.5f, 1);
            avatarRt.sizeDelta = new Vector2(180, 180);
            avatarRt.anchoredPosition = new Vector2(0, -100);
            
            Image avatarImg = avatarArea.AddComponent<Image>();
            avatarImg.color = new Color(0.3f, 0.3f, 0.35f, 1f);

            // 创建武将名称
            GameObject nameObj = new GameObject("HeroName");
            nameObj.transform.SetParent(heroCard.transform);
            
            RectTransform nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0.5f, 0);
            nameRt.anchorMax = new Vector2(0.5f, 0);
            nameRt.sizeDelta = new Vector2(180, 40);
            nameRt.anchoredPosition = new Vector2(0, 50);
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "武将名称";
            nameText.fontSize = 24;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;
            nameText.fontStyle = FontStyles.Bold;

            // 创建势力标识
            GameObject factionObj = new GameObject("Faction");
            factionObj.transform.SetParent(heroCard.transform);
            
            RectTransform factionRt = factionObj.AddComponent<RectTransform>();
            factionRt.anchorMin = new Vector2(0.5f, 0);
            factionRt.anchorMax = new Vector2(0.5f, 0);
            factionRt.sizeDelta = new Vector2(180, 30);
            factionRt.anchoredPosition = new Vector2(0, 20);
            
            TextMeshProUGUI factionText = factionObj.AddComponent<TextMeshProUGUI>();
            factionText.text = "势力";
            factionText.fontSize = 18;
            factionText.alignment = TextAlignmentOptions.Center;
            factionText.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            // 保存预制件
            SavePrefab(heroCard, "Assets/Prefabs/UI/HeroCard.prefab");
            
            EditorUtility.DisplayDialog("完成", "武将卡片预制件已创建！", "确定");
        }

        /// <summary>
        /// 创建房间列表项预制件
        /// </summary>
        private void CreateRoomListItemPrefab()
        {
            GameObject roomItem = new GameObject("RoomListItem");
            
            RectTransform rt = roomItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1360, 80);
            
            Image bg = roomItem.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f, 1f);

            // 房间名称
            GameObject nameObj = new GameObject("RoomName");
            nameObj.transform.SetParent(roomItem.transform);
            
            RectTransform nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0, 0.5f);
            nameRt.anchorMax = new Vector2(0, 0.5f);
            nameRt.sizeDelta = new Vector2(300, 60);
            nameRt.anchoredPosition = new Vector2(170, 0);
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "房间名称";
            nameText.fontSize = 26;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.color = Color.white;
            nameText.fontStyle = FontStyles.Bold;

            // 玩家数
            GameObject playersObj = new GameObject("Players");
            playersObj.transform.SetParent(roomItem.transform);
            
            RectTransform playersRt = playersObj.AddComponent<RectTransform>();
            playersRt.anchorMin = new Vector2(0.5f, 0.5f);
            playersRt.anchorMax = new Vector2(0.5f, 0.5f);
            playersRt.sizeDelta = new Vector2(150, 60);
            playersRt.anchoredPosition = new Vector2(-100, 0);
            
            TextMeshProUGUI playersText = playersObj.AddComponent<TextMeshProUGUI>();
            playersText.text = "2/4人";
            playersText.fontSize = 22;
            playersText.alignment = TextAlignmentOptions.Center;
            playersText.color = new Color(0.7f, 0.9f, 0.7f, 1f);

            // 地形规则
            GameObject terrainObj = new GameObject("TerrainRule");
            terrainObj.transform.SetParent(roomItem.transform);
            
            RectTransform terrainRt = terrainObj.AddComponent<RectTransform>();
            terrainRt.anchorMin = new Vector2(0.5f, 0.5f);
            terrainRt.anchorMax = new Vector2(0.5f, 0.5f);
            terrainRt.sizeDelta = new Vector2(200, 60);
            terrainRt.anchoredPosition = new Vector2(150, 0);
            
            TextMeshProUGUI terrainText = terrainObj.AddComponent<TextMeshProUGUI>();
            terrainText.text = "赤壁水战";
            terrainText.fontSize = 20;
            terrainText.alignment = TextAlignmentOptions.Center;
            terrainText.color = new Color(0.9f, 0.85f, 0.6f, 1f);

            // 加入按钮
            GameObject joinBtn = new GameObject("JoinButton");
            joinBtn.transform.SetParent(roomItem.transform);
            
            RectTransform joinRt = joinBtn.AddComponent<RectTransform>();
            joinRt.anchorMin = new Vector2(1, 0.5f);
            joinRt.anchorMax = new Vector2(1, 0.5f);
            joinRt.sizeDelta = new Vector2(150, 60);
            joinRt.anchoredPosition = new Vector2(-95, 0);
            
            Image joinBg = joinBtn.AddComponent<Image>();
            joinBg.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            
            Button joinButton = joinBtn.AddComponent<Button>();
            joinButton.targetGraphic = joinBg;

            GameObject joinTextObj = new GameObject("Text");
            joinTextObj.transform.SetParent(joinBtn.transform);
            
            RectTransform joinTextRt = joinTextObj.AddComponent<RectTransform>();
            joinTextRt.anchorMin = Vector2.zero;
            joinTextRt.anchorMax = Vector2.one;
            joinTextRt.sizeDelta = Vector2.zero;
            joinTextRt.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI joinText = joinTextObj.AddComponent<TextMeshProUGUI>();
            joinText.text = "加入";
            joinText.fontSize = 24;
            joinText.alignment = TextAlignmentOptions.Center;
            joinText.color = Color.white;
            joinText.fontStyle = FontStyles.Bold;

            SavePrefab(roomItem, "Assets/Prefabs/UI/RoomListItem.prefab");
            
            EditorUtility.DisplayDialog("完成", "房间列表项预制件已创建！", "确定");
        }

        /// <summary>
        /// 创建卡牌预制件
        /// </summary>
        private void CreateCardPrefab()
        {
            GameObject card = new GameObject("Card");
            
            RectTransform rt = card.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(120, 180);
            
            Image bg = card.AddComponent<Image>();
            bg.color = Color.white;
            
            Button button = card.AddComponent<Button>();
            button.targetGraphic = bg;

            // 卡牌名称
            GameObject nameObj = new GameObject("CardName");
            nameObj.transform.SetParent(card.transform);
            
            RectTransform nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0.5f, 1);
            nameRt.anchorMax = new Vector2(0.5f, 1);
            nameRt.sizeDelta = new Vector2(110, 30);
            nameRt.anchoredPosition = new Vector2(0, -20);
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "杀";
            nameText.fontSize = 20;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.black;
            nameText.fontStyle = FontStyles.Bold;

            // 卡牌图案区域
            GameObject iconObj = new GameObject("CardIcon");
            iconObj.transform.SetParent(card.transform);
            
            RectTransform iconRt = iconObj.AddComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0.5f, 0.5f);
            iconRt.anchorMax = new Vector2(0.5f, 0.5f);
            iconRt.sizeDelta = new Vector2(100, 100);
            iconRt.anchoredPosition = new Vector2(0, 5);
            
            Image iconImg = iconObj.AddComponent<Image>();
            iconImg.color = new Color(0.9f, 0.9f, 0.9f, 1f);

            // 卡牌类型
            GameObject typeObj = new GameObject("CardType");
            typeObj.transform.SetParent(card.transform);
            
            RectTransform typeRt = typeObj.AddComponent<RectTransform>();
            typeRt.anchorMin = new Vector2(0.5f, 0);
            typeRt.anchorMax = new Vector2(0.5f, 0);
            typeRt.sizeDelta = new Vector2(110, 25);
            typeRt.anchoredPosition = new Vector2(0, 15);
            
            TextMeshProUGUI typeText = typeObj.AddComponent<TextMeshProUGUI>();
            typeText.text = "基本牌";
            typeText.fontSize = 14;
            typeText.alignment = TextAlignmentOptions.Center;
            typeText.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            SavePrefab(card, "Assets/Prefabs/Game/Card.prefab");
            
            EditorUtility.DisplayDialog("完成", "卡牌预制件已创建！", "确定");
        }

        /// <summary>
        /// 创建地形块预制件
        /// </summary>
        private void CreateTerrainTilePrefab()
        {
            // 创建基础地形块
            GameObject tile = new GameObject("TerrainTile");
            tile.AddComponent<BoxCollider>();
            
            MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();
            
            // 创建一个简单的平面网格
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0, -0.5f),
                new Vector3(0.5f, 0, -0.5f),
                new Vector3(0.5f, 0, 0.5f),
                new Vector3(-0.5f, 0, 0.5f)
            };
            mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
            mesh.normals = new Vector3[]
            {
                Vector3.up, Vector3.up, Vector3.up, Vector3.up
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            
            meshFilter.mesh = mesh;

            // 创建材质
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.5f, 0.7f, 0.5f, 1f); // 默认绿色（平原）
            meshRenderer.material = mat;

            // 添加地形块标识组件（需要创建）
            // var terrainTileComponent = tile.AddComponent<TerrainTile>();

            // 添加UI提示（可选）
            GameObject canvas = new GameObject("TileInfoCanvas");
            canvas.transform.SetParent(tile.transform);
            canvas.transform.localPosition = new Vector3(0, 0.1f, 0);
            canvas.transform.localRotation = Quaternion.Euler(90, 0, 0);
            
            Canvas canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.WorldSpace;
            
            RectTransform canvasRt = canvas.GetComponent<RectTransform>();
            canvasRt.sizeDelta = new Vector2(1, 1);

            GameObject textObj = new GameObject("TileText");
            textObj.transform.SetParent(canvas.transform);
            
            RectTransform textRt = textObj.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;
            textRt.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "平原";
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            SavePrefab(tile, "Assets/Prefabs/Game/TerrainTile.prefab");
            
            EditorUtility.DisplayDialog("完成", "地形块预制件已创建！\n建议手动创建不同类型的变体（山地、森林、河流等）", "确定");
        }

        /// <summary>
        /// 创建技能按钮预制件
        /// </summary>
        private void CreateSkillButtonPrefab()
        {
            GameObject skillBtn = new GameObject("SkillButton");
            
            RectTransform rt = skillBtn.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(280, 80);
            
            Image bg = skillBtn.AddComponent<Image>();
            bg.color = new Color(0.3f, 0.3f, 0.4f, 1f);
            
            Button button = skillBtn.AddComponent<Button>();
            button.targetGraphic = bg;

            // 技能图标
            GameObject iconObj = new GameObject("SkillIcon");
            iconObj.transform.SetParent(skillBtn.transform);
            
            RectTransform iconRt = iconObj.AddComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 0.5f);
            iconRt.anchorMax = new Vector2(0, 0.5f);
            iconRt.sizeDelta = new Vector2(60, 60);
            iconRt.anchoredPosition = new Vector2(40, 0);
            
            Image iconImg = iconObj.AddComponent<Image>();
            iconImg.color = new Color(0.9f, 0.7f, 0.3f, 1f);

            // 技能名称
            GameObject nameObj = new GameObject("SkillName");
            nameObj.transform.SetParent(skillBtn.transform);
            
            RectTransform nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0, 0.5f);
            nameRt.anchorMax = new Vector2(1, 0.5f);
            nameRt.sizeDelta = new Vector2(-90, 30);
            nameRt.anchoredPosition = new Vector2(30, 15);
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "技能名称";
            nameText.fontSize = 20;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.color = Color.white;
            nameText.fontStyle = FontStyles.Bold;

            // 技能描述
            GameObject descObj = new GameObject("SkillDescription");
            descObj.transform.SetParent(skillBtn.transform);
            
            RectTransform descRt = descObj.AddComponent<RectTransform>();
            descRt.anchorMin = new Vector2(0, 0.5f);
            descRt.anchorMax = new Vector2(1, 0.5f);
            descRt.sizeDelta = new Vector2(-90, 25);
            descRt.anchoredPosition = new Vector2(30, -15);
            
            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = "技能描述";
            descText.fontSize = 14;
            descText.alignment = TextAlignmentOptions.Left;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            // CD/费用显示
            GameObject costObj = new GameObject("SkillCost");
            costObj.transform.SetParent(skillBtn.transform);
            
            RectTransform costRt = costObj.AddComponent<RectTransform>();
            costRt.anchorMin = new Vector2(1, 1);
            costRt.anchorMax = new Vector2(1, 1);
            costRt.sizeDelta = new Vector2(40, 40);
            costRt.anchoredPosition = new Vector2(-10, -10);
            
            Image costBg = costObj.AddComponent<Image>();
            costBg.color = new Color(0.8f, 0.3f, 0.3f, 0.8f);
            
            GameObject costTextObj = new GameObject("Text");
            costTextObj.transform.SetParent(costObj.transform);
            
            RectTransform costTextRt = costTextObj.AddComponent<RectTransform>();
            costTextRt.anchorMin = Vector2.zero;
            costTextRt.anchorMax = Vector2.one;
            costTextRt.sizeDelta = Vector2.zero;
            costTextRt.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI costText = costTextObj.AddComponent<TextMeshProUGUI>();
            costText.text = "1";
            costText.fontSize = 18;
            costText.alignment = TextAlignmentOptions.Center;
            costText.color = Color.white;
            costText.fontStyle = FontStyles.Bold;

            SavePrefab(skillBtn, "Assets/Prefabs/UI/SkillButton.prefab");
            
            EditorUtility.DisplayDialog("完成", "技能按钮预制件已创建！", "确定");
        }

        /// <summary>
        /// 保存预制件
        /// </summary>
        private void SavePrefab(GameObject obj, string path)
        {
            // 确保目录存在
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // 保存预制件
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            
            // 清理场景中的临时对象
            DestroyImmediate(obj);
            
            Debug.Log($"预制件已保存到: {path}");
        }
    }
}

