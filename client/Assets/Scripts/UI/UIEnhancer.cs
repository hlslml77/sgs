using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI视觉增强组件
/// 为UI元素添加圆角、阴影、发光等视觉效果
/// </summary>
[ExecuteInEditMode]
public class UIEnhancer : MonoBehaviour
{
    [Header("圆角设置")]
    public bool enableRoundedCorners = true;
    public float cornerRadius = 10f;

    [Header("阴影设置")]
    public bool enableShadow = true;
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    public Vector2 shadowOffset = new Vector2(3, -3);
    public float shadowBlur = 5f;

    [Header("发光设置")]
    public bool enableGlow = false;
    public Color glowColor = new Color(1, 1, 1, 0.5f);
    public float glowSize = 10f;

    [Header("边框设置")]
    public bool enableBorder = false;
    public Color borderColor = new Color(1, 1, 1, 0.3f);
    public float borderWidth = 2f;

    private Image image;
    private GameObject shadowObject;
    private GameObject glowObject;
    private GameObject borderObject;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        ApplyEnhancements();
    }

    void OnValidate()
    {
        if (Application.isPlaying || !enabled)
            return;

        ApplyEnhancements();
    }

    public void ApplyEnhancements()
    {
        if (image == null)
            image = GetComponent<Image>();

        if (image == null)
            return;

        // 清理旧的效果对象
        CleanupEffects();

        // 应用新效果
        if (enableShadow)
            CreateShadow();

        if (enableGlow)
            CreateGlow();

        if (enableBorder)
            CreateBorder();
    }

    void CreateShadow()
    {
        if (shadowObject != null)
            return;

        shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform, false);
        shadowObject.transform.SetAsFirstSibling();

        var shadowRect = shadowObject.AddComponent<RectTransform>();
        var myRect = GetComponent<RectTransform>();
        
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.sizeDelta = new Vector2(shadowBlur * 2, shadowBlur * 2);
        shadowRect.anchoredPosition = shadowOffset;

        var shadowImage = shadowObject.AddComponent<Image>();
        shadowImage.color = shadowColor;
        shadowImage.raycastTarget = false;
    }

    void CreateGlow()
    {
        if (glowObject != null)
            return;

        glowObject = new GameObject("Glow");
        glowObject.transform.SetParent(transform, false);
        glowObject.transform.SetAsFirstSibling();

        var glowRect = glowObject.AddComponent<RectTransform>();
        glowRect.anchorMin = Vector2.zero;
        glowRect.anchorMax = Vector2.one;
        glowRect.sizeDelta = new Vector2(glowSize * 2, glowSize * 2);
        glowRect.anchoredPosition = Vector2.zero;

        var glowImage = glowObject.AddComponent<Image>();
        glowImage.color = glowColor;
        glowImage.raycastTarget = false;
    }

    void CreateBorder()
    {
        if (borderObject != null)
            return;

        borderObject = new GameObject("Border");
        borderObject.transform.SetParent(transform, false);

        var borderRect = borderObject.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = Vector2.zero;
        borderRect.anchoredPosition = Vector2.zero;

        var borderOutline = borderObject.AddComponent<Outline>();
        borderOutline.effectColor = borderColor;
        borderOutline.effectDistance = new Vector2(borderWidth, -borderWidth);
    }

    void CleanupEffects()
    {
        if (shadowObject != null)
        {
            if (Application.isPlaying)
                Destroy(shadowObject);
            else
                DestroyImmediate(shadowObject);
            shadowObject = null;
        }

        if (glowObject != null)
        {
            if (Application.isPlaying)
                Destroy(glowObject);
            else
                DestroyImmediate(glowObject);
            glowObject = null;
        }

        if (borderObject != null)
        {
            if (Application.isPlaying)
                Destroy(borderObject);
            else
                DestroyImmediate(borderObject);
            borderObject = null;
        }
    }

    void OnDisable()
    {
        CleanupEffects();
    }

    void OnDestroy()
    {
        CleanupEffects();
    }
}

