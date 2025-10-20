using UnityEngine;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 地形控制器
    /// </summary>
    public class TerrainController : MonoBehaviour
    {
        public string TerrainId { get; private set; }
        
        [Header("Visual")]
        [SerializeField] private Renderer modelRenderer;
        [SerializeField] private ParticleSystem effectParticles;

        private TerrainState currentState;

        /// <summary>
        /// 初始化地形
        /// </summary>
        public void Initialize(TerrainState state)
        {
            currentState = state;
            TerrainId = state.TerrainId;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            // 根据地形类型设置外观
            switch (currentState.Type)
            {
                case "山地":
                    SetColor(new Color(0.5f, 0.3f, 0.2f)); // 棕色
                    break;
                case "森林":
                    SetColor(new Color(0.2f, 0.6f, 0.2f)); // 绿色
                    break;
                case "河流":
                    SetColor(new Color(0.2f, 0.4f, 0.8f)); // 蓝色
                    break;
                case "火焰":
                    SetColor(Color.red);
                    PlayEffect();
                    break;
                default:
                    SetColor(Color.gray);
                    break;
            }
        }

        private void SetColor(Color color)
        {
            if (modelRenderer != null)
            {
                modelRenderer.material.color = color;
            }
        }

        private void PlayEffect()
        {
            if (effectParticles != null)
            {
                effectParticles.Play();
            }
        }

        /// <summary>
        /// 激活地形效果
        /// </summary>
        public void Activate()
        {
            Debug.Log($"Terrain {currentState.Name} activated!");
            currentState.IsActivated = true;
            PlayEffect();
        }

        /// <summary>
        /// 地形消失
        /// </summary>
        public void Remove()
        {
            Debug.Log($"Terrain {currentState.Name} removed!");
            Destroy(gameObject, 0.5f);
        }
    }
}

