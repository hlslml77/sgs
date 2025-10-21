using System.Collections.Generic;
using UnityEngine;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 卡牌管理器
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        [Header("Card Prefabs")]
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform handCardsContainer;

        private List<GameObject> handCardObjects = new List<GameObject>();
        private GameObject selectedCard;

        /// <summary>
        /// 显示手牌
        /// </summary>
        public void DisplayHandCards(List<Card> cards)
        {
            ClearHandCards();

            for (int i = 0; i < cards.Count; i++)
            {
                GameObject cardObj = Instantiate(cardPrefab, handCardsContainer);
                var cardController = cardObj.GetComponent<CardController>();
                
                if (cardController != null)
                {
                    cardController.Initialize(cards[i]);
                    cardController.OnCardClicked += HandleCardClicked;
                }

                handCardObjects.Add(cardObj);
            }

            ArrangeHandCards();
        }

        private void ArrangeHandCards()
        {
            // 排列手牌位置
            float spacing = 120f;
            float startX = -(handCardObjects.Count - 1) * spacing / 2f;

            for (int i = 0; i < handCardObjects.Count; i++)
            {
                RectTransform rectTransform = handCardObjects[i].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(startX + i * spacing, 0);
                }
            }
        }

        private void HandleCardClicked(GameObject card)
        {
            Debug.Log($"Card clicked: {card.name}");
            
            if (selectedCard == card)
            {
                DeselectCard();
            }
            else
            {
                SelectCard(card);
            }
        }

        private void SelectCard(GameObject card)
        {
            DeselectCard();
            selectedCard = card;

            var cardController = card.GetComponent<CardController>();
            cardController?.ShowSelection(true);

            // 提升卡牌
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition += new Vector2(0, 30);
            }
        }

        private void DeselectCard()
        {
            if (selectedCard != null)
            {
                var cardController = selectedCard.GetComponent<CardController>();
                cardController?.ShowSelection(false);

                // 恢复位置
                ArrangeHandCards();
                selectedCard = null;
            }
        }

        /// <summary>
        /// 使用选中的卡牌
        /// </summary>
        public void UseSelectedCard(List<string> targetIds)
        {
            if (selectedCard != null)
            {
                var cardController = selectedCard.GetComponent<CardController>();
                if (cardController != null)
                {
                    GameManager.Instance.PlayCard(cardController.CardId, targetIds);
                    RemoveCard(selectedCard);
                    DeselectCard();
                }
            }
        }

        private void RemoveCard(GameObject card)
        {
            handCardObjects.Remove(card);
            Destroy(card);
            ArrangeHandCards();
        }

        private void ClearHandCards()
        {
            foreach (var card in handCardObjects)
            {
                if (card != null) Destroy(card);
            }
            handCardObjects.Clear();
        }
    }

    // 注意：CardController 已在 Game/CardController.cs 定义，此处移除重复定义
}

