using UnityEngine;
using TMPro;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 武将控制器
    /// </summary>
    public class GeneralController : MonoBehaviour
    {
        public string GeneralId { get; private set; }
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private TextMeshPro hpText;
        [SerializeField] private GameObject selectionIndicator;

        [Header("Visual")]
        [SerializeField] private Renderer modelRenderer;
        [SerializeField] private Color teamRedColor = Color.red;
        [SerializeField] private Color teamBlueColor = Color.blue;

        private GeneralState currentState;
        public event System.Action<GameObject> OnGeneralClicked;

        /// <summary>
        /// 初始化武将
        /// </summary>
        public void Initialize(GeneralState state)
        {
            currentState = state;
            GeneralId = state.GeneralId;
            UpdateVisuals();
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void UpdateState(GeneralState state)
        {
            currentState = state;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (nameText != null)
            {
                nameText.text = currentState.Name;
            }

            if (hpText != null)
            {
                hpText.text = $"{currentState.CurrentHP}/{currentState.MaxHP}";
            }

            // 根据阵营设置颜色
            // TODO: 实现根据阵营获取颜色
        }

        /// <summary>
        /// 显示/隐藏选中指示器
        /// </summary>
        public void ShowSelection(bool show)
        {
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(show);
            }
        }

        private void OnMouseDown()
        {
            OnGeneralClicked?.Invoke(gameObject);
        }

        /// <summary>
        /// 播放攻击动画
        /// </summary>
        public void PlayAttackAnimation()
        {
            // TODO: 实现攻击动画
            Debug.Log($"{currentState.Name} attacks!");
        }

        /// <summary>
        /// 播放受击动画
        /// </summary>
        public void PlayHitAnimation()
        {
            // TODO: 实现受击动画
            Debug.Log($"{currentState.Name} takes damage!");
        }

        /// <summary>
        /// 播放死亡动画
        /// </summary>
        public void PlayDeathAnimation()
        {
            // TODO: 实现死亡动画
            Debug.Log($"{currentState.Name} dies!");
            Destroy(gameObject, 1f);
        }
    }
}

