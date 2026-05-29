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
    [SerializeField] private float decisionFeedbackSeconds = 1.25f;

    private bool resolvingDecision;

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
            refugeStats.ApplyChanges(
                visitor.foodChangeOnAccept,
                visitor.securityChangeOnAccept,
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

        yield return new WaitForSeconds(decisionFeedbackSeconds);

        bool canContinue = nightManager.RegisterDecision(visitor, accepted);
        resolvingDecision = false;

        if (canContinue)
        {
            SetInteractable(true);
        }
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
