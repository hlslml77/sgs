using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 背景管理器 - 创建和管理商业级游戏背景
    /// </summary>
    public class BackgroundManager : MonoBehaviour
    {
        [Header("背景类型")]
        [SerializeField] private BackgroundType backgroundType = BackgroundType.MainMenu;
        
        [Header("背景图片")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Sprite customBackground;
        
        [Header("渐变效果")]
        [SerializeField] private bool useGradientOverlay = true;
        [SerializeField] private Color gradientTopColor = new Color(0.1f, 0.1f, 0.2f, 0.8f);
        [SerializeField] private Color gradientBottomColor = new Color(0.2f, 0.1f, 0.1f, 0.8f);
        
        [Header("动态效果")]
        [SerializeField] private bool enableParallax = true;
        [SerializeField] private float parallaxSpeed = 10f;
        [SerializeField] private bool enablePulse = false;
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseAmount = 0.05f;
        
        [Header("粒子效果")]
        [SerializeField] private bool enableParticles = true;
        [SerializeField] private GameObject particlePrefab;
        [SerializeField] private int particleCount = 50;
        
        [Header("模糊效果")]
        [SerializeField] private bool enableBlur = false;
        [SerializeField] private Material blurMaterial;
        
        private RectTransform backgroundRect;
        private Image gradientOverlay;
        private Vector2 parallaxOffset;
        
        public enum BackgroundType
        {
            MainMenu,
            RoomList,
            HeroSelection,
            GameScene
        }
        
        private void Start()
        {
            InitializeBackground();
        }
        
        private void Update()
        {
            if (enableParallax)
            {
                UpdateParallax();
            }
            
            if (enablePulse)
            {
                UpdatePulse();
            }
        }
        
        /// <summary>
        /// 初始化背景
        /// </summary>
        private void InitializeBackground()
        {
            // 查找或创建背景Image
            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
                if (backgroundImage == null)
                {
                    backgroundImage = gameObject.AddComponent<Image>();
                }
            }
            
            backgroundRect = backgroundImage.GetComponent<RectTransform>();
            
            // 设置背景铺满整个Canvas
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.sizeDelta = Vector2.zero;
            backgroundRect.anchoredPosition = Vector2.zero;
            
            // 设置背景图片
            if (customBackground != null)
            {
                backgroundImage.sprite = customBackground;
            }
            else
            {
                // 使用程序生成的背景
                CreateProceduralBackground();
            }
            
            // 添加渐变遮罩
            if (useGradientOverlay)
            {
                CreateGradientOverlay();
            }
            
            // 添加粒子效果
            if (enableParticles)
            {
                CreateParticleEffect();
            }
            
            // 应用模糊效果
            if (enableBlur && blurMaterial != null)
            {
                backgroundImage.material = blurMaterial;
            }
        }
        
        /// <summary>
        /// 创建程序生成的背景
        /// </summary>
        private void CreateProceduralBackground()
        {
            // 创建渐变纹理
            Texture2D texture = new Texture2D(512, 512);
            Color[] pixels = new Color[512 * 512];
            
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    float t = (float)y / 512f;
                    
                    Color color;
                    switch (backgroundType)
                    {
                        case BackgroundType.MainMenu:
                            // 深红到暗金渐变（三国主题）
                            color = Color.Lerp(
                                new Color(0.3f, 0.1f, 0.1f), 
                                new Color(0.2f, 0.15f, 0.05f), 
                                t
                            );
                            break;
                        case BackgroundType.RoomList:
                            // 深蓝到紫色
                            color = Color.Lerp(
                                new Color(0.1f, 0.1f, 0.3f), 
                                new Color(0.2f, 0.1f, 0.3f), 
                                t
                            );
                            break;
                        case BackgroundType.HeroSelection:
                            // 深绿到青色（战场氛围）
                            color = Color.Lerp(
                                new Color(0.1f, 0.2f, 0.15f), 
                                new Color(0.05f, 0.15f, 0.2f), 
                                t
                            );
                            break;
                        case BackgroundType.GameScene:
                            // 自然色调
                            color = Color.Lerp(
                                new Color(0.2f, 0.25f, 0.2f), 
                                new Color(0.15f, 0.2f, 0.25f), 
                                t
                            );
                            break;
                        default:
                            color = Color.gray;
                            break;
                    }
                    
                    // 添加噪声
                    float noise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f) * 0.1f;
                    color.r += noise;
                    color.g += noise;
                    color.b += noise;
                    
                    pixels[y * 512 + x] = color;
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f));
            backgroundImage.sprite = sprite;
        }
        
        /// <summary>
        /// 创建渐变遮罩层
        /// </summary>
        private void CreateGradientOverlay()
        {
            GameObject overlayObj = new GameObject("GradientOverlay");
            overlayObj.transform.SetParent(transform, false);
            
            gradientOverlay = overlayObj.AddComponent<Image>();
            
            // 创建渐变纹理
            Texture2D gradientTexture = new Texture2D(1, 256);
            Color[] pixels = new Color[256];
            
            for (int i = 0; i < 256; i++)
            {
                float t = (float)i / 255f;
                pixels[i] = Color.Lerp(gradientBottomColor, gradientTopColor, t);
            }
            
            gradientTexture.SetPixels(pixels);
            gradientTexture.Apply();
            
            Sprite gradientSprite = Sprite.Create(gradientTexture, new Rect(0, 0, 1, 256), new Vector2(0.5f, 0.5f));
            gradientOverlay.sprite = gradientSprite;
            
            // 设置铺满
            RectTransform overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.sizeDelta = Vector2.zero;
            overlayRect.anchoredPosition = Vector2.zero;
        }
        
        /// <summary>
        /// 创建粒子效果（飘落的花瓣、烟雾等）
        /// </summary>
        private void CreateParticleEffect()
        {
            GameObject particleObj = new GameObject("BackgroundParticles");
            particleObj.transform.SetParent(transform, false);
            
            // 如果没有预制体，创建简单的粒子系统
            ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
            
            var main = ps.main;
            main.startSize = 0.1f;
            main.startSpeed = 1f;
            main.startLifetime = 10f;
            main.maxParticles = particleCount;
            
            var emission = ps.emission;
            emission.rateOverTime = particleCount / 10f;
            
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Rectangle;
            shape.scale = new Vector3(20, 1, 1);
            
            // 设置颜色（金色粒子，像花瓣或萤火虫）
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(1f, 0.8f, 0.3f), 0f),
                    new GradientColorKey(new Color(1f, 0.6f, 0.2f), 1f)
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.5f, 0.3f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = gradient;
            
            // 设置渲染器
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }
        
        /// <summary>
        /// 更新视差效果
        /// </summary>
        private void UpdateParallax()
        {
            if (backgroundRect == null) return;
            
            // 根据鼠标位置创建轻微的视差移动
            Vector3 mousePos = Input.mousePosition;
            float screenCenterX = Screen.width / 2f;
            float screenCenterY = Screen.height / 2f;
            
            float offsetX = (mousePos.x - screenCenterX) / screenCenterX * parallaxSpeed;
            float offsetY = (mousePos.y - screenCenterY) / screenCenterY * parallaxSpeed;
            
            parallaxOffset = Vector2.Lerp(parallaxOffset, new Vector2(offsetX, offsetY), Time.deltaTime * 2f);
            backgroundRect.anchoredPosition = parallaxOffset;
        }
        
        /// <summary>
        /// 更新脉冲效果
        /// </summary>
        private void UpdatePulse()
        {
            if (backgroundRect == null) return;
            
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            backgroundRect.localScale = Vector3.one * pulse;
        }
        
        /// <summary>
        /// 设置背景图片
        /// </summary>
        public void SetBackground(Sprite sprite)
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = sprite;
            }
        }
        
        /// <summary>
        /// 淡入背景
        /// </summary>
        public IEnumerator FadeIn(float duration = 1f)
        {
            if (backgroundImage == null) yield break;
            
            Color color = backgroundImage.color;
            color.a = 0;
            backgroundImage.color = color;
            
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, elapsed / duration);
                backgroundImage.color = color;
                yield return null;
            }
            
            color.a = 1;
            backgroundImage.color = color;
        }
    }
}

