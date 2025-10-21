using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 卡牌控制器 - 处理卡牌显示、交互和效果
    /// </summary>
    public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("Card Data")]
        [SerializeField] private CardData cardData;
        
        [Header("UI References")]
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI cardDescriptionText;
        [SerializeField] private TextMeshProUGUI cardCostText;
        [SerializeField] private Image cardTypeIcon;
        
        [Header("Visual Effects")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float hoverLiftHeight = 30f;
        [SerializeField] private GameObject glowEffect;
        
        private Vector3 originalPosition;
        private Vector3 originalScale;
        private bool isDragging = false;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        
        // 事件和属性
        public event System.Action<GameObject> OnCardClicked;
        public string CardId => cardData?.cardId;
        
        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                
            originalScale = transform.localScale;
        }
        
        /// <summary>
        /// 初始化卡牌数据
        /// </summary>
        public void Initialize(CardData data)
        {
            cardData = data;
            UpdateCardDisplay();
        }
        
        /// <summary>
        /// 从 Game.Card 初始化卡牌
        /// </summary>
        public void Initialize(Card card)
        {
            cardData = new CardData
            {
                cardId = card.Id,
                cardName = card.Name,
                cardType = ConvertCardType(card.Type),
                description = GetCardDescription(card.Type),
                cost = 1, // 默认消耗
                effectData = ""
            };
            UpdateCardDisplay();
        }
        
        /// <summary>
        /// 转换卡牌类型
        /// </summary>
        private CardType ConvertCardType(string typeString)
        {
            switch (typeString?.ToLower())
            {
                case "kill":
                case "杀":
                    return CardType.Kill;
                case "dodge":
                case "闪":
                    return CardType.Dodge;
                case "peach":
                case "桃":
                    return CardType.Peach;
                case "fire_attack":
                case "火攻":
                    return CardType.FireAttack;
                case "lightning":
                case "闪电":
                    return CardType.Lightning;
                case "supply":
                case "兵粮寸断":
                    return CardType.Supply;
                case "terrain":
                case "地形":
                    return CardType.Terrain;
                case "equipment":
                case "装备":
                    return CardType.Equipment;
                default:
                    return CardType.Strategy;
            }
        }
        
        /// <summary>
        /// 获取卡牌描述
        /// </summary>
        private string GetCardDescription(string cardType)
        {
            // TODO: 从配置或本地化文件读取描述
            return $"{cardType} 卡牌";
        }
        
        /// <summary>
        /// 更新卡牌显示
        /// </summary>
        private void UpdateCardDisplay()
        {
            if (cardData == null) return;
            
            if (cardNameText != null)
                cardNameText.text = cardData.cardName;
                
            if (cardDescriptionText != null)
                cardDescriptionText.text = cardData.description;
                
            if (cardCostText != null)
                cardCostText.text = cardData.cost.ToString();
            
            // TODO: 加载卡牌图片
            // if (cardImage != null)
            //     cardImage.sprite = Resources.Load<Sprite>($"Cards/{cardData.cardId}");
        }
        
        /// <summary>
        /// 鼠标进入
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isDragging)
            {
                transform.localScale = originalScale * hoverScale;
                transform.position += Vector3.up * hoverLiftHeight;
                
                if (glowEffect != null)
                    glowEffect.SetActive(true);
                    
                // 显示详细信息
                ShowCardDetails();
            }
        }
        
        /// <summary>
        /// 鼠标离开
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isDragging)
            {
                transform.localScale = originalScale;
                transform.position = originalPosition;
                
                if (glowEffect != null)
                    glowEffect.SetActive(false);
                    
                HideCardDetails();
            }
        }
        
        /// <summary>
        /// 点击卡牌
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isDragging)
            {
                Debug.Log($"点击卡牌: {cardData?.cardName}");
                OnCardClicked?.Invoke(gameObject);
            }
        }
        
        /// <summary>
        /// 显示/隐藏选中效果
        /// </summary>
        public void ShowSelection(bool show)
        {
            if (glowEffect != null)
            {
                glowEffect.SetActive(show);
            }
            
            if (show)
            {
                transform.localScale = originalScale * hoverScale;
            }
            else
            {
                transform.localScale = originalScale;
            }
        }
        
        /// <summary>
        /// 开始拖拽
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPosition = transform.position;
            isDragging = true;
            
            if (canvasGroup != null)
                canvasGroup.alpha = 0.7f;
                
            if (glowEffect != null)
                glowEffect.SetActive(false);
        }
        
        /// <summary>
        /// 拖拽中
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (canvas != null)
            {
                transform.position = eventData.position;
            }
        }
        
        /// <summary>
        /// 结束拖拽
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
            
            // 检查是否拖拽到有效目标
            if (IsValidTarget(eventData.position))
            {
                PlayCard();
            }
            else
            {
                // 返回原位置
                transform.position = originalPosition;
                transform.localScale = originalScale;
            }
        }
        
        /// <summary>
        /// 检查是否为有效目标
        /// </summary>
        private bool IsValidTarget(Vector2 position)
        {
            // TODO: 实现目标检测逻辑
            // 射线检测是否在有效的地形/武将上
            return false;
        }
        
        /// <summary>
        /// 打出卡牌
        /// </summary>
        private void PlayCard()
        {
            if (cardData == null) return;
            
            Debug.Log($"打出卡牌: {cardData.cardName} (类型: {cardData.cardType})");
            
            // TODO: 调用GameManager处理卡牌效果
            // GameManager.Instance.PlayCard(cardData);
            
            // 播放动画
            PlayCardAnimation();
        }
        
        /// <summary>
        /// 播放卡牌动画
        /// </summary>
        private void PlayCardAnimation()
        {
            // TODO: 实现卡牌打出动画
            // 可以使用DOTween或Animator
        }
        
        /// <summary>
        /// 显示卡牌详细信息
        /// </summary>
        private void ShowCardDetails()
        {
            // TODO: 显示详细信息面板
        }
        
        /// <summary>
        /// 隐藏卡牌详细信息
        /// </summary>
        private void HideCardDetails()
        {
            // TODO: 隐藏详细信息面板
        }
        
        /// <summary>
        /// 卡牌数据结构
        /// </summary>
        [System.Serializable]
        public class CardData
        {
            public string cardId;
            public string cardName;
            public CardType cardType;
            public string description;
            public int cost;
            public string effectData;
        }
        
        /// <summary>
        /// 卡牌类型
        /// </summary>
        public enum CardType
        {
            Kill,           // 杀
            Dodge,          // 闪
            Peach,          // 桃
            FireAttack,     // 火攻
            Lightning,      // 闪电
            Supply,         // 兵粮寸断
            Terrain,        // 地形牌
            Equipment,      // 装备牌
            Strategy        // 锦囊牌
        }
    }
}

