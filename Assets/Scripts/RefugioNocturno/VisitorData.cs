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

    [Header("Preguntas de Radio (requiere Radio de Onda Corta)")]
    public List<QuestionAnswer> radioQuestions = new List<QuestionAnswer>();

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
    [Header("Visual")]
    public string wetClothes = "NO";
    public string tremor = "NO";
    public string evasiveLook = "NO";
    public string visibleWounds = "NO";
    public string behavior = "NORMAL";
    public string answers = "COHERENTES";

    [Header("Audio (requiere Microfono Mejorado)")]
    public string voiceTone = "";
    public string breathingPattern = "";

    [Header("Termico (requiere Detector Termico)")]
    public string bodyTemperature = "";

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

    /// <summary>
    /// Returns only the value column for the observation panel.
    /// Labels live in a separate static text object.
    /// Observation noise is applied via ObservationFilter before calling this.
    /// </summary>
    public string ToFullPanelText(bool hasMicrophone, bool hasThermal)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine(wetClothes);
        sb.AppendLine();
        sb.AppendLine(tremor);
        sb.AppendLine();
        sb.AppendLine(evasiveLook);
        sb.AppendLine();
        sb.AppendLine(visibleWounds);
        sb.AppendLine();
        sb.AppendLine(behavior);
        sb.AppendLine();
        sb.AppendLine(answers);

        if (hasMicrophone)
        {
            string voice = string.IsNullOrEmpty(voiceTone) ? "SIN DATOS" : voiceTone;
            string breath = string.IsNullOrEmpty(breathingPattern) ? "SIN DATOS" : breathingPattern;
            sb.AppendLine();
            sb.AppendLine(voice);
            sb.AppendLine();
            sb.AppendLine(breath);
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("NO DISPONIBLE");
            sb.AppendLine();
            sb.AppendLine("NO DISPONIBLE");
        }

        if (hasThermal)
        {
            string temp = string.IsNullOrEmpty(bodyTemperature) ? "SIN DATOS" : bodyTemperature;
            sb.AppendLine();
            sb.AppendLine(temp);
        }

        return sb.ToString();
    }
}
