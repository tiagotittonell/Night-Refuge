using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra el nivel de sospecha actual al jugador.
/// Se auto-vincula a un texto llamado "SuspicionText" en DynamicGameplayLayer.
/// Si no existe, crea uno como fallback bajo DynamicGameplayLayer.
/// </summary>
public class SuspicionUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text suspicionTmpText;
    [SerializeField] private Text suspicionLegacyText;
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

        GameObject textObject = new GameObject("SuspicionText", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0.4f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(15f, -10f);
        rect.sizeDelta = new Vector2(0f, 35f);

        Text legacyText = textObject.GetComponent<Text>();
        legacyText.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 18);
        legacyText.fontSize = 18;
        legacyText.color = colorUnknown;
        legacyText.alignment = TextAnchor.MiddleLeft;
        legacyText.raycastTarget = false;

        suspicionLegacyText = legacyText;
    }
}
