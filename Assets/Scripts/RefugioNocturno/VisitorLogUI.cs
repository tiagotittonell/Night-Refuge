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
            GameObject existing = GameObject.Find("VisitorLogPanel");
            if (existing != null)
            {
                panelRoot = existing;
            }
        }

        if (logTmpText == null && logLegacyText == null && panelRoot != null)
        {
            logTmpText = panelRoot.GetComponentInChildren<TMP_Text>(true);
            logLegacyText = panelRoot.GetComponentInChildren<Text>(true);
        }

        if (toggleButton == null)
        {
            GameObject btn = GameObject.Find("VisitorLogToggleButton");
            if (btn != null)
            {
                toggleButton = btn.GetComponent<Button>();
                if (toggleButton != null)
                {
                    toggleButton.onClick.RemoveListener(Toggle);
                    toggleButton.onClick.AddListener(Toggle);
                }
            }
        }
    }
}
