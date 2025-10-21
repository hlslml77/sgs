using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;

namespace SanguoStrategy.Editor
{
    /// <summary>
    /// UI设置辅助工具 - 自动生成主菜单、房间列表、选将界面等UI元素
    /// </summary>
    public class UISetupHelper : EditorWindow
    {
        [MenuItem("三国策略/UI设置工具")]
        public static void ShowWindow()
        {
            GetWindow<UISetupHelper>("UI设置工具");
        }

        private void OnGUI()
        {
            GUILayout.Label("UI自动生成工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("设置主菜单UI", GUILayout.Height(30)))
            {
                SetupMainMenuUI();
            }

            if (GUILayout.Button("设置房间列表UI", GUILayout.Height(30)))
            {
                SetupRoomListUI();
            }

            if (GUILayout.Button("设置选将界面UI", GUILayout.Height(30)))
            {
                SetupHeroSelectionUI();
            }

            if (GUILayout.Button("设置游戏场景UI", GUILayout.Height(30)))
            {
                SetupGameSceneUI();
            }

            if (GUILayout.Button("设置地形编辑器UI", GUILayout.Height(30)))
            {
                SetupTerrainEditorUI();
            }

            GUILayout.Space(20);
            GUILayout.Label("提示：请先打开对应场景，然后点击上面的按钮", EditorStyles.helpBox);
        }

        /// <summary>
        /// 设置主菜单UI
        /// </summary>
        private void SetupMainMenuUI()
        {
            // 创建Canvas
            GameObject canvasObj = CreateCanvas("MainMenuCanvas");
            Canvas canvas = canvasObj.GetComponent<Canvas>();

            // 创建背景
            CreateBackground(canvasObj.transform);

            // 创建标题
            CreateTitle(canvasObj.transform, "三国策略 - 地形玩法版");

            // 创建玩家信息面板
            GameObject playerInfoPanel = CreatePlayerInfoPanel(canvasObj.transform);

            // 创建主按钮组
            GameObject buttonPanel = CreateMainButtons(canvasObj.transform);

            // 添加主菜单控制器
            var controller = canvasObj.AddComponent<SanguoStrategy.UI.MainMenuController>();
            
            // 通过反射设置私有字段（因为是SerializeField）
            SetupMainMenuController(controller, buttonPanel, playerInfoPanel);

            EditorUtility.DisplayDialog("完成", "主菜单UI已设置完成！", "确定");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// 设置房间列表UI
        /// </summary>
        private void SetupRoomListUI()
        {
            GameObject canvasObj = CreateCanvas("RoomListCanvas");
            
            CreateBackground(canvasObj.transform);
            CreateTitle(canvasObj.transform, "房间列表");

            // 创建房间列表滚动视图
            GameObject scrollView = CreateRoomListScrollView(canvasObj.transform);

            // 创建底部按钮
            CreateRoomListButtons(canvasObj.transform);

            // 创建"创建房间"面板
            GameObject createRoomPanel = CreateCreateRoomPanel(canvasObj.transform);

            EditorUtility.DisplayDialog("完成", "房间列表UI已设置完成！", "确定");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// 设置选将界面UI
        /// </summary>
        private void SetupHeroSelectionUI()
        {
            GameObject canvasObj = CreateCanvas("HeroSelectionCanvas");
            
            CreateBackground(canvasObj.transform);
            CreateTitle(canvasObj.transform, "选择武将 - 每个职能选1个");

            // 创建倒计时显示
            CreateTimerText(canvasObj.transform);

            // 创建职能分类容器
            CreateHeroRoleContainers(canvasObj.transform);

            // 创建已选武将显示区
            CreateSelectedHeroesDisplay(canvasObj.transform);

            // 创建底部按钮
            CreateHeroSelectionButtons(canvasObj.transform);

            EditorUtility.DisplayDialog("完成", "选将界面UI已设置完成！", "确定");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// 设置游戏场景UI
        /// </summary>
        private void SetupGameSceneUI()
        {
            GameObject canvasObj = CreateCanvas("GameCanvas");
            
            // 创建游戏信息显示（回合、阶段、行动点）
            CreateGameInfo(canvasObj.transform);

            // 创建手牌区
            CreateHandCardsArea(canvasObj.transform);

            // 创建技能区
            CreateSkillsArea(canvasObj.transform);

            // 创建消息面板
            CreateMessagePanel(canvasObj.transform);

            EditorUtility.DisplayDialog("完成", "游戏场景UI已设置完成！", "确定");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// 设置地形编辑器UI
        /// </summary>
        private void SetupTerrainEditorUI()
        {
            GameObject canvasObj = CreateCanvas("TerrainEditorCanvas");
            
            CreateBackground(canvasObj.transform);
            CreateTitle(canvasObj.transform, "地形编辑器");

            // 创建地形块工具栏
            CreateTerrainToolbar(canvasObj.transform);

            // 创建属性面板
            CreateTerrainPropertiesPanel(canvasObj.transform);

            // 创建预览区信息
            CreateTerrainPreviewInfo(canvasObj.transform);

            EditorUtility.DisplayDialog("完成", "地形编辑器UI已设置完成！", "确定");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        #region Helper Methods

        private GameObject CreateCanvas(string name)
        {
            // 查找是否已存在Canvas
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                if (EditorUtility.DisplayDialog("提示", $"已存在{name}，是否替换？", "是", "否"))
                {
                    DestroyImmediate(existing);
                }
                else
                {
                    return existing;
                }
            }

            GameObject canvasObj = new GameObject(name);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();

            return canvasObj;
        }

        private void CreateBackground(Transform parent)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(parent);
            
            RectTransform rt = bg.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            
            Image image = bg.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.15f, 1f);
        }

        private void CreateTitle(Transform parent, string titleText)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent);
            
            RectTransform rt = titleObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(800, 100);
            rt.anchoredPosition = new Vector2(0, -80);
            
            TextMeshProUGUI text = titleObj.AddComponent<TextMeshProUGUI>();
            text.text = titleText;
            text.fontSize = 60;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(1f, 0.85f, 0.4f);
            text.fontStyle = FontStyles.Bold;
        }

        private GameObject CreatePlayerInfoPanel(Transform parent)
        {
            GameObject panel = new GameObject("PlayerInfoPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(400, 150);
            rt.anchoredPosition = new Vector2(220, -100);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f, 0.8f);

            // 创建玩家名称文本
            CreateText(panel.transform, "PlayerNameText", new Vector2(0, 40), "玩家名称", 28);
            
            // 创建等级文本
            CreateText(panel.transform, "PlayerLevelText", new Vector2(0, 0), "等级: 1", 24);
            
            // 创建欢乐豆文本
            CreateText(panel.transform, "PlayerCoinsText", new Vector2(0, -40), "欢乐豆: 1000", 24);

            return panel;
        }

        private GameObject CreateMainButtons(Transform parent)
        {
            GameObject buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(parent);
            
            RectTransform rt = buttonPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400, 500);
            rt.anchoredPosition = new Vector2(0, -50);

            // 创建按钮
            CreateButton(buttonPanel.transform, "QuickMatchButton", new Vector2(0, 150), "快速匹配", new Color(0.2f, 0.7f, 0.3f));
            CreateButton(buttonPanel.transform, "RoomListButton", new Vector2(0, 50), "房间列表", new Color(0.3f, 0.5f, 0.8f));
            CreateButton(buttonPanel.transform, "ProfileButton", new Vector2(0, -50), "玩家资料", new Color(0.6f, 0.4f, 0.8f));
            CreateButton(buttonPanel.transform, "SettingsButton", new Vector2(0, -150), "设置", new Color(0.5f, 0.5f, 0.5f));
            CreateButton(buttonPanel.transform, "ExitButton", new Vector2(0, -250), "退出游戏", new Color(0.8f, 0.3f, 0.3f));

            return buttonPanel;
        }

        private GameObject CreateButton(Transform parent, string name, Vector2 position, string text, Color color)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent);
            
            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(350, 70);
            rt.anchoredPosition = position;
            
            Image image = btnObj.AddComponent<Image>();
            image.color = color;
            
            Button button = btnObj.AddComponent<Button>();
            button.targetGraphic = image;

            // 添加文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform);
            
            RectTransform textRt = textObj.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;
            textRt.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
            textComp.text = text;
            textComp.fontSize = 32;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.color = Color.white;
            textComp.fontStyle = FontStyles.Bold;

            return btnObj;
        }

        private GameObject CreateText(Transform parent, string name, Vector2 position, string text, float fontSize)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            
            RectTransform rt = textObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(380, 40);
            rt.anchoredPosition = position;
            
            TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.color = Color.white;

            return textObj;
        }

        private GameObject CreateRoomListScrollView(Transform parent)
        {
            GameObject scrollView = new GameObject("RoomListScrollView");
            scrollView.transform.SetParent(parent);
            
            RectTransform rt = scrollView.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(1400, 700);
            rt.anchoredPosition = new Vector2(0, 0);
            
            Image bg = scrollView.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
            
            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();

            // 创建Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform);
            
            RectTransform vpRt = viewport.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.sizeDelta = Vector2.zero;
            vpRt.anchoredPosition = Vector2.zero;
            
            viewport.AddComponent<Mask>();
            Image vpImage = viewport.AddComponent<Image>();
            vpImage.color = Color.clear;

            // 创建Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform);
            
            RectTransform contentRt = content.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1);
            contentRt.sizeDelta = new Vector2(0, 1000);
            contentRt.anchoredPosition = Vector2.zero;
            
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            scrollRect.viewport = vpRt;
            scrollRect.content = contentRt;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;

            return scrollView;
        }

        private void CreateRoomListButtons(Transform parent)
        {
            GameObject buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(parent);
            
            RectTransform rt = buttonPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(800, 80);
            rt.anchoredPosition = new Vector2(0, 80);

            CreateButton(buttonPanel.transform, "CreateRoomButton", new Vector2(-250, 0), "创建房间", new Color(0.2f, 0.7f, 0.3f));
            CreateButton(buttonPanel.transform, "RefreshButton", new Vector2(0, 0), "刷新", new Color(0.3f, 0.5f, 0.8f));
            CreateButton(buttonPanel.transform, "BackButton", new Vector2(250, 0), "返回", new Color(0.6f, 0.4f, 0.4f));
        }

        private GameObject CreateCreateRoomPanel(Transform parent)
        {
            GameObject panel = new GameObject("CreateRoomPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);

            // 创建面板内容
            GameObject contentPanel = new GameObject("ContentPanel");
            contentPanel.transform.SetParent(panel.transform);
            
            RectTransform contentRt = contentPanel.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0.5f, 0.5f);
            contentRt.anchorMax = new Vector2(0.5f, 0.5f);
            contentRt.sizeDelta = new Vector2(600, 500);
            contentRt.anchoredPosition = Vector2.zero;
            
            Image contentBg = contentPanel.AddComponent<Image>();
            contentBg.color = new Color(0.2f, 0.2f, 0.25f, 1f);

            CreateText(contentPanel.transform, "Title", new Vector2(0, 200), "创建房间", 40);
            CreateText(contentPanel.transform, "RoomNameLabel", new Vector2(0, 120), "房间名称:", 28);
            // TODO: 添加输入框、下拉框等
            
            CreateButton(contentPanel.transform, "ConfirmCreateButton", new Vector2(-120, -180), "确认", new Color(0.2f, 0.7f, 0.3f));
            CreateButton(contentPanel.transform, "CancelCreateButton", new Vector2(120, -180), "取消", new Color(0.6f, 0.4f, 0.4f));

            panel.SetActive(false);
            return panel;
        }

        private void CreateTimerText(Transform parent)
        {
            CreateText(parent, "TimerText", new Vector2(0, 450), "剩余时间: 10s", 36);
        }

        private void CreateHeroRoleContainers(Transform parent)
        {
            GameObject container = new GameObject("HeroRoleContainers");
            container.transform.SetParent(parent);
            
            RectTransform rt = container.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(1800, 600);
            rt.anchoredPosition = new Vector2(0, 0);

            HorizontalLayoutGroup layout = container.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateHeroRolePanel(container.transform, "AttackerPanel", "输出型", new Color(0.8f, 0.3f, 0.3f));
            CreateHeroRolePanel(container.transform, "ControllerPanel", "控制型", new Color(0.3f, 0.5f, 0.8f));
            CreateHeroRolePanel(container.transform, "SupporterPanel", "辅助型", new Color(0.3f, 0.7f, 0.4f));
            CreateHeroRolePanel(container.transform, "SpecialPanel", "特殊型", new Color(0.7f, 0.5f, 0.9f));
        }

        private void CreateHeroRolePanel(Transform parent, string name, string title, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 0.8f);

            // 标题
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panel.transform);
            
            RectTransform titleRt = titleObj.AddComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0, 1);
            titleRt.anchorMax = new Vector2(1, 1);
            titleRt.sizeDelta = new Vector2(0, 50);
            titleRt.anchoredPosition = new Vector2(0, -25);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = title;
            titleText.fontSize = 28;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = color;
            titleText.fontStyle = FontStyles.Bold;

            // Hero容器
            GameObject heroContainer = new GameObject("HeroContainer");
            heroContainer.transform.SetParent(panel.transform);
            
            RectTransform heroRt = heroContainer.AddComponent<RectTransform>();
            heroRt.anchorMin = new Vector2(0, 0);
            heroRt.anchorMax = new Vector2(1, 1);
            heroRt.sizeDelta = new Vector2(-20, -70);
            heroRt.anchoredPosition = new Vector2(0, -10);
            
            VerticalLayoutGroup heroLayout = heroContainer.AddComponent<VerticalLayoutGroup>();
            heroLayout.spacing = 10;
            heroLayout.padding = new RectOffset(10, 10, 10, 10);
            heroLayout.childControlWidth = true;
            heroLayout.childControlHeight = false;
        }

        private void CreateSelectedHeroesDisplay(Transform parent)
        {
            GameObject panel = new GameObject("SelectedHeroesPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(1400, 150);
            rt.anchoredPosition = new Vector2(0, 180);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f, 0.9f);

            CreateText(panel.transform, "SelectedCountText", new Vector2(0, 50), "已选择: 0/4", 28);
        }

        private void CreateHeroSelectionButtons(Transform parent)
        {
            GameObject buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(parent);
            
            RectTransform rt = buttonPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(600, 80);
            rt.anchoredPosition = new Vector2(0, 60);

            CreateButton(buttonPanel.transform, "RandomSelectButton", new Vector2(-180, 0), "随机选择", new Color(0.6f, 0.5f, 0.3f));
            CreateButton(buttonPanel.transform, "ConfirmButton", new Vector2(180, 0), "确认选择", new Color(0.2f, 0.7f, 0.3f));
        }

        private void CreateGameInfo(Transform parent)
        {
            GameObject panel = new GameObject("GameInfoPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.sizeDelta = new Vector2(0, 80);
            rt.anchoredPosition = new Vector2(0, -40);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

            CreateText(panel.transform, "RoundText", new Vector2(-700, 0), "回合: 1/12", 28);
            CreateText(panel.transform, "PhaseText", new Vector2(0, 0), "阶段: 部署", 28);
            CreateText(panel.transform, "ActionPointsText", new Vector2(700, 0), "行动点: 3", 28);
        }

        private void CreateHandCardsArea(Transform parent)
        {
            GameObject panel = new GameObject("HandCardsPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(1600, 200);
            rt.anchoredPosition = new Vector2(0, 120);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

            GameObject container = new GameObject("HandCardsContainer");
            container.transform.SetParent(panel.transform);
            
            RectTransform containerRt = container.AddComponent<RectTransform>();
            containerRt.anchorMin = Vector2.zero;
            containerRt.anchorMax = Vector2.one;
            containerRt.sizeDelta = Vector2.zero;
            containerRt.anchoredPosition = Vector2.zero;
            
            HorizontalLayoutGroup layout = container.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = true;
        }

        private void CreateSkillsArea(Transform parent)
        {
            GameObject panel = new GameObject("SkillsPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(300, 600);
            rt.anchoredPosition = new Vector2(-160, 0);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

            CreateText(panel.transform, "SkillsTitle", new Vector2(0, 270), "武将技能", 28);

            GameObject container = new GameObject("SkillsContainer");
            container.transform.SetParent(panel.transform);
            
            RectTransform containerRt = container.AddComponent<RectTransform>();
            containerRt.anchorMin = new Vector2(0, 0);
            containerRt.anchorMax = new Vector2(1, 1);
            containerRt.sizeDelta = new Vector2(-20, -80);
            containerRt.anchoredPosition = new Vector2(0, -20);
            
            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
        }

        private void CreateMessagePanel(Transform parent)
        {
            GameObject panel = new GameObject("MessagePanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(500, 200);
            rt.anchoredPosition = Vector2.zero;
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);

            CreateText(panel.transform, "MessageText", Vector2.zero, "", 24);

            panel.SetActive(false);
        }

        private void CreateTerrainToolbar(Transform parent)
        {
            GameObject toolbar = new GameObject("TerrainToolbar");
            toolbar.transform.SetParent(parent);
            
            RectTransform rt = toolbar.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.sizeDelta = new Vector2(250, 800);
            rt.anchoredPosition = new Vector2(135, 0);
            
            Image bg = toolbar.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

            CreateText(toolbar.transform, "ToolbarTitle", new Vector2(0, 370), "地形块", 28);
        }

        private void CreateTerrainPropertiesPanel(Transform parent)
        {
            GameObject panel = new GameObject("PropertiesPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(350, 800);
            rt.anchoredPosition = new Vector2(-185, 0);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

            CreateText(panel.transform, "PropertiesTitle", new Vector2(0, 370), "属性", 28);
        }

        private void CreateTerrainPreviewInfo(Transform parent)
        {
            GameObject panel = new GameObject("PreviewInfoPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(1200, 100);
            rt.anchoredPosition = new Vector2(0, -70);
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

            CreateText(panel.transform, "InfoText", Vector2.zero, "选择地形块放置到预览区", 24);
        }

        private void SetupMainMenuController(object controller, GameObject buttonPanel, GameObject playerInfoPanel)
        {
            // 使用反射设置私有字段
            var type = controller.GetType();
            
            // 设置按钮引用
            var buttons = buttonPanel.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                string fieldName = btn.name.Substring(0, 1).ToLower() + btn.name.Substring(1);
                var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(controller, btn);
                }
            }
            
            // 设置文本引用
            var texts = playerInfoPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                string fieldName = txt.name.Substring(0, 1).ToLower() + txt.name.Substring(1);
                var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(controller, txt);
                }
            }
        }

        #endregion
    }
}

