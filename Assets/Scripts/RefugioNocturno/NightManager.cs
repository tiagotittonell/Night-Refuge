using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NightManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VisitorManager visitorManager;
    [SerializeField] private RefugeStats refugeStats;
    [SerializeField] private DecisionController decisionController;
    [SerializeField] private EndNightSummaryUI endNightSummaryUI;
    [SerializeField] private TMP_Text ruleText;
    [SerializeField] private Text ruleLegacyText;
    [SerializeField] private GameObject rulePanel;
    [SerializeField] private TMP_Text nightText;
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private Text nightLegacyText;
    [SerializeField] private Text clockLegacyText;

    [Header("Noches")]
    [SerializeField] private List<NightData> nights = new List<NightData>();
    [SerializeField] private bool createExampleDataOnStart = true;

    private int currentNightIndex = -1;
    private NightSummary currentSummary;
    private bool gameEnded;
    private SuspicionSystem suspicionSystem;
    private InterEventSystem interEventSystem;
    private NightClock nightClock;

    public int CurrentNightNumber => currentNightIndex + 1;

    private void Awake()
    {
        if (endNightSummaryUI != null)
        {
            endNightSummaryUI.Initialize(this);
        }
    }

    private void Start()
    {
        EnsureSuspicionSystem();
        EnsureVisitorLog();

        if (createExampleDataOnStart && nights.Count == 0)
        {
            nights = ExampleVisitorFactory.CreateExampleNights();
        }

        StartNextNight();
    }

    private void EnsureSuspicionSystem()
    {
        suspicionSystem = Object.FindFirstObjectByType<SuspicionSystem>();
        if (suspicionSystem == null)
        {
            GameObject systemObject = new GameObject("SuspicionSystem");
            suspicionSystem = systemObject.AddComponent<SuspicionSystem>();
            systemObject.AddComponent<SuspicionUI>();
            systemObject.AddComponent<ContradictionFeedbackUI>();
        }
        else if (Object.FindFirstObjectByType<ContradictionFeedbackUI>() == null)
        {
            suspicionSystem.gameObject.AddComponent<ContradictionFeedbackUI>();
        }
    }

    private void EnsureVisitorLog()
    {
        VisitorLog log = Object.FindFirstObjectByType<VisitorLog>();
        if (log == null)
        {
            GameObject logObject = new GameObject("VisitorLog");
            logObject.AddComponent<VisitorLog>();
            logObject.AddComponent<VisitorLogUI>();
        }

        interEventSystem = Object.FindFirstObjectByType<InterEventSystem>();
        if (interEventSystem == null)
        {
            GameObject eventObject = new GameObject("InterEventSystem");
            interEventSystem = eventObject.AddComponent<InterEventSystem>();
            eventObject.AddComponent<InterEventUI>();
        }

        nightClock = Object.FindFirstObjectByType<NightClock>();
        if (nightClock == null)
        {
            GameObject clockObject = new GameObject("NightClock");
            nightClock = clockObject.AddComponent<NightClock>();
        }
    }

    public void StartNextNight()
    {
        currentNightIndex++;

        if (currentNightIndex >= nights.Count)
        {
            ShowFinalGameState();
            return;
        }

        NightData night = nights[currentNightIndex];
        currentSummary.Reset(currentNightIndex + 1);
        UpdateNightHeader(night, currentNightIndex + 1);

        if (nightClock != null)
        {
            nightClock.StartNight(night.clockTime);
        }

        if (suspicionSystem != null)
        {
            suspicionSystem.SetNightRule(night.suspicionRule);
        }

        if (interEventSystem != null)
        {
            interEventSystem.SetNightEvents(night);
        }

        if (rulePanel != null)
        {
            rulePanel.SetActive(true);
        }

        SetText(ruleText, ruleLegacyText, $"Noche {currentNightIndex + 1}\n{night.rule}");

        if (visitorManager != null)
        {
            visitorManager.LoadVisitors(night.visitors);
            ShowNextVisitorOrEndNight();
        }

        if (decisionController != null)
        {
            decisionController.SetInteractable(true);
        }
    }

    public bool RegisterDecision(VisitorData visitor, bool accepted)
    {
        if (gameEnded)
        {
            return false;
        }

        if (visitor.isImitator)
        {
            if (accepted)
            {
                currentSummary.imitatorsAccepted++;
            }
            else
            {
                currentSummary.imitatorsRejected++;
            }
        }
        else
        {
            if (accepted)
            {
                currentSummary.humansAccepted++;
            }
            else
            {
                currentSummary.humansRejected++;
            }
        }

        if (refugeStats != null && refugeStats.SecurityCollapsed)
        {
            EndRun(
                "DERROTA",
                "La seguridad llego a cero. El refugio ya no puede distinguir la puerta de la noche.");
            return false;
        }

        if (refugeStats != null && refugeStats.MoraleCollapsed)
        {
            EndRun(
                "DERROTA",
                "La moral llego a cero. La gente deja de obedecer y el refugio se rompe desde adentro.");
            return false;
        }

        TryTriggerInterEvent(accepted);

        return ShowNextVisitorOrEndNight();
    }

    private void TryTriggerInterEvent(bool lastAccepted)
    {
        if (interEventSystem == null || refugeStats == null)
        {
            return;
        }

        interEventSystem.UpdateContext(lastAccepted, refugeStats.Security);
        InterVisitorEvent interEvent = interEventSystem.TryGetNextEvent();

        if (interEvent == null)
        {
            return;
        }

        // Apply resource effects
        refugeStats.ApplyChanges(
            interEvent.foodChange,
            interEvent.securityChange,
            interEvent.moraleChange,
            interEvent.populationChange);

        if (nightClock != null)
        {
            nightClock.ConsumeInterEvent();
        }

        interEventSystem.NotifyEventTriggered(interEvent);
    }

    private bool ShowNextVisitorOrEndNight()
    {
        if (visitorManager != null && visitorManager.ShowNextVisitor())
        {
            if (nightClock != null)
            {
                nightClock.ConsumeVisitorArrival();
            }

            return true;
        }

        EndCurrentNight();
        return false;
    }

    private void EndCurrentNight()
    {
        if (decisionController != null)
        {
            decisionController.SetInteractable(false);
        }

        if (rulePanel != null)
        {
            rulePanel.SetActive(false);
        }

        if (refugeStats != null && refugeStats.FoodDepleted)
        {
            EndRun(
                "DERROTA",
                "La comida llego a cero al cierre de la noche. Nadie discute la puerta cuando empieza el hambre.");
            return;
        }

        bool hasNextNight = currentNightIndex + 1 < nights.Count;
        if (endNightSummaryUI != null)
        {
            if (hasNextNight)
            {
                endNightSummaryUI.Show(currentSummary, refugeStats, true);
            }
            else
            {
                gameEnded = true;
                endNightSummaryUI.Show(
                    currentSummary,
                    refugeStats,
                    false,
                    "VICTORIA",
                    "Sobreviviste las dos noches. El refugio sigue en pie, aunque nadie vuelve a mirar la ventana igual.");
            }
        }
    }

    private void ShowFinalGameState()
    {
        gameEnded = true;

        if (rulePanel != null)
        {
            rulePanel.SetActive(false);
        }

        if (decisionController != null)
        {
            decisionController.SetInteractable(false);
        }
    }

    private void EndRun(string title, string result)
    {
        gameEnded = true;

        if (decisionController != null)
        {
            decisionController.SetInteractable(false);
        }

        if (rulePanel != null)
        {
            rulePanel.SetActive(false);
        }

        if (endNightSummaryUI != null)
        {
            endNightSummaryUI.Show(currentSummary, refugeStats, false, title, result);
        }
    }

    private void SetText(TMP_Text tmpText, Text legacyText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }

        if (legacyText != null)
        {
            legacyText.text = value;
        }
    }

    private void UpdateNightHeader(NightData night, int nightNumber)
    {
        EnsureHeaderBindings();

        string label = string.IsNullOrWhiteSpace(night.nightLabel)
            ? $"NOCHE {nightNumber}"
            : night.nightLabel;
        string time = string.IsNullOrWhiteSpace(night.clockTime)
            ? GetDefaultClockTime(nightNumber)
            : night.clockTime;

        SetText(nightText, nightLegacyText, label);
        SetText(clockText, clockLegacyText, time);
    }

    private void EnsureHeaderBindings()
    {
        if (nightLegacyText == null)
        {
            nightLegacyText = FindLegacyText("NightStatic");
        }

        if (clockLegacyText == null)
        {
            clockLegacyText = FindLegacyText("ClockStatic");
        }

        if (nightText == null)
        {
            nightText = FindTmpText("NightStatic");
        }

        if (clockText == null)
        {
            clockText = FindTmpText("ClockStatic");
        }
    }

    private Text FindLegacyText(string objectName)
    {
        GameObject found = GameObject.Find(objectName);
        return found != null ? found.GetComponent<Text>() : null;
    }

    private TMP_Text FindTmpText(string objectName)
    {
        GameObject found = GameObject.Find(objectName);
        return found != null ? found.GetComponent<TMP_Text>() : null;
    }

    private string GetDefaultClockTime(int nightNumber)
    {
        switch (nightNumber)
        {
            case 1:
                return "01:47 AM";
            case 2:
                return "03:12 AM";
            default:
                return "04:30 AM";
        }
    }
}

[System.Serializable]
public class NightData
{
    public string nightLabel;
    public string clockTime;
    [TextArea(2, 4)]
    public string rule;
    public NightRuleData suspicionRule;
    public List<VisitorData> visitors = new List<VisitorData>();
    public List<InterVisitorEvent> interEvents = new List<InterVisitorEvent>();
}
