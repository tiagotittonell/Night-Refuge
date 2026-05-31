using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel UI que muestra el registro de visitantes procesados.
/// Se puede abrir/cerrar durante el juego para revisar decisiones pasadas.
/// </summary>
public class VisitorLogUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text logTmpText;
    [SerializeField] private Text logLegacyText;
    [SerializeField] private Button toggleButton;
    [SerializeField] private VisitorLog visitorLog;

    private bool isVisible;

    private void Awake()
    {
        EnsureBindings();

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(Toggle);
        }

        Hide();
    }

    public void Toggle()
    {
        if (isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        EnsureBindings();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            panelRoot.transform.SetAsLastSibling();
        }

        UpdateContent();
        isVisible = true;
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        isVisible = false;
    }

    private void UpdateContent()
    {
        if (visitorLog == null)
        {
            SetText("[ Sin registros ]");
            return;
        }

        if (visitorLog.TotalVisitors == 0)
        {
            SetText("REGISTRO DE VISITANTES\n\n[ Aún no se han procesado visitantes ]");
            return;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("REGISTRO DE VISITANTES");
        builder.AppendLine("═══════════════════════════════");

        int currentNight = -1;
        foreach (VisitorRecord record in visitorLog.Records)
        {
            if (record.nightNumber != currentNight)
            {
                currentNight = record.nightNumber;
                builder.AppendLine();
                builder.AppendLine($"── Noche {currentNight} ──");
            }

            string decision = record.wasAccepted ? "PERMITIDO" : "RECHAZADO";
            string suspicion = GetSuspicionLabel(record.suspicionAtDecision);

            builder.AppendLine();
            builder.AppendLine($"▪ {record.visitorName} [{record.timestamp}]");
            builder.AppendLine($"  Decisión: {decision}");
            builder.AppendLine($"  Sospecha: {suspicion}");

            if (record.observedClues.Count > 0)
            {
                builder.Append("  Pistas: ");
                builder.AppendLine(string.Join(", ", record.observedClues));
            }

            if (record.questionsAsked.Count > 0)
            {
                builder.AppendLine($"  Preguntas: {record.questionsAsked.Count}");
            }
        }

        SetText(builder.ToString());
    }

    private string GetSuspicionLabel(SuspicionLevel level)
    {
        switch (level)
        {
            case SuspicionLevel.Low:
                return "Baja";
            case SuspicionLevel.Medium:
                return "Media";
            case SuspicionLevel.High:
                return "Alta";
            case SuspicionLevel.Unknown:
            default:
                return "Desconocida";
        }
    }

    private void SetText(string value)
    {
        if (logTmpText != null)
        {
            logTmpText.text = value;
        }

        if (logLegacyText != null)
        {
            logLegacyText.text = value;
        }
    }

    private void EnsureBindings()
    {
        if (visitorLog == null)
        {
            visitorLog = Object.FindFirstObjectByType<VisitorLog>();
        }

        if (panelRoot == null)
        {
            GameObject existing = UISpriteLoader.FindIncludingInactive("VisitorLogPanel");
            if (existing != null)
            {
                panelRoot = existing;
            }
            else
            {
                CreateFallbackPanel();
            }
        }

        if (logTmpText == null && logLegacyText == null && panelRoot != null)
        {
            logTmpText = panelRoot.GetComponentInChildren<TMP_Text>(true);
            logLegacyText = panelRoot.GetComponentInChildren<Text>(true);
        }

        if (toggleButton == null)
        {
            GameObject btn = UISpriteLoader.FindIncludingInactive("VisitorLogToggleButton");
            if (btn != null)
            {
                toggleButton = btn.GetComponent<Button>();
            }
            else
            {
                CreateToggleButton();
            }

            if (toggleButton != null)
            {
                toggleButton.onClick.RemoveListener(Toggle);
                toggleButton.onClick.AddListener(Toggle);
            }
        }
    }

    private void CreateToggleButton()
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform parent = dynamicLayer != null ? dynamicLayer.transform
            : canvas != null ? canvas.transform : transform;

        GameObject btnObj = new GameObject("VisitorLogToggleButton", typeof(RectTransform), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(1f, 1f);
        btnRect.anchorMax = new Vector2(1f, 1f);
        btnRect.pivot = new Vector2(1f, 1f);
        btnRect.anchoredPosition = new Vector2(-15f, -60f);
        btnRect.sizeDelta = new Vector2(160f, 40f);

        Image btnImage = btnObj.GetComponent<Image>();
        Sprite btnSprite = UISpriteLoader.ButtonNormal;
        if (btnSprite != null)
        {
            btnImage.sprite = btnSprite;
            btnImage.type = Image.Type.Sliced;
            btnImage.color = Color.white;
        }
        else
        {
            btnImage.color = new Color(0.12f, 0.10f, 0.08f, 0.9f);
        }

        toggleButton = btnObj.GetComponent<Button>();

        // Hover state
        Sprite hoverSprite = UISpriteLoader.ButtonHover;
        if (btnSprite != null && hoverSprite != null)
        {
            toggleButton.transition = Selectable.Transition.SpriteSwap;
            SpriteState spriteState = new SpriteState
            {
                highlightedSprite = hoverSprite,
                pressedSprite = hoverSprite
            };
            toggleButton.spriteState = spriteState;
        }

        GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelObj.transform.SetParent(btnObj.transform, false);

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text label = labelObj.GetComponent<Text>();
        label.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 16);
        label.fontSize = 16;
        label.color = new Color(0.78f, 0.70f, 0.58f, 1f);
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;
        label.text = "\u270d REGISTRO";
    }

    private void CreateFallbackPanel()
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform parent = dynamicLayer != null ? dynamicLayer.transform
            : canvas != null ? canvas.transform : transform;

        // Panel root
        GameObject panel = new GameObject("VisitorLogPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.1f);
        panelRect.anchorMax = new Vector2(0.85f, 0.9f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        Sprite logBg = UISpriteLoader.LogBackground;
        if (logBg != null)
        {
            panelImage.sprite = logBg;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;
        }
        else
        {
            panelImage.color = new Color(0.04f, 0.035f, 0.03f, 0.95f);
        }

        panelRoot = panel;

        // Scrollable text
        GameObject textObj = new GameObject("LogText", typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0.08f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = new Vector2(24f, 0f);
        textRect.offsetMax = new Vector2(-24f, -16f);

        Text logText = textObj.GetComponent<Text>();
        logText.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 15);
        logText.fontSize = 15;
        logText.color = new Color(0.70f, 0.63f, 0.52f, 1f);
        logText.alignment = TextAnchor.UpperLeft;
        logText.raycastTarget = false;
        logText.verticalOverflow = VerticalWrapMode.Overflow;

        logLegacyText = logText;

        // Close button
        GameObject closeObj = new GameObject("CloseBtn", typeof(RectTransform), typeof(Image), typeof(Button));
        closeObj.transform.SetParent(panel.transform, false);

        RectTransform closeRect = closeObj.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-8f, -8f);
        closeRect.sizeDelta = new Vector2(36f, 36f);

        Image closeImage = closeObj.GetComponent<Image>();
        Sprite btnSprite = UISpriteLoader.ButtonNormal;
        if (btnSprite != null)
        {
            closeImage.sprite = btnSprite;
            closeImage.type = Image.Type.Sliced;
            closeImage.color = Color.white;
        }
        else
        {
            closeImage.color = new Color(0.3f, 0.15f, 0.1f, 0.9f);
        }

        Button closeButton = closeObj.GetComponent<Button>();
        closeButton.onClick.AddListener(Hide);

        GameObject closeLabelObj = new GameObject("X", typeof(RectTransform), typeof(Text));
        closeLabelObj.transform.SetParent(closeObj.transform, false);

        RectTransform closeLabelRect = closeLabelObj.GetComponent<RectTransform>();
        closeLabelRect.anchorMin = Vector2.zero;
        closeLabelRect.anchorMax = Vector2.one;
        closeLabelRect.offsetMin = Vector2.zero;
        closeLabelRect.offsetMax = Vector2.zero;

        Text closeLabel = closeLabelObj.GetComponent<Text>();
        closeLabel.font = Font.CreateDynamicFontFromOSFont(new[] { "Arial" }, 20);
        closeLabel.fontSize = 20;
        closeLabel.color = new Color(0.9f, 0.7f, 0.6f, 1f);
        closeLabel.alignment = TextAnchor.MiddleCenter;
        closeLabel.raycastTarget = false;
        closeLabel.text = "X";
    }
}
