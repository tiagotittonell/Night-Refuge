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
    /// Returns the full observation text with sensor data included based on upgrades owned.
    /// Observation noise is applied via ObservationFilter before calling this.
    /// </summary>
    public string ToFullPanelText(bool hasMicrophone, bool hasThermal)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Ropa mojada: {wetClothes}");
        sb.AppendLine($"Temblor: {tremor}");
        sb.AppendLine($"Mirada evasiva: {evasiveLook}");
        sb.AppendLine($"Heridas visibles: {visibleWounds}");
        sb.AppendLine($"Comportamiento: {behavior}");
        sb.AppendLine($"Respuestas: {answers}");

        if (hasMicrophone)
        {
            string voice = string.IsNullOrEmpty(voiceTone) ? "SIN DATOS" : voiceTone;
            string breath = string.IsNullOrEmpty(breathingPattern) ? "SIN DATOS" : breathingPattern;
            sb.AppendLine($"Tono de voz: {voice}");
            sb.AppendLine($"Respiracion: {breath}");
        }
        else
        {
            sb.AppendLine("Tono de voz: NO DISPONIBLE");
            sb.AppendLine("Respiracion: NO DISPONIBLE");
        }

        if (hasThermal)
        {
            string temp = string.IsNullOrEmpty(bodyTemperature) ? "SIN DATOS" : bodyTemperature;
            sb.AppendLine($"Temperatura: {temp}");
        }

        return sb.ToString();
    }
}
