using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisitorManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image visitorImage;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestionUI questionUI;

    private List<VisitorData> visitors = new List<VisitorData>();
    private int currentVisitorIndex = -1;

    public VisitorData CurrentVisitor { get; private set; }
    public bool HasActiveVisitor => CurrentVisitor != null;

    public void LoadVisitors(List<VisitorData> nightVisitors)
    {
        visitors = nightVisitors != null ? new List<VisitorData>(nightVisitors) : new List<VisitorData>();
        currentVisitorIndex = -1;
        CurrentVisitor = null;
    }

    public bool ShowNextVisitor()
    {
        currentVisitorIndex++;

        if (currentVisitorIndex >= visitors.Count)
        {
            CurrentVisitor = null;
            ClearVisitorUI();
            return false;
        }

        CurrentVisitor = visitors[currentVisitorIndex];
        ShowCurrentVisitor();
        return true;
    }

    private void ShowCurrentVisitor()
    {
        if (visitorImage != null)
        {
            visitorImage.sprite = CurrentVisitor.visitorSprite;
            visitorImage.enabled = true;
            visitorImage.preserveAspect = true;
            visitorImage.color = CurrentVisitor.visitorSprite != null
                ? Color.white
                : new Color(0.08f, 0.08f, 0.09f, 0.85f);
        }

        if (dialogueUI != null)
        {
            dialogueUI.ShowVisitor(CurrentVisitor);
        }

        if (questionUI != null)
        {
            questionUI.SetupQuestions(CurrentVisitor);
        }
    }

    private void ClearVisitorUI()
    {
        if (visitorImage != null)
        {
            visitorImage.enabled = false;
        }

        if (dialogueUI != null)
        {
            dialogueUI.Clear();
        }

        if (questionUI != null)
        {
            questionUI.SetupQuestions(null);
        }
    }
}
