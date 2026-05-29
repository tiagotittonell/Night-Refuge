using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Visitor", menuName = "Refugio Nocturno/Visitor Data")]
public class VisitorData : ScriptableObject
{
    [Header("Identidad")]
    public string visitorName;
    public bool isImitator;
    public Sprite visitorSprite;

    [Header("Dialogo")]
    [TextArea(2, 5)]
    public string introDialogue;
    public List<QuestionAnswer> answers = new List<QuestionAnswer>();

    [Header("Observacion")]
    [TextArea(1, 3)]
    public List<string> visualClues = new List<string>();
    public ObservationProfile observationProfile = new ObservationProfile();

    [Header("Consecuencias al permitir")]
    public int foodChangeOnAccept;
    public int securityChangeOnAccept;
    public int moraleChangeOnAccept;
    public int populationChangeOnAccept;
    [TextArea(2, 4)]
    public string feedbackOnAccept;

    [Header("Consecuencias al rechazar")]
    public int foodChangeOnReject;
    public int securityChangeOnReject;
    public int moraleChangeOnReject;
    public int populationChangeOnReject;
    [TextArea(2, 4)]
    public string feedbackOnReject;
}

[System.Serializable]
public class ObservationProfile
{
    public string wetClothes = "NO";
    public string tremor = "NO";
    public string evasiveLook = "NO";
    public string visibleWounds = "NO";
    public string behavior = "NORMAL";
    public string answers = "COHERENTES";

    public string ToPanelText()
    {
        return
            $"{wetClothes}\n\n" +
            $"{tremor}\n\n" +
            $"{evasiveLook}\n\n" +
            $"{visibleWounds}\n\n" +
            $"{behavior}\n\n" +
            $"{answers}";
    }
}
