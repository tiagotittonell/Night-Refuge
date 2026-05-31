using System.Collections;
using System.Collections.Generic;
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

        int foodApplied = 0;
        int securityApplied = 0;
        int moraleApplied = 0;
        int popApplied = 0;

        if (accepted)
        {
            int securityChange = visitor.securityChangeOnAccept;
            int foodChange = visitor.foodChangeOnAccept;

            UpgradeManager upgrades = Object.FindFirstObjectByType<UpgradeManager>();

            // Cerradura reforzada: reduce daño de seguridad
            if (upgrades != null && securityChange < 0 && upgrades.HasUpgrade(UpgradeEffect.ReinforcedLock))
            {
                securityChange += upgrades.GetUpgradeValue(UpgradeEffect.ReinforcedLock);
                if (securityChange > 0) securityChange = 0;
            }

            // Guardia extra: reduce daño de seguridad adicional
            if (upgrades != null && securityChange < 0 && upgrades.HasUpgrade(UpgradeEffect.ExtraGuard))
            {
                securityChange += upgrades.GetUpgradeValue(UpgradeEffect.ExtraGuard);
                if (securityChange > 0) securityChange = 0;
            }

            // Racionamiento: reduce comida consumida, pero baja moral ligeramente
            if (upgrades != null && foodChange < 0 && upgrades.HasUpgrade(UpgradeEffect.Rationing))
            {
                foodChange += upgrades.GetUpgradeValue(UpgradeEffect.Rationing);
                if (foodChange > 0) foodChange = 0;
            }

            foodApplied = foodChange;
            securityApplied = securityChange;
            moraleApplied = visitor.moraleChangeOnAccept;
            popApplied = visitor.populationChangeOnAccept;

            // Rationing drawback: slight morale penalty when accepting visitors
            if (upgrades != null && upgrades.HasUpgrade(UpgradeEffect.Rationing) && !visitor.isImitator)
            {
                moraleApplied -= 1;
            }

            refugeStats.ApplyChanges(foodApplied, securityApplied, moraleApplied, popApplied);
        }
        else
        {
            foodApplied = visitor.foodChangeOnReject;
            securityApplied = visitor.securityChangeOnReject;
            moraleApplied = visitor.moraleChangeOnReject;
            popApplied = visitor.populationChangeOnReject;

            refugeStats.ApplyChanges(foodApplied, securityApplied, moraleApplied, popApplied);
        }

        if (dialogueUI == null)
        {
            dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
        }

        if (dialogueUI != null)
        {
            dialogueUI.ShowDecisionFeedback(GetDecisionFeedback(visitor, accepted));
        }

        RecordVisitorDecision(visitor, accepted, foodApplied, securityApplied, moraleApplied, popApplied);

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

    private void RecordVisitorDecision(VisitorData visitor, bool accepted, int foodApplied, int securityApplied, int moraleApplied, int popApplied)
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
        record.foodDelta = foodApplied;
        record.securityDelta = securityApplied;
        record.moraleDelta = moraleApplied;
        record.populationDelta = popApplied;

        // Collect questions asked during this visitor
        if (questionUI == null)
        {
            questionUI = Object.FindFirstObjectByType<QuestionUI>();
        }
        if (questionUI != null)
        {
            questionUI.GetAndClearPendingQuestions(out var questions, out var tags);
            record.questionsAsked.AddRange(questions);
            record.responseTags.AddRange(tags);
        }

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
