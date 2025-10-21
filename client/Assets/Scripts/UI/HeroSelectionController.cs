using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 选将控制器 - 实现随机选将池、n选3机制、职能分类
    /// </summary>
    public class HeroSelectionController : MonoBehaviour
    {
        [Header("Selection Settings")]
        [SerializeField] private int heroesPerRole = 3; // 每个职能提供的武将数量
        [SerializeField] private float selectionTime = 10f; // 选将倒计时
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button randomSelectButton;
        
        [Header("Hero Display")]
        [SerializeField] private Transform attackerHeroesContainer;
        [SerializeField] private Transform controllerHeroesContainer;
        [SerializeField] private Transform supporterHeroesContainer;
        [SerializeField] private Transform specialHeroesContainer;
        [SerializeField] private GameObject heroCardPrefab;
        
        [Header("Selected Heroes Display")]
        [SerializeField] private Transform selectedHeroesContainer;
        [SerializeField] private TextMeshProUGUI selectedCountText;
        
        private Dictionary<HeroRole, List<HeroData>> heroPool = new Dictionary<HeroRole, List<HeroData>>();
        private List<HeroData> selectedHeroes = new List<HeroData>();
        private float remainingTime;
        private bool isSelectionComplete = false;
        
        private void Start()
        {
            remainingTime = selectionTime;
            
            InitializeButtons();
            GenerateRandomHeroPool();
            DisplayHeroes();
        }
        
        private void Update()
        {
            if (!isSelectionComplete)
            {
                UpdateTimer();
            }
        }
        
        private void InitializeButtons()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmSelection);
                confirmButton.interactable = false;
            }
                
            if (randomSelectButton != null)
                randomSelectButton.onClick.AddListener(OnRandomSelect);
        }
        
        /// <summary>
        /// 生成随机武将池 - 按职能分类，每类提供n个武将供选择
        /// </summary>
        private void GenerateRandomHeroPool()
        {
            // TODO: 从服务器获取武将数据
            // 这里使用测试数据
            
            heroPool[HeroRole.Attacker] = GetRandomHeroesByRole(HeroRole.Attacker, heroesPerRole);
            heroPool[HeroRole.Controller] = GetRandomHeroesByRole(HeroRole.Controller, heroesPerRole);
            heroPool[HeroRole.Supporter] = GetRandomHeroesByRole(HeroRole.Supporter, heroesPerRole);
            heroPool[HeroRole.Special] = GetRandomHeroesByRole(HeroRole.Special, heroesPerRole);
        }
        
        /// <summary>
        /// 根据职能获取随机武将
        /// </summary>
        private List<HeroData> GetRandomHeroesByRole(HeroRole role, int count)
        {
            // TODO: 实际从数据库获取
            List<HeroData> allHeroes = GetAllHeroesByRole(role);
            
            // 随机选择
            allHeroes = allHeroes.OrderBy(x => Random.value).Take(count).ToList();
            
            return allHeroes;
        }
        
        /// <summary>
        /// 获取所有指定职能的武将（测试数据）
        /// </summary>
        private List<HeroData> GetAllHeroesByRole(HeroRole role)
        {
            List<HeroData> heroes = new List<HeroData>();
            
            switch (role)
            {
                case HeroRole.Attacker:
                    heroes.Add(new HeroData { heroId = "1", heroName = "关羽", role = HeroRole.Attacker, faction = "蜀" });
                    heroes.Add(new HeroData { heroId = "2", heroName = "张飞", role = HeroRole.Attacker, faction = "蜀" });
                    heroes.Add(new HeroData { heroId = "3", heroName = "赵云", role = HeroRole.Attacker, faction = "蜀" });
                    heroes.Add(new HeroData { heroId = "4", heroName = "吕布", role = HeroRole.Attacker, faction = "群" });
                    heroes.Add(new HeroData { heroId = "5", heroName = "孙尚香", role = HeroRole.Attacker, faction = "吴" });
                    break;
                case HeroRole.Controller:
                    heroes.Add(new HeroData { heroId = "6", heroName = "诸葛亮", role = HeroRole.Controller, faction = "蜀" });
                    heroes.Add(new HeroData { heroId = "7", heroName = "周瑜", role = HeroRole.Controller, faction = "吴" });
                    heroes.Add(new HeroData { heroId = "8", heroName = "司马懿", role = HeroRole.Controller, faction = "魏" });
                    heroes.Add(new HeroData { heroId = "9", heroName = "郭嘉", role = HeroRole.Controller, faction = "魏" });
                    break;
                case HeroRole.Supporter:
                    heroes.Add(new HeroData { heroId = "10", heroName = "华佗", role = HeroRole.Supporter, faction = "群" });
                    heroes.Add(new HeroData { heroId = "11", heroName = "蔡文姬", role = HeroRole.Supporter, faction = "魏" });
                    heroes.Add(new HeroData { heroId = "12", heroName = "孙权", role = HeroRole.Supporter, faction = "吴" });
                    break;
                case HeroRole.Special:
                    heroes.Add(new HeroData { heroId = "13", heroName = "貂蝉", role = HeroRole.Special, faction = "群" });
                    heroes.Add(new HeroData { heroId = "14", heroName = "曹操", role = HeroRole.Special, faction = "魏" });
                    heroes.Add(new HeroData { heroId = "15", heroName = "刘备", role = HeroRole.Special, faction = "蜀" });
                    break;
            }
            
            return heroes;
        }
        
        /// <summary>
        /// 显示武将选项
        /// </summary>
        private void DisplayHeroes()
        {
            DisplayHeroesInContainer(attackerHeroesContainer, heroPool[HeroRole.Attacker]);
            DisplayHeroesInContainer(controllerHeroesContainer, heroPool[HeroRole.Controller]);
            DisplayHeroesInContainer(supporterHeroesContainer, heroPool[HeroRole.Supporter]);
            DisplayHeroesInContainer(specialHeroesContainer, heroPool[HeroRole.Special]);
        }
        
        private void DisplayHeroesInContainer(Transform container, List<HeroData> heroes)
        {
            if (container == null || heroCardPrefab == null) return;
            
            foreach (var hero in heroes)
            {
                GameObject card = Instantiate(heroCardPrefab, container);
                
                // 设置武将卡片信息
                TextMeshProUGUI nameText = card.GetComponentInChildren<TextMeshProUGUI>();
                if (nameText != null)
                    nameText.text = $"{hero.heroName}\n({hero.faction})";
                
                // 添加点击事件
                Button button = card.GetComponent<Button>();
                if (button != null)
                {
                    HeroData heroCopy = hero;
                    button.onClick.AddListener(() => OnSelectHero(heroCopy));
                }
            }
        }
        
        /// <summary>
        /// 选择武将
        /// </summary>
        private void OnSelectHero(HeroData hero)
        {
            // 每个职能只能选1个
            var existingHero = selectedHeroes.FirstOrDefault(h => h.role == hero.role);
            if (existingHero != null)
            {
                selectedHeroes.Remove(existingHero);
            }
            
            selectedHeroes.Add(hero);
            UpdateSelectedHeroesDisplay();
            
            Debug.Log($"选择武将: {hero.heroName} ({hero.role})");
        }
        
        /// <summary>
        /// 随机选择武将
        /// </summary>
        private void OnRandomSelect()
        {
            selectedHeroes.Clear();
            
            foreach (var roleHeroes in heroPool.Values)
            {
                if (roleHeroes.Count > 0)
                {
                    int randomIndex = Random.Range(0, roleHeroes.Count);
                    selectedHeroes.Add(roleHeroes[randomIndex]);
                }
            }
            
            UpdateSelectedHeroesDisplay();
            Debug.Log("随机选择武将完成");
        }
        
        /// <summary>
        /// 更新已选武将显示
        /// </summary>
        private void UpdateSelectedHeroesDisplay()
        {
            if (selectedCountText != null)
                selectedCountText.text = $"已选择: {selectedHeroes.Count}/4";
            
            // 更新确认按钮状态
            if (confirmButton != null)
                confirmButton.interactable = selectedHeroes.Count == 4;
            
            // TODO: 更新已选武将容器显示
        }
        
        /// <summary>
        /// 确认选择
        /// </summary>
        private void OnConfirmSelection()
        {
            if (selectedHeroes.Count != 4)
            {
                Debug.LogWarning("请选择所有职能的武将！");
                return;
            }
            
            isSelectionComplete = true;
            Debug.Log("选将完成，进入游戏场景");
            
            // TODO: 发送选择结果到服务器
            // networkManager.SendHeroSelection(selectedHeroes);
            
            // 进入游戏场景
            SceneManager.LoadScene("GameScene");
        }
        
        /// <summary>
        /// 更新倒计时
        /// </summary>
        private void UpdateTimer()
        {
            remainingTime -= Time.deltaTime;
            
            if (timerText != null)
                timerText.text = $"剩余时间: {Mathf.CeilToInt(remainingTime)}s";
            
            if (remainingTime <= 0)
            {
                // 时间到，自动随机选择
                OnRandomSelect();
                OnConfirmSelection();
            }
        }
        
        /// <summary>
        /// 武将数据结构
        /// </summary>
        [System.Serializable]
        public class HeroData
        {
            public string heroId;
            public string heroName;
            public HeroRole role;
            public string faction;
            public string description;
        }
        
        /// <summary>
        /// 武将职能枚举
        /// </summary>
        public enum HeroRole
        {
            Attacker,   // 输出型
            Controller, // 控制型
            Supporter,  // 辅助型
            Special     // 特殊型
        }
    }
}

