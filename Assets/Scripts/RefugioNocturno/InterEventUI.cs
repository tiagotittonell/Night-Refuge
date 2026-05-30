using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra el texto narrativo de un evento entre visitantes.
/// Aparece brevemente y luego se desvanece.
/// </summary>
public class InterEventUI : MonoBehaviour
{
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
