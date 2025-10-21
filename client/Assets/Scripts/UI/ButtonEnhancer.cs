using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 按钮增强器 - 添加专业的视觉效果和音效反馈
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonEnhancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("视觉效果")]
        [SerializeField] private bool enableHoverEffect = true;
        [SerializeField] private bool enableClickEffect = true;
        [SerializeField] private bool enableScaleAnimation = true;
        
        [Header("缩放动画")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 0.95f;
        [SerializeField] private float animationSpeed = 10f;
        
        [Header("颜色效果")]
        [SerializeField] private bool useColorTint = true;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 0.9f, 0.7f);
        [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f);
        
        [Header("发光效果")]
        [SerializeField] private bool enableGlow = true;
        [SerializeField] private Image glowImage;
        [SerializeField] private float glowIntensity = 1.5f;
        
        [Header("音效")]
        [SerializeField] private bool playSound = true;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;
        
        private Button button;
        private Image buttonImage;
        private TextMeshProUGUI buttonText;
        private Vector3 originalScale;
        private Vector3 targetScale;
        private AudioSource audioSource;
        private bool isHovering = false;
        private bool isPressed = false;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
            originalScale = transform.localScale;
            targetScale = originalScale;
            
            // 创建音频源
            if (playSound)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0; // 2D音效
            }
            
            // 设置初始颜色
            if (buttonImage != null && useColorTint)
            {
                buttonImage.color = normalColor;
            }
            
            // 创建发光效果
            if (enableGlow && glowImage == null)
            {
                CreateGlowEffect();
            }
        }
        
        private void Update()
        {
            // 平滑缩放动画
            if (enableScaleAnimation)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable) return;
            
            isHovering = true;
            
            // 缩放效果
            if (enableHoverEffect)
            {
                targetScale = originalScale * hoverScale;
            }
            
            // 颜色效果
            if (buttonImage != null && useColorTint)
            {
                buttonImage.color = hoverColor;
            }
            
            // 文字效果
            if (buttonText != null)
            {
                StartCoroutine(PulseText());
            }
            
            // 发光效果
            if (glowImage != null)
            {
                glowImage.gameObject.SetActive(true);
                StartCoroutine(PulseGlow());
            }
            
            // 音效
            if (playSound && hoverSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            
            if (!isPressed)
            {
                // 恢复原始状态
                targetScale = originalScale;
                
                if (buttonImage != null && useColorTint)
                {
                    buttonImage.color = normalColor;
                }
                
                if (glowImage != null)
                {
                    glowImage.gameObject.SetActive(false);
                }
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable) return;
            
            isPressed = true;
            
            // 按下缩放
            if (enableClickEffect)
            {
                targetScale = originalScale * clickScale;
            }
            
            // 按下颜色
            if (buttonImage != null && useColorTint)
            {
                buttonImage.color = pressedColor;
            }
            
            // 音效
            if (playSound && clickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            
            if (isHovering)
            {
                targetScale = originalScale * hoverScale;
                if (buttonImage != null && useColorTint)
                {
                    buttonImage.color = hoverColor;
                }
            }
            else
            {
                targetScale = originalScale;
                if (buttonImage != null && useColorTint)
                {
                    buttonImage.color = normalColor;
                }
            }
        }
        
        /// <summary>
        /// 创建发光效果
        /// </summary>
        private void CreateGlowEffect()
        {
            GameObject glowObj = new GameObject("Glow");
            glowObj.transform.SetParent(transform, false);
            glowObj.transform.SetAsFirstSibling(); // 放在最底层
            
            glowImage = glowObj.AddComponent<Image>();
            glowImage.color = new Color(1f, 0.9f, 0.5f, 0.5f);
            
            // 设置为稍大的尺寸
            RectTransform glowRect = glowObj.GetComponent<RectTransform>();
            RectTransform buttonRect = GetComponent<RectTransform>();
            
            glowRect.anchorMin = new Vector2(0, 0);
            glowRect.anchorMax = new Vector2(1, 1);
            glowRect.sizeDelta = new Vector2(20, 20); // 比按钮大一圈
            glowRect.anchoredPosition = Vector2.zero;
            
            glowObj.SetActive(false);
        }
        
        /// <summary>
        /// 文字脉冲效果
        /// </summary>
        private IEnumerator PulseText()
        {
            if (buttonText == null) yield break;
            
            float originalSize = buttonText.fontSize;
            float targetSize = originalSize * 1.05f;
            float elapsed = 0;
            float duration = 0.2f;
            
            while (elapsed < duration && isHovering)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                buttonText.fontSize = Mathf.Lerp(originalSize, targetSize, t);
                yield return null;
            }
            
            // 恢复
            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                buttonText.fontSize = Mathf.Lerp(targetSize, originalSize, t);
                yield return null;
            }
            
            buttonText.fontSize = originalSize;
        }
        
        /// <summary>
        /// 发光脉冲效果
        /// </summary>
        private IEnumerator PulseGlow()
        {
            if (glowImage == null) yield break;
            
            while (isHovering && glowImage.gameObject.activeSelf)
            {
                // 淡入
                for (float t = 0; t < 1; t += Time.deltaTime * 2)
                {
                    if (!isHovering) break;
                    Color color = glowImage.color;
                    color.a = Mathf.Lerp(0.3f, 0.7f, t);
                    glowImage.color = color;
                    yield return null;
                }
                
                // 淡出
                for (float t = 0; t < 1; t += Time.deltaTime * 2)
                {
                    if (!isHovering) break;
                    Color color = glowImage.color;
                    color.a = Mathf.Lerp(0.7f, 0.3f, t);
                    glowImage.color = color;
                    yield return null;
                }
            }
        }
        
        private void OnDisable()
        {
            // 重置状态
            transform.localScale = originalScale;
            isHovering = false;
            isPressed = false;
        }
    }
}

