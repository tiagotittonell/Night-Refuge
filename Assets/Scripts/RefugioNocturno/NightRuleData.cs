using System;
using UnityEngine;

public enum NightRuleType
{
    None,
    ImitatorsAvoidProperNames,
    ImitatorsDontTrembleInRain,
    ImitatorsRepeatPhraseStructures,
    WoundedHumansLieFromFear,
    FakeRefugeReference,
    ImitatorsHaveCleanHands,
    ImitatorsAvoidSpecificDetails
}

[Serializable]
public class NightRuleData
{
    [TextArea(2, 4)]
    public string description;
    public NightRuleType ruleType = NightRuleType.None;

    [Header("Pesos de sospecha por regla")]
    [Tooltip("Puntos de sospecha si el visitante viola esta regla")]
    [Range(0, 5)]
    public int suspicionOnViolation = 2;

    [Tooltip("Puntos que reduce sospecha si cumple la regla")]
    [Range(0, 3)]
    public int reliefOnCompliance = 1;
}
