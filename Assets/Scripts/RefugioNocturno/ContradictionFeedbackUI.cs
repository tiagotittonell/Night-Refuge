using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra feedback visual breve cuando una respuesta tiene tags sospechosos.
/// Se integra con QuestionUI.QuestionAnswered para mostrar indicadores sutiles.
/// </summary>
public class ContradictionFeedbackUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text feedbackTmpText;
    [SerializeField] private Text feedbackLegacyText;

    [Header("Configuración")]
    [SerializeField] private float displayDuration = 2.5f;
    [SerializeField] private Color contradictionColor = new Color(0.9f, 0.45f, 0.2f, 1f);
    [SerializeField] private Color evasiveColor = new Color(0.7f, 0.6f, 0.3f, 1f);
    [SerializeField] private Color dangerousColor = new Color(0.85f, 0.15f, 0.15f, 1f);
    [SerializeField] private Color coherentColor = new Color(0.4f, 0.65f, 0.4f, 0.8f);

    private Coroutine fadeCoroutine;
    private QuestionUI questionUI;

    private void Awake()
    {
        EnsureBindings();
    }

    private void OnEnable()
    {
        if (questionUI != null)
        {
            questionUI.QuestionAnswered += OnQuestionAnswered;
        }
    }

    private void OnDisable()
    {
        if (questionUI != null)
        {
            questionUI.QuestionAnswered -= OnQuestionAnswered;
        }
    }

    private void OnQuestionAnswered(QuestionAnswer qa)
    {
        if (qa == null)
        {
            return;
        }

        string feedback = GetFeedbackText(qa.responseTag);
        Color color = GetFeedbackColor(qa.responseTag);

        if (string.IsNullOrEmpty(feedback))
        {
            return;
        }

        ShowFeedback(feedback, color);
    }

    private void ShowFeedback(string text, Color color)
    {
        if (feedbackTmpText != null)
        {
            feedbackTmpText.text = text;
            feedbackTmpText.color = color;
        }

        if (feedbackLegacyText != null)
        {
            feedbackLegacyText.text = text;
            feedbackLegacyText.color = color;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    public void Clear()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        SetText("");
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(displayDuration);

        float fadeDuration = 0.8f;
        float elapsed = 0f;
        Color startColor = feedbackTmpText != null ? feedbackTmpText.color : (feedbackLegacyText != null ? feedbackLegacyText.color : Color.white);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            Color faded = new Color(startColor.r, startColor.g, startColor.b, alpha);

            if (feedbackTmpText != null)
            {
                feedbackTmpText.color = faded;
            }

            if (feedbackLegacyText != null)
            {
                feedbackLegacyText.color = faded;
            }

            yield return null;
        }

        SetText("");
        fadeCoroutine = null;
    }

    private string GetFeedbackText(ResponseTag tag)
    {
        switch (tag)
        {
            case ResponseTag.Contradictory:
                return "[ Algo no cuadra en su respuesta... ]";
            case ResponseTag.Evasive:
                return "[ Evade la pregunta. ]";
            case ResponseTag.Dangerous:
                return "[ Esas palabras no suenan humanas. ]";
            case ResponseTag.Coherent:
                return "[ Respuesta coherente. ]";
            case ResponseTag.Unknown:
            default:
                return null;
        }
    }

    private Color GetFeedbackColor(ResponseTag tag)
    {
        switch (tag)
        {
            case ResponseTag.Contradictory:
                return contradictionColor;
            case ResponseTag.Evasive:
                return evasiveColor;
            case ResponseTag.Dangerous:
                return dangerousColor;
            case ResponseTag.Coherent:
                return coherentColor;
            default:
                return Color.gray;
        }
    }

    private void SetText(string value)
    {
        if (feedbackTmpText != null)
        {
            feedbackTmpText.text = value;
        }

        if (feedbackLegacyText != null)
        {
            feedbackLegacyText.text = value;
        }
    }

    private void EnsureBindings()
    {
        if (questionUI == null)
        {
            questionUI = Object.FindFirstObjectByType<QuestionUI>();
        }

        if (questionUI != null)
        {
            questionUI.QuestionAnswered -= OnQuestionAnswered;
            questionUI.QuestionAnswered += OnQuestionAnswered;
        }

        if (feedbackTmpText == null && feedbackLegacyText == null)
        {
            GameObject existing = GameObject.Find("ContradictionFeedbackText");
            if (existing != null)
            {
                feedbackTmpText = existing.GetComponent<TMP_Text>();
                feedbackLegacyText = existing.GetComponent<Text>();
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

        GameObject textObject = new GameObject("ContradictionFeedbackText", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.3f, 0.0f);
        rect.anchorMax = new Vector2(0.95f, 0.0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 55f);
        rect.sizeDelta = new Vector2(0f, 30f);

        Text legacyText = textObject.GetComponent<Text>();
        legacyText.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 16);
        legacyText.fontSize = 16;
        legacyText.fontStyle = FontStyle.Italic;
        legacyText.color = evasiveColor;
        legacyText.alignment = TextAnchor.MiddleCenter;
        legacyText.raycastTarget = false;

        feedbackLegacyText = legacyText;
    }
}
