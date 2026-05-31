using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra el panel de evento correcto según el tipo de InterVisitorEvent.
/// Cada tipo de evento tiene su propio panel en escena (con su fondo específico).
/// El runtime solo activa/desactiva el panel que corresponde.
/// Los paneles se nombran: InterEvent_NoiseInDucts, InterEvent_Blackout, etc.
/// </summary>
public class InterEventUI : MonoBehaviour
{
    [Header("Estilo")]
    [SerializeField] private Color eventColor = new Color(0.6f, 0.55f, 0.45f, 1f);
    [SerializeField] private Color dangerEventColor = new Color(0.8f, 0.3f, 0.2f, 1f);

    private InterEventSystem interEventSystem;
    private Coroutine displayCoroutine;
    private GameObject activePanel;

    // Maps InterEventType → scene object name
    private static readonly Dictionary<InterEventType, string> PanelNames = new Dictionary<InterEventType, string>
    {
        { InterEventType.NoiseInDucts,      "InterEvent_NoiseInDucts" },
        { InterEventType.ShelterProtest,    "InterEvent_ShelterProtest" },
        { InterEventType.PartialBlackout,   "InterEvent_Blackout" },
        { InterEventType.IntermittentRadio, "InterEvent_Radio" },
        { InterEventType.InteriorDoorKnock, "InterEvent_DoorKnock" },
        { InterEventType.AcceptedPersonInfo,"InterEvent_PersonInfo" },
        { InterEventType.FalseRumor,        "InterEvent_FalseRumor" },
        { InterEventType.DistantScream,     "InterEvent_DistantScream" },
        { InterEventType.SilenceBreak,      "InterEvent_SilenceBreak" },
    };

    // Generic fallback panel used when no specific panel exists
    private const string GenericPanelName = "InterEvent_Generic";

    private void Awake()
    {
        EnsureBindings();
        HideAll();
    }

    private void Start()
    {
        EnsureBindings();
    }

    private void OnEnable()
    {
        if (interEventSystem != null)
        {
            interEventSystem.EventTriggered += OnEventTriggered;
            interEventSystem.EventDismissed += OnEventDismissed;
        }
    }

    private void OnDisable()
    {
        if (interEventSystem != null)
        {
            interEventSystem.EventTriggered -= OnEventTriggered;
            interEventSystem.EventDismissed -= OnEventDismissed;
        }
    }

    private void OnEventTriggered(InterVisitorEvent interEvent)
    {
        ShowEvent(interEvent);
    }

    private void OnEventDismissed()
    {
        HideAll();
    }

    public void ShowEvent(InterVisitorEvent interEvent)
    {
        EnsureBindings();
        HideAll();

        // Find the specific panel for this event type, fall back to generic
        string panelName;
        if (!PanelNames.TryGetValue(interEvent.eventType, out panelName))
        {
            panelName = GenericPanelName;
        }

        GameObject panel = UISpriteLoader.FindIncludingInactive(panelName)
                        ?? UISpriteLoader.FindIncludingInactive(GenericPanelName);

        if (panel == null)
        {
            panel = CreateFallbackPanel(panelName);
        }

        panel.SetActive(true);
        activePanel = panel;

        // Update text inside the panel
        bool isDangerous = interEvent.securityChange < 0 || interEvent.moraleChange < -1;
        Color color = isDangerous ? dangerEventColor : eventColor;

        Text legacyText = panel.GetComponentInChildren<Text>(true);
        TMP_Text tmpText = panel.GetComponentInChildren<TMP_Text>(true);

        if (legacyText != null)
        {
            legacyText.text = interEvent.narrativeText;
            legacyText.color = color;
        }
        if (tmpText != null)
        {
            tmpText.text = interEvent.narrativeText;
            tmpText.color = color;
        }

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        float duration = interEventSystem != null ? interEventSystem.EventDisplayDuration : 4.5f;
        displayCoroutine = StartCoroutine(AutoHide(duration));
    }

    private IEnumerator AutoHide(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideAll();
        displayCoroutine = null;
    }

    public void HideAll()
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
            activePanel = null;
        }

        // Also ensure all known event panels are hidden
        foreach (string name in PanelNames.Values)
        {
            GameObject panel = UISpriteLoader.FindIncludingInactive(name);
            if (panel != null) panel.SetActive(false);
        }

        GameObject generic = UISpriteLoader.FindIncludingInactive(GenericPanelName);
        if (generic != null) generic.SetActive(false);
    }

    // Legacy method name for compatibility
    public void HidePanel() => HideAll();

    private void EnsureBindings()
    {
        if (interEventSystem == null)
        {
            interEventSystem = Object.FindFirstObjectByType<InterEventSystem>();
        }

        if (interEventSystem != null)
        {
            interEventSystem.EventTriggered -= OnEventTriggered;
            interEventSystem.EventTriggered += OnEventTriggered;
            interEventSystem.EventDismissed -= OnEventDismissed;
            interEventSystem.EventDismissed += OnEventDismissed;
        }
    }

    private GameObject CreateFallbackPanel(string name)
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform parent = dynamicLayer != null ? dynamicLayer.transform
            : canvas != null ? canvas.transform : transform;

        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.35f);
        panelRect.anchorMax = new Vector2(0.85f, 0.65f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.05f, 0.04f, 0.03f, 0.92f);
        panelImage.raycastTarget = false;

        GameObject textObject = new GameObject("EventText", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(24f, 16f);
        textRect.offsetMax = new Vector2(-24f, -16f);

        Text text = textObject.GetComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 20);
        text.fontSize = 20;
        text.fontStyle = FontStyle.Italic;
        text.color = eventColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        return panel;
    }
}

    [Header("Referencias")]
    [SerializeField] private TMP_Text eventTmpText;
    [SerializeField] private Text eventLegacyText;
    [SerializeField] private GameObject panelRoot;

    [Header("Estilo")]
    [SerializeField] private Color eventColor = new Color(0.6f, 0.55f, 0.45f, 1f);
    [SerializeField] private Color dangerEventColor = new Color(0.8f, 0.3f, 0.2f, 1f);

    private InterEventSystem interEventSystem;
    private Coroutine displayCoroutine;

    private void Awake()
    {
        EnsureBindings();
        HidePanel();
    }

    private void OnEnable()
    {
        if (interEventSystem != null)
        {
            interEventSystem.EventTriggered += OnEventTriggered;
            interEventSystem.EventDismissed += OnEventDismissed;
        }
    }

    private void OnDisable()
    {
        if (interEventSystem != null)
        {
            interEventSystem.EventTriggered -= OnEventTriggered;
            interEventSystem.EventDismissed -= OnEventDismissed;
        }
    }

    private void OnEventTriggered(InterVisitorEvent interEvent)
    {
        ShowEvent(interEvent);
    }

    private void OnEventDismissed()
    {
        HidePanel();
    }

    public void ShowEvent(InterVisitorEvent interEvent)
    {
        EnsureBindings();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);

            // Apply event-specific background
            Image panelImage = panelRoot.GetComponent<Image>();
            if (panelImage != null)
            {
                Sprite eventBg = UISpriteLoader.GetEventBackground(interEvent.eventType);
                if (eventBg != null)
                {
                    panelImage.sprite = eventBg;
                    panelImage.type = Image.Type.Simple;
                    panelImage.preserveAspect = false;
                    panelImage.color = new Color(1f, 1f, 1f, 0.92f);
                }
                else
                {
                    panelImage.sprite = null;
                    panelImage.color = new Color(0.05f, 0.04f, 0.03f, 0.92f);
                }
            }
        }

        bool isDangerous = interEvent.securityChange < 0 || interEvent.moraleChange < -1;
        Color color = isDangerous ? dangerEventColor : eventColor;

        if (eventTmpText != null)
        {
            eventTmpText.text = interEvent.narrativeText;
            eventTmpText.color = color;
        }

        if (eventLegacyText != null)
        {
            eventLegacyText.text = interEvent.narrativeText;
            eventLegacyText.color = color;
        }

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        float duration = interEventSystem != null ? interEventSystem.EventDisplayDuration : 2.5f;
        displayCoroutine = StartCoroutine(AutoHide(duration));
    }

    private IEnumerator AutoHide(float duration)
    {
        yield return new WaitForSeconds(duration);
        HidePanel();
        displayCoroutine = null;
    }

    public void HidePanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
        else
        {
            SetText("");
        }
    }

    private void SetText(string value)
    {
        if (eventTmpText != null)
        {
            eventTmpText.text = value;
        }

        if (eventLegacyText != null)
        {
            eventLegacyText.text = value;
        }
    }

    private void EnsureBindings()
    {
        if (interEventSystem == null)
        {
            interEventSystem = Object.FindFirstObjectByType<InterEventSystem>();
        }

        if (interEventSystem != null)
        {
            interEventSystem.EventTriggered -= OnEventTriggered;
            interEventSystem.EventTriggered += OnEventTriggered;
            interEventSystem.EventDismissed -= OnEventDismissed;
            interEventSystem.EventDismissed += OnEventDismissed;
        }

        if (eventTmpText == null && eventLegacyText == null)
        {
            GameObject existing = UISpriteLoader.FindIncludingInactive("InterEventText");
            if (existing != null)
            {
                eventTmpText = existing.GetComponent<TMP_Text>();
                eventLegacyText = existing.GetComponentInChildren<Text>(true);
                panelRoot = existing;
            }
            else
            {
                CreateFallbackUI();
            }
        }
    }

    private void CreateFallbackUI()
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        Transform parent = dynamicLayer != null ? dynamicLayer.transform : transform;

        GameObject panel = new GameObject("InterEventText", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.35f);
        panelRect.anchorMax = new Vector2(0.85f, 0.65f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.05f, 0.04f, 0.03f, 0.92f);
        panelImage.raycastTarget = false;

        panelRoot = panel;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(24f, 16f);
        textRect.offsetMax = new Vector2(-24f, -16f);

        Text text = textObject.GetComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 20);
        text.fontSize = 20;
        text.fontStyle = FontStyle.Italic;
        text.color = eventColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        eventLegacyText = text;
    }
}
