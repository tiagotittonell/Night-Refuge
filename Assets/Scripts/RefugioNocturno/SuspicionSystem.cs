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
        int score = CalculateObservationScore(currentVisitor.observationProfile);
        score += CalculateResponseScore();
        score += CalculateNightRuleScore();

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
        if (behavior != "NORMAL" && behavior != "ASUSTADA" && behavior != "ASUSTADO"
            && behavior != "NERVIOSA" && behavior != "NERVIOSO" && behavior != "AGITADA")
        {
            score += weightAbnormalBehavior;
        }

        // Respuestas incoherentes en el perfil
        if (answers != "COHERENTES")
        {
            score += weightIncoherentAnswers;
        }

        return score;
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
