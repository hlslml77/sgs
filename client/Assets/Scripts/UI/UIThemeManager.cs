using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// UI主题管理器 - 统一管理UI颜色、字体、效果
    /// </summary>
    public class UIThemeManager : MonoBehaviour
    {
        public static UIThemeManager Instance { get; private set; }
        
        [Header("当前主题")]
        [SerializeField] private UITheme currentTheme = UITheme.ThreeKingdoms;
        
        [Header("三国主题配置")]
        [SerializeField] private ThemeConfig threeKingdomsTheme = new ThemeConfig
        {
            primaryColor = new Color(0.3f, 0.1f, 0.1f),      // 深红
            secondaryColor = new Color(0.8f, 0.6f, 0.2f),    // 金色
            accentColor = new Color(1f, 0.84f, 0f),          // 亮金
            textColor = new Color(1f, 0.95f, 0.9f),          // 淡金白
            buttonNormalColor = new Color(0.4f, 0.2f, 0.1f),
            buttonHoverColor = new Color(0.8f, 0.6f, 0.3f),
            buttonPressColor = new Color(0.6f, 0.4f, 0.2f),
            backgroundTopColor = new Color(0.3f, 0.1f, 0.1f, 0.9f),
            backgroundBottomColor = new Color(0.2f, 0.15f, 0.05f, 0.9f)
        };
        
        [Header("现代主题配置")]
        [SerializeField] private ThemeConfig modernTheme = new ThemeConfig
        {
            primaryColor = new Color(0.1f, 0.1f, 0.3f),
            secondaryColor = new Color(0.2f, 0.5f, 0.8f),
            accentColor = new Color(0f, 0.8f, 1f),
            textColor = Color.white,
            buttonNormalColor = new Color(0.2f, 0.2f, 0.4f),
            buttonHoverColor = new Color(0.3f, 0.5f, 0.9f),
            buttonPressColor = new Color(0.1f, 0.3f, 0.7f),
            backgroundTopColor = new Color(0.1f, 0.1f, 0.3f, 0.9f),
            backgroundBottomColor = new Color(0.2f, 0.1f, 0.3f, 0.9f)
        };
        
        [Header("自然主题配置")]
        [SerializeField] private ThemeConfig natureTheme = new ThemeConfig
        {
            primaryColor = new Color(0.1f, 0.3f, 0.2f),
            secondaryColor = new Color(0.4f, 0.7f, 0.3f),
            accentColor = new Color(0f, 1f, 0.5f),
            textColor = new Color(0.9f, 1f, 0.9f),
            buttonNormalColor = new Color(0.2f, 0.4f, 0.2f),
            buttonHoverColor = new Color(0.4f, 0.8f, 0.3f),
            buttonPressColor = new Color(0.3f, 0.6f, 0.2f),
            backgroundTopColor = new Color(0.1f, 0.2f, 0.15f, 0.9f),
            backgroundBottomColor = new Color(0.05f, 0.15f, 0.2f, 0.9f)
        };
        
        public enum UITheme
        {
            ThreeKingdoms,  // 三国主题
            Modern,         // 现代主题
            Nature          // 自然主题
        }
        
        [System.Serializable]
        public class ThemeConfig
        {
            public Color primaryColor;
            public Color secondaryColor;
            public Color accentColor;
            public Color textColor;
            public Color buttonNormalColor;
            public Color buttonHoverColor;
            public Color buttonPressColor;
            public Color backgroundTopColor;
            public Color backgroundBottomColor;
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            ApplyTheme(currentTheme);
        }
        
        /// <summary>
        /// 应用主题
        /// </summary>
        public void ApplyTheme(UITheme theme)
        {
            currentTheme = theme;
            ThemeConfig config = GetThemeConfig(theme);
            
            // 应用到所有UI元素
            ApplyToAllButtons(config);
            ApplyToAllTexts(config);
            ApplyToAllBackgrounds(config);
            
            Debug.Log($"已应用主题: {theme}");
        }
        
        /// <summary>
        /// 获取主题配置
        /// </summary>
        public ThemeConfig GetThemeConfig(UITheme theme)
        {
            switch (theme)
            {
                case UITheme.ThreeKingdoms:
                    return threeKingdomsTheme;
                case UITheme.Modern:
                    return modernTheme;
                case UITheme.Nature:
                    return natureTheme;
                default:
                    return threeKingdomsTheme;
            }
        }
        
        /// <summary>
        /// 获取当前主题配置
        /// </summary>
        public ThemeConfig GetCurrentThemeConfig()
        {
            return GetThemeConfig(currentTheme);
        }
        
        /// <summary>
        /// 应用到所有按钮
        /// </summary>
        private void ApplyToAllButtons(ThemeConfig config)
        {
            Button[] buttons = FindObjectsOfType<Button>(true);
            
            foreach (Button button in buttons)
            {
                // 设置按钮颜色
                ColorBlock colors = button.colors;
                colors.normalColor = config.buttonNormalColor;
                colors.highlightedColor = config.buttonHoverColor;
                colors.pressedColor = config.buttonPressColor;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                button.colors = colors;
                
                // 设置按钮上的文字颜色
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.color = config.textColor;
                }
                
                // 更新ButtonEnhancer配置
                ButtonEnhancer enhancer = button.GetComponent<ButtonEnhancer>();
                if (enhancer != null)
                {
                    // 通过反射或公共方法更新颜色
                }
            }
        }
        
        /// <summary>
        /// 应用到所有文本
        /// </summary>
        private void ApplyToAllTexts(ThemeConfig config)
        {
            TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>(true);
            
            foreach (TextMeshProUGUI text in texts)
            {
                // 跳过按钮内的文本（已在按钮中处理）
                if (text.GetComponentInParent<Button>() == null)
                {
                    text.color = config.textColor;
                }
            }
        }
        
        /// <summary>
        /// 应用到所有背景
        /// </summary>
        private void ApplyToAllBackgrounds(ThemeConfig config)
        {
            BackgroundManager[] backgrounds = FindObjectsOfType<BackgroundManager>(true);
            
            foreach (BackgroundManager bg in backgrounds)
            {
                // 更新背景渐变颜色
                // 需要在BackgroundManager中添加公共方法来设置颜色
            }
        }
        
        /// <summary>
        /// 切换到下一个主题
        /// </summary>
        public void NextTheme()
        {
            int nextIndex = ((int)currentTheme + 1) % System.Enum.GetValues(typeof(UITheme)).Length;
            ApplyTheme((UITheme)nextIndex);
        }
        
        /// <summary>
        /// 获取主题颜色
        /// </summary>
        public Color GetColor(ColorType colorType)
        {
            ThemeConfig config = GetCurrentThemeConfig();
            
            switch (colorType)
            {
                case ColorType.Primary:
                    return config.primaryColor;
                case ColorType.Secondary:
                    return config.secondaryColor;
                case ColorType.Accent:
                    return config.accentColor;
                case ColorType.Text:
                    return config.textColor;
                default:
                    return Color.white;
            }
        }
        
        public enum ColorType
        {
            Primary,
            Secondary,
            Accent,
            Text
        }
    }
}

