using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra el nivel de sospecha actual al jugador.
/// Se auto-vincula a un texto llamado "SuspicionText" en DynamicGameplayLayer.
/// Si no existe, crea uno como fallback bajo DynamicGameplayLayer.
/// Incluye icono visual del nivel de sospecha.
/// </summary>
public class SuspicionUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text suspicionTmpText;
    [SerializeField] private Text suspicionLegacyText;
    [SerializeField] private Image suspicionIcon;
    [SerializeField] private SuspicionSystem suspicionSystem;

    [Header("Colores")]
    [SerializeField] private Color colorUnknown = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    [SerializeField] private Color colorLow = new Color(0.4f, 0.7f, 0.4f, 1f);
    [SerializeField] private Color colorMedium = new Color(0.85f, 0.7f, 0.2f, 1f);
    [SerializeField] private Color colorHigh = new Color(0.85f, 0.25f, 0.2f, 1f);

    private void Awake()
    {
        EnsureBindings();
    }

    private void OnEnable()
    {
        if (suspicionSystem != null)
        {
            suspicionSystem.SuspicionChanged += OnSuspicionChanged;
        }
    }

    private void OnDisable()
    {
        if (suspicionSystem != null)
        {
            suspicionSystem.SuspicionChanged -= OnSuspicionChanged;
        }
    }

    private void Start()
    {
        EnsureBindings();
        UpdateDisplay(SuspicionLevel.Unknown);
    }

    private void OnSuspicionChanged(SuspicionLevel level)
    {
        UpdateDisplay(level);
    }

    public void UpdateDisplay(SuspicionLevel level)
    {
        string label = GetLevelLabel(level);
        Color color = GetLevelColor(level);

        if (suspicionTmpText != null)
        {
            suspicionTmpText.text = label;
            suspicionTmpText.color = color;
        }

        if (suspicionLegacyText != null)
        {
            suspicionLegacyText.text = label;
            suspicionLegacyText.color = color;
        }

        // Update icon
        if (suspicionIcon != null)
        {
            Sprite iconSprite = UISpriteLoader.GetSuspicionIcon(level);
            if (iconSprite != null)
            {
                suspicionIcon.sprite = iconSprite;
                suspicionIcon.enabled = true;
            }
            else
            {
                suspicionIcon.enabled = false;
            }
        }
    }

    private string GetLevelLabel(SuspicionLevel level)
    {
        switch (level)
        {
            case SuspicionLevel.Low:
                return "SOSPECHA: BAJA";
            case SuspicionLevel.Medium:
                return "SOSPECHA: MEDIA";
            case SuspicionLevel.High:
                return "SOSPECHA: ALTA";
            case SuspicionLevel.Unknown:
            default:
                return "SOSPECHA: ---";
        }
    }

    private Color GetLevelColor(SuspicionLevel level)
    {
        switch (level)
        {
            case SuspicionLevel.Low:
                return colorLow;
            case SuspicionLevel.Medium:
                return colorMedium;
            case SuspicionLevel.High:
                return colorHigh;
            case SuspicionLevel.Unknown:
            default:
                return colorUnknown;
        }
    }

    private void EnsureBindings()
    {
        if (suspicionSystem == null)
        {
            suspicionSystem = FindFirstObjectByType<SuspicionSystem>();
        }

        if (suspicionSystem != null)
        {
            suspicionSystem.SuspicionChanged -= OnSuspicionChanged;
            suspicionSystem.SuspicionChanged += OnSuspicionChanged;
        }

        if (suspicionTmpText == null && suspicionLegacyText == null)
        {
            GameObject existing = GameObject.Find("SuspicionText");
            if (existing != null)
            {
                suspicionTmpText = existing.GetComponent<TMP_Text>();
                suspicionLegacyText = existing.GetComponent<Text>();

                // Look for icon sibling
                Transform iconTransform = existing.transform.parent != null
                    ? existing.transform.parent.Find("SuspicionIcon")
                    : null;
                if (iconTransform != null)
                {
                    suspicionIcon = iconTransform.GetComponent<Image>();
                }
            }
            else
            {
                CreateFallbackText();
            }
        }
    }

    private void CreateFallbackText()
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        Transform parent = dynamicLayer != null ? dynamicLayer.transform : transform;

        // Container for icon + text
        GameObject container = new GameObject("SuspicionContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        container.transform.SetParent(parent, false);

        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0f, 1f);
        containerRect.anchorMax = new Vector2(0.4f, 1f);
        containerRect.pivot = new Vector2(0f, 1f);
        containerRect.anchoredPosition = new Vector2(15f, -10f);
        containerRect.sizeDelta = new Vector2(0f, 40f);

        HorizontalLayoutGroup hlg = container.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 6f;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        // Icon
        GameObject iconObj = new GameObject("SuspicionIcon", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        iconObj.transform.SetParent(container.transform, false);

        Image iconImage = iconObj.GetComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        iconImage.enabled = false;

        LayoutElement iconLayout = iconObj.GetComponent<LayoutElement>();
        iconLayout.preferredWidth = 32f;
        iconLayout.preferredHeight = 32f;

        suspicionIcon = iconImage;

        // Text
        GameObject textObject = new GameObject("SuspicionText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
        textObject.transform.SetParent(container.transform, false);

        LayoutElement textLayout = textObject.GetComponent<LayoutElement>();
        textLayout.preferredWidth = 250f;
        textLayout.preferredHeight = 35f;

        Text legacyText = textObject.GetComponent<Text>();
        legacyText.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 18);
        legacyText.fontSize = 18;
        legacyText.color = colorUnknown;
        legacyText.alignment = TextAnchor.MiddleLeft;
        legacyText.raycastTarget = false;

        suspicionLegacyText = legacyText;
    }
}
