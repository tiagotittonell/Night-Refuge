using System;
using UnityEngine;

public enum UpgradeEffect
{
    None,
    ExtraQuestion,
    ReinforcedLock,
    ReinforcedLamp,
    ImprovedMicrophone,
    InternalArchive,
    Rationing,
    ShortWaveRadio,
    ThermalDetector,
    ExtraGuard,
    OperatorCoffee
}

/// <summary>
/// Define una mejora comprable entre noches.
/// </summary>
[Serializable]
public class UpgradeData
{
    public string id;
    public string upgradeName;
    [TextArea(1, 3)]
    public string description;
    public int cost = 3;
    public UpgradeEffect effect = UpgradeEffect.None;
    public bool repeatable = false;

    [Header("Efectos numéricos")]
    [Tooltip("Valor genérico del efecto (ej: +1 pregunta, -1 daño de imitador)")]
    public int effectValue = 1;

    [Header("Desventaja opcional")]
    [TextArea(1, 2)]
    public string drawback;
}
