using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndNightSummaryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private Text summaryLegacyText;
    [SerializeField] private Button continueButton;

    private NightManager nightManager;

    private void Awake()
    {
        EnsureLayout();

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(Continue);
        }

        Hide();
    }

    public void Initialize(NightManager manager)
    {
        nightManager = manager;
    }

    public void Show(NightSummary summary, RefugeStats stats, bool hasNextNight, string title = "", string overrideResult = "")
    {
        EnsureLayout();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            panelRoot.transform.SetAsLastSibling();
        }

        string result = string.IsNullOrWhiteSpace(overrideResult) ? GetResultText(stats) : overrideResult;
        string header = string.IsNullOrWhiteSpace(title) ? $"Fin de la noche {summary.nightNumber}" : title;
        string summaryTextValue =
            $"{header}\n\n" +
            $"Humanos aceptados: {summary.humansAccepted}\n" +
            $"Humanos rechazados: {summary.humansRejected}\n" +
            $"Imitadores aceptados: {summary.imitatorsAccepted}\n" +
            $"Imitadores rechazados: {summary.imitatorsRejected}\n\n" +
            $"Comida restante: {stats.Food}\n" +
            $"Seguridad: {stats.Security}\n" +
            $"Moral: {stats.Morale}\n" +
            $"Poblacion: {stats.Population}\n\n" +
            $"Resultado general: {result}";

        if (summaryText != null)
        {
            summaryText.text = summaryTextValue;
        }

        if (summaryLegacyText != null)
        {
            summaryLegacyText.text = summaryTextValue;
        }

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(hasNextNight);
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Continue()
    {
        Hide();

        if (nightManager != null)
        {
            nightManager.StartNextNight();
        }
    }

    private string GetResultText(RefugeStats stats)
    {
        if (stats.Security <= 0)
        {
            return "El refugio queda expuesto. Algo logro entrar.";
        }

        if (stats.Morale <= 0)
        {
            return "La gente pierde la esperanza y nadie duerme.";
        }

        if (stats.Food <= 0)
        {
            return "El refugio sobrevive, pero el hambre ya decide por todos.";
        }

        return "El refugio resiste una noche mas.";
    }

    private void EnsureLayout()
    {
        if (panelRoot != null)
        {
            RectTransform panelRect = panelRoot.GetComponent<RectTransform>();
            if (panelRect != null && (panelRect.sizeDelta.x <= 1f || panelRect.sizeDelta.y <= 1f))
            {
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.anchoredPosition = Vector2.zero;
                panelRect.sizeDelta = new Vector2(660f, 560f);
                panelRect.localScale = Vector3.one;
            }
        }

        if (summaryText != null)
        {
            RectTransform summaryRect = summaryText.GetComponent<RectTransform>();
            if (summaryRect != null)
            {
                summaryRect.anchorMin = Vector2.zero;
                summaryRect.anchorMax = Vector2.one;
                summaryRect.offsetMin = new Vector2(42f, 120f);
                summaryRect.offsetMax = new Vector2(-42f, -42f);
                summaryRect.localScale = Vector3.one;
            }

            summaryText.fontSize = 26f;
            summaryText.color = new Color(0.78f, 0.70f, 0.58f, 1f);
            summaryText.alignment = TextAlignmentOptions.TopLeft;
        }

        if (summaryLegacyText != null)
        {
            RectTransform summaryRect = summaryLegacyText.GetComponent<RectTransform>();
            if (summaryRect != null)
            {
                summaryRect.anchorMin = Vector2.zero;
                summaryRect.anchorMax = Vector2.one;
                summaryRect.offsetMin = new Vector2(42f, 120f);
                summaryRect.offsetMax = new Vector2(-42f, -42f);
                summaryRect.localScale = Vector3.one;
            }

            summaryLegacyText.fontSize = 24;
            summaryLegacyText.color = new Color(0.78f, 0.70f, 0.58f, 1f);
            summaryLegacyText.alignment = TextAnchor.UpperLeft;
        }

        if (continueButton == null)
        {
            return;
        }

        RectTransform buttonRect = continueButton.GetComponent<RectTransform>();
        if (buttonRect != null && (buttonRect.sizeDelta.x <= 1f || buttonRect.sizeDelta.y <= 1f))
        {
            buttonRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.anchoredPosition = new Vector2(0f, 35f);
            buttonRect.sizeDelta = new Vector2(250f, 58f);
            buttonRect.localScale = Vector3.one;
        }

        Image buttonImage = continueButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = new Color(0.18f, 0.15f, 0.10f, 0.95f);
        }

        TMP_Text tmpLabel = continueButton.GetComponentInChildren<TMP_Text>(true);
        if (tmpLabel != null)
        {
            RectTransform labelRect = tmpLabel.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;
                labelRect.localScale = Vector3.one;
            }

            tmpLabel.text = "CONTINUAR";
            tmpLabel.fontSize = 22f;
            tmpLabel.color = new Color(0.78f, 0.70f, 0.58f, 1f);
            tmpLabel.alignment = TextAlignmentOptions.Center;
        }

        Text legacyLabel = continueButton.GetComponentInChildren<Text>(true);
        if (legacyLabel != null)
        {
            RectTransform labelRect = legacyLabel.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;
                labelRect.localScale = Vector3.one;
            }

            legacyLabel.text = "CONTINUAR";
            legacyLabel.fontSize = 22;
            legacyLabel.color = new Color(0.78f, 0.70f, 0.58f, 1f);
            legacyLabel.alignment = TextAnchor.MiddleCenter;
        }
    }
}

public struct NightSummary
{
    public int nightNumber;
    public int humansAccepted;
    public int humansRejected;
    public int imitatorsAccepted;
    public int imitatorsRejected;

    public void Reset(int newNightNumber)
    {
        nightNumber = newNightNumber;
        humansAccepted = 0;
        humansRejected = 0;
        imitatorsAccepted = 0;
        imitatorsRejected = 0;
    }
}
