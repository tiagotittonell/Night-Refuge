using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text visitorNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text cluesText;
    [SerializeField] private TMP_Text observationValuesText;

    [Header("UI Legacy Text")]
    [SerializeField] private Text visitorNameLegacyText;
    [SerializeField] private Text dialogueLegacyText;
    [SerializeField] private Text cluesLegacyText;
    [SerializeField] private Text observationValuesLegacyText;

    private void Awake()
    {
        EnsureLegacyBindings();
    }

    public void ShowVisitor(VisitorData visitor)
    {
        EnsureLegacyBindings();

        if (visitor == null)
        {
            Clear();
            return;
        }

        SetText(visitorNameText, visitorNameLegacyText, visitor.visitorName);
        SetText(dialogueText, dialogueLegacyText, visitor.introDialogue);

        ShowClues(visitor);
        ShowObservationValues(visitor);
    }

    public void ShowAnswer(string answer)
    {
        EnsureLegacyBindings();
        SetText(dialogueText, dialogueLegacyText, answer);
    }

    public void ShowDecisionFeedback(string feedback)
    {
        EnsureLegacyBindings();
        SetText(dialogueText, dialogueLegacyText, feedback);
    }

    public void Clear()
    {
        EnsureLegacyBindings();
        SetText(visitorNameText, visitorNameLegacyText, string.Empty);
        SetText(dialogueText, dialogueLegacyText, string.Empty);
        SetText(cluesText, cluesLegacyText, string.Empty);
        SetText(observationValuesText, observationValuesLegacyText, string.Empty);
    }

    private void ShowClues(VisitorData visitor)
    {
        if (visitor.visualClues == null || visitor.visualClues.Count == 0)
        {
            SetText(cluesText, cluesLegacyText, "Pistas: sin pistas claras");
            return;
        }

        StringBuilder builder = new StringBuilder("Pistas observables:");
        foreach (string clue in visitor.visualClues)
        {
            builder.AppendLine();
            builder.Append("- ");
            builder.Append(clue);
        }

        SetText(cluesText, cluesLegacyText, builder.ToString());
    }

    private void ShowObservationValues(VisitorData visitor)
    {
        if (visitor.observationProfile == null)
        {
            SetText(observationValuesText, observationValuesLegacyText, string.Empty);
            return;
        }

        SetText(observationValuesText, observationValuesLegacyText, visitor.observationProfile.ToPanelText());
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

    private void EnsureLegacyBindings()
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        if (dynamicLayer != null)
        {
            dynamicLayer.transform.SetAsLastSibling();
        }

        if (visitorNameLegacyText == null)
        {
            visitorNameLegacyText = FindLegacyText("CurrentVisitorNameText");
        }

        if (dialogueLegacyText == null)
        {
            dialogueLegacyText = FindLegacyText("RuntimeDialogueText");
        }

        if (cluesLegacyText == null)
        {
            cluesLegacyText = FindLegacyText("RuntimeCluesText");
        }

        if (observationValuesLegacyText == null)
        {
            observationValuesLegacyText = FindLegacyText("ObservationValues");
        }
    }

    private Text FindLegacyText(string objectName)
    {
        GameObject found = GameObject.Find(objectName);
        return found != null ? found.GetComponent<Text>() : null;
    }
}
