using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DecisionController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VisitorManager visitorManager;
    [SerializeField] private RefugeStats refugeStats;
    [SerializeField] private NightManager nightManager;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestionUI questionUI;
    [SerializeField] private Button allowButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private float decisionFeedbackSeconds = 3.5f;

    private bool resolvingDecision;
    private VisitorLog visitorLog;
    private SuspicionSystem suspicionSystem;

    private void Awake()
    {
        if (allowButton != null)
        {
            allowButton.onClick.AddListener(AllowCurrentVisitor);
        }

        if (rejectButton != null)
        {
            rejectButton.onClick.AddListener(RejectCurrentVisitor);
        }

        visitorLog = Object.FindFirstObjectByType<VisitorLog>();
        suspicionSystem = Object.FindFirstObjectByType<SuspicionSystem>();
    }

    public void SetInteractable(bool interactable)
    {
        if (allowButton != null)
        {
            allowButton.interactable = interactable;
        }

        if (rejectButton != null)
        {
            rejectButton.interactable = interactable;
        }
    }

    private void AllowCurrentVisitor()
    {
        Decide(true);
    }

    private void RejectCurrentVisitor()
    {
        Decide(false);
    }

    private void Decide(bool accepted)
    {
        if (resolvingDecision || visitorManager == null || refugeStats == null || nightManager == null)
        {
            return;
        }

        VisitorData visitor = visitorManager.CurrentVisitor;
        if (visitor == null)
        {
            return;
        }

        StartCoroutine(ResolveDecision(visitor, accepted));
    }

    private IEnumerator ResolveDecision(VisitorData visitor, bool accepted)
    {
        resolvingDecision = true;
        SetInteractable(false);

        if (questionUI == null)
        {
            questionUI = Object.FindFirstObjectByType<QuestionUI>();
        }

        if (questionUI != null)
        {
            questionUI.SetInteractable(false);
        }

        if (accepted)
        {
            int securityChange = visitor.securityChangeOnAccept;
            int foodChange = visitor.foodChangeOnAccept;

            // Cerradura reforzada: reduce daño de seguridad
            UpgradeManager upgrades = Object.FindFirstObjectByType<UpgradeManager>();
            if (upgrades != null && securityChange < 0 && upgrades.HasUpgrade(UpgradeEffect.ReinforcedLock))
            {
                securityChange += upgrades.GetUpgradeValue(UpgradeEffect.ReinforcedLock);
                if (securityChange > 0) securityChange = 0;
            }

            // Racionamiento: reduce comida consumida
            if (upgrades != null && foodChange < 0 && upgrades.HasUpgrade(UpgradeEffect.Rationing))
            {
                foodChange += upgrades.GetUpgradeValue(UpgradeEffect.Rationing);
                if (foodChange > 0) foodChange = 0;
            }

            refugeStats.ApplyChanges(
                foodChange,
                securityChange,
                visitor.moraleChangeOnAccept,
                visitor.populationChangeOnAccept);
        }
        else
        {
            refugeStats.ApplyChanges(
                visitor.foodChangeOnReject,
                visitor.securityChangeOnReject,
                visitor.moraleChangeOnReject,
                visitor.populationChangeOnReject);
        }

        if (dialogueUI == null)
        {
            dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
        }

        if (dialogueUI != null)
        {
            dialogueUI.ShowDecisionFeedback(GetDecisionFeedback(visitor, accepted));
        }

        RecordVisitorDecision(visitor, accepted);

        // Consume clock time for making a decision
        NightClock clock = Object.FindFirstObjectByType<NightClock>();
        if (clock != null)
        {
            clock.ConsumeDecision();
        }

        yield return new WaitForSeconds(decisionFeedbackSeconds);

        bool canContinue = nightManager.RegisterDecision(visitor, accepted);
        resolvingDecision = false;

        if (canContinue)
        {
            SetInteractable(true);
        }
    }

    private void RecordVisitorDecision(VisitorData visitor, bool accepted)
    {
        if (visitorLog == null)
        {
            visitorLog = Object.FindFirstObjectByType<VisitorLog>();
        }

        if (visitorLog == null)
        {
            return;
        }

        if (suspicionSystem == null)
        {
            suspicionSystem = Object.FindFirstObjectByType<SuspicionSystem>();
        }

        SuspicionLevel suspicion = suspicionSystem != null
            ? suspicionSystem.CurrentLevel
            : SuspicionLevel.Unknown;

        VisitorRecord record = new VisitorRecord(
            nightManager != null ? nightManager.CurrentNightNumber : 1,
            visitor.visitorName,
            accepted,
            suspicion);

        if (visitor.visualClues != null)
        {
            record.observedClues.AddRange(visitor.visualClues);
        }

        record.decisionFeedback = GetDecisionFeedback(visitor, accepted);
        visitorLog.RegisterVisitor(record);
    }

    private string GetDecisionFeedback(VisitorData visitor, bool accepted)
    {
        string customFeedback = accepted ? visitor.feedbackOnAccept : visitor.feedbackOnReject;
        if (!string.IsNullOrWhiteSpace(customFeedback))
        {
            return customFeedback;
        }

        string action = accepted ? "permitiste entrar" : "rechazaste";
        string identity = visitor.isImitator ? "imitador" : "humano";
        return $"Decidiste: {action} a {visitor.visitorName}.\nEra {identity}.";
    }
}
