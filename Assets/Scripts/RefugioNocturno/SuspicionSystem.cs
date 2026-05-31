using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calcula y expone el nivel de sospecha de un visitante basándose en
/// observaciones, respuestas obtenidas y la regla de la noche activa.
/// Nunca accede a isImitator — solo usa datos observables.
/// </summary>
public class SuspicionSystem : MonoBehaviour
{
    [Header("Pesos de observación")]
    [SerializeField] private int weightDryClothesInRain = 2;
    [SerializeField] private int weightNoTremor = 1;
    [SerializeField] private int weightEvasiveLook = 1;
    [SerializeField] private int weightNoWounds = 0;
    [SerializeField] private int weightAbnormalBehavior = 2;
    [SerializeField] private int weightIncoherentAnswers = 2;

    [Header("Pesos de respuestas")]
    [SerializeField] private int weightEvasiveResponse = 1;
    [SerializeField] private int weightContradictoryResponse = 2;
    [SerializeField] private int weightDangerousResponse = 3;
    [SerializeField] private int weightCoherentResponse = -1;

    [Header("Umbrales")]
    [SerializeField] private int thresholdLow = 2;
    [SerializeField] private int thresholdMedium = 4;
    [SerializeField] private int thresholdHigh = 6;

    public SuspicionLevel CurrentLevel { get; private set; } = SuspicionLevel.Unknown;
    public int CurrentScore { get; private set; }
    public event Action<SuspicionLevel> SuspicionChanged;

    private NightRuleData activeRule;
    private VisitorData currentVisitor;
    private readonly List<QuestionAnswer> answeredQuestions = new List<QuestionAnswer>();

    public void SetNightRule(NightRuleData rule)
    {
        activeRule = rule;
    }

    public void EvaluateVisitor(VisitorData visitor)
    {
        currentVisitor = visitor;
        answeredQuestions.Clear();

        if (visitor == null)
        {
            CurrentScore = 0;
            SetLevel(SuspicionLevel.Unknown);
            return;
        }

        Recalculate();
    }

    public void OnQuestionAnswered(QuestionAnswer qa)
    {
        if (qa == null || currentVisitor == null)
        {
            return;
        }

        answeredQuestions.Add(qa);
        Recalculate();
    }

    public void Clear()
    {
        currentVisitor = null;
        answeredQuestions.Clear();
        CurrentScore = 0;
        SetLevel(SuspicionLevel.Unknown);
    }

    private void Recalculate()
    {
        // Use filtered profile for scoring — same data the player sees
        UpgradeManager upgrades = UnityEngine.Object.FindFirstObjectByType<UpgradeManager>();
        bool hasLamp = upgrades != null && upgrades.HasUpgrade(UpgradeEffect.ReinforcedLamp);
        bool hasMic = upgrades != null && upgrades.HasUpgrade(UpgradeEffect.ImprovedMicrophone);
        bool hasThermal = upgrades != null && upgrades.HasUpgrade(UpgradeEffect.ThermalDetector);

        InterEventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<InterEventSystem>();
        bool blackout = eventSystem != null && eventSystem.IsBlackoutActive;

        NightManager nightManager = UnityEngine.Object.FindFirstObjectByType<NightManager>();
        int nightNum = nightManager != null ? nightManager.CurrentNightNumber : 1;

        ObservationProfile filtered = ObservationFilter.Filter(
            currentVisitor.observationProfile, nightNum, currentVisitor.visitorName, hasLamp, blackout);

        int score = CalculateObservationScore(filtered);
        score += CalculateResponseScore();
        score += CalculateNightRuleScore();

        // Audio/thermal scoring only if sensors owned
        if (hasMic)
        {
            score += CalculateAudioScore(currentVisitor.observationProfile);
        }
        if (hasThermal)
        {
            score += CalculateThermalScore(currentVisitor.observationProfile);
        }

        CurrentScore = Mathf.Max(0, score);
        SuspicionLevel newLevel = ScoreToLevel(CurrentScore);
        SetLevel(newLevel);
    }

    private int CalculateObservationScore(ObservationProfile profile)
    {
        if (profile == null)
        {
            return 0;
        }

        int score = 0;
        string wet = Normalize(profile.wetClothes);
        string tremor = Normalize(profile.tremor);
        string evasive = Normalize(profile.evasiveLook);
        string behavior = Normalize(profile.behavior);
        string answers = Normalize(profile.answers);

        // Skip noisy/inconclusive fields — they shouldn't affect scoring
        if (IsNoise(wet)) wet = "";
        if (IsNoise(tremor)) tremor = "";
        if (IsNoise(evasive)) evasive = "";
        if (IsNoise(behavior)) behavior = "";

        // Ropa seca cuando debería estar mojada (contexto de lluvia) es sospechoso
        if (wet == "NO")
        {
            score += weightDryClothesInRain;
        }

        // Sin temblor en condiciones frías
        if (tremor == "NO")
        {
            score += weightNoTremor;
        }

        // Mirada evasiva puede ser humano nervioso O imitador
        if (evasive == "SI")
        {
            score += weightEvasiveLook;
        }

        // Comportamiento anormal
        if (!string.IsNullOrEmpty(behavior) &&
            behavior != "NORMAL" && behavior != "ASUSTADA" && behavior != "ASUSTADO"
            && behavior != "NERVIOSA" && behavior != "NERVIOSO" && behavior != "AGITADA")
        {
            score += weightAbnormalBehavior;
        }

        // Respuestas incoherentes en el perfil
        if (answers != "COHERENTES" && !string.IsNullOrEmpty(answers))
        {
            score += weightIncoherentAnswers;
        }

        return score;
    }

    private static bool IsNoise(string value)
    {
        return value == "INCONCLUSO" || value == "NO VISIBLE" || value == "FALLA DE LUZ";
    }

    private int CalculateResponseScore()
    {
        int score = 0;

        foreach (QuestionAnswer qa in answeredQuestions)
        {
            switch (qa.responseTag)
            {
                case ResponseTag.Coherent:
                    score += weightCoherentResponse;
                    break;
                case ResponseTag.Evasive:
                    score += weightEvasiveResponse;
                    break;
                case ResponseTag.Contradictory:
                    score += weightContradictoryResponse;
                    break;
                case ResponseTag.Dangerous:
                    score += weightDangerousResponse;
                    break;
                case ResponseTag.Unknown:
                default:
                    break;
            }
        }

        return score;
    }

    private int CalculateNightRuleScore()
    {
        if (activeRule == null || activeRule.ruleType == NightRuleType.None || currentVisitor == null)
        {
            return 0;
        }

        bool violatesRule = EvaluateRuleViolation(currentVisitor, answeredQuestions, activeRule.ruleType);

        if (violatesRule)
        {
            return activeRule.suspicionOnViolation;
        }

        return -activeRule.reliefOnCompliance;
    }

    private bool EvaluateRuleViolation(VisitorData visitor, List<QuestionAnswer> answered, NightRuleType ruleType)
    {
        ObservationProfile profile = visitor.observationProfile;

        switch (ruleType)
        {
            case NightRuleType.ImitatorsAvoidProperNames:
                // Violación: respuestas que evitan nombres propios
                foreach (QuestionAnswer qa in answered)
                {
                    if (qa.responseTag == ResponseTag.Evasive || qa.responseTag == ResponseTag.Contradictory)
                    {
                        string answerLower = qa.answer != null ? qa.answer.ToLowerInvariant() : "";
                        bool hasProperName = ContainsCapitalWord(qa.answer);
                        if (!hasProperName && qa.question != null &&
                            (qa.question.ToLowerInvariant().Contains("quien") ||
                             qa.question.ToLowerInvariant().Contains("nombre") ||
                             qa.question.ToLowerInvariant().Contains("conoces")))
                        {
                            return true;
                        }
                    }
                }
                return false;

            case NightRuleType.ImitatorsDontTrembleInRain:
                // Violación: ropa mojada = SI implica lluvia, pero no tiembla
                if (profile != null && Normalize(profile.wetClothes) == "NO" && Normalize(profile.tremor) == "NO")
                {
                    return true;
                }
                return false;

            case NightRuleType.ImitatorsRepeatPhraseStructures:
                // Violación: respuestas repetitivas en el perfil
                if (profile != null && Normalize(profile.answers) == "REPETITIVAS")
                {
                    return true;
                }
                return false;

            case NightRuleType.WoundedHumansLieFromFear:
                // Reduce sospecha si hay heridas y evasividad (humano asustado)
                if (profile != null && Normalize(profile.visibleWounds) == "SI"
                    && Normalize(profile.evasiveLook) == "SI")
                {
                    return false; // No viola — es un humano herido con miedo
                }
                return false;

            case NightRuleType.FakeRefugeReference:
                // Violación: menciona refugios que no existen
                foreach (QuestionAnswer qa in answered)
                {
                    if (qa.answer != null && qa.answer.ToLowerInvariant().Contains("refugio")
                        && qa.responseTag == ResponseTag.Contradictory)
                    {
                        return true;
                    }
                }
                return false;

            case NightRuleType.ImitatorsHaveCleanHands:
                // Violación: manos limpias en contexto de caos
                foreach (string clue in visitor.visualClues)
                {
                    if (clue != null && clue.ToLowerInvariant().Contains("manos limpias"))
                    {
                        return true;
                    }
                }
                return false;

            case NightRuleType.ImitatorsAvoidSpecificDetails:
                // Violación: respuestas vagas a preguntas de detalle
                foreach (QuestionAnswer qa in answered)
                {
                    if (qa.responseTag == ResponseTag.Evasive)
                    {
                        return true;
                    }
                }
                return false;

            case NightRuleType.ImitatorsUseNamesButFailPlaces:
                // Violación: usa nombres propios pero falla al recordar lugares
                bool usesNames = false;
                bool failsPlaces = false;
                foreach (QuestionAnswer qa in answered)
                {
                    if (qa.answer != null && ContainsCapitalWord(qa.answer))
                    {
                        usesNames = true;
                    }
                    if (qa.question != null &&
                        (qa.question.ToLowerInvariant().Contains("donde") ||
                         qa.question.ToLowerInvariant().Contains("lugar") ||
                         qa.question.ToLowerInvariant().Contains("venis")) &&
                        (qa.responseTag == ResponseTag.Evasive || qa.responseTag == ResponseTag.Contradictory))
                    {
                        failsPlaces = true;
                    }
                }
                return usesNames && failsPlaces;

            default:
                return false;
        }
    }

    private SuspicionLevel ScoreToLevel(int score)
    {
        if (score >= thresholdHigh)
        {
            return SuspicionLevel.High;
        }

        if (score >= thresholdMedium)
        {
            return SuspicionLevel.Medium;
        }

        if (score >= thresholdLow)
        {
            return SuspicionLevel.Low;
        }

        return SuspicionLevel.Unknown;
    }

    private int CalculateAudioScore(ObservationProfile raw)
    {
        if (raw == null) return 0;
        int score = 0;
        string voice = Normalize(raw.voiceTone);
        string breath = Normalize(raw.breathingPattern);

        // Suspicious voice patterns
        if (voice == "VOZ SIN AIRE" || voice == "VOZ DEMASIADO CALMA" || voice == "REPITE CADENCIA")
        {
            score += 2;
        }
        else if (voice == "TIEMBLA AL HABLAR")
        {
            score -= 1; // Sounds human/afraid
        }

        // Suspicious breathing
        if (breath == "IRREGULAR" || breath == "AUSENTE" || breath == "MECANICA")
        {
            score += 2;
        }
        else if (breath == "AGITADA")
        {
            score -= 1; // Sounds human/scared
        }

        return score;
    }

    private int CalculateThermalScore(ObservationProfile raw)
    {
        if (raw == null) return 0;
        string temp = Normalize(raw.bodyTemperature);

        if (temp == "FRIA" || temp == "IRREGULAR")
        {
            return 2;
        }
        if (temp == "ERROR" || temp == "NO MEDIBLE")
        {
            return 1; // Inconclusive but slightly suspicious
        }

        return 0; // NORMAL = no suspicion from thermal
    }

    private void SetLevel(SuspicionLevel level)
    {
        if (CurrentLevel != level)
        {
            CurrentLevel = level;
            SuspicionChanged?.Invoke(level);
        }
    }

    private static string Normalize(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "" : value.Trim().ToUpperInvariant();
    }

    private static bool ContainsCapitalWord(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string[] words = text.Split(' ', '.', ',', ';');
        foreach (string word in words)
        {
            if (word.Length > 1 && char.IsUpper(word[0]))
            {
                // Ignora inicio de oración (primera palabra)
                return true;
            }
        }

        return false;
    }
}
