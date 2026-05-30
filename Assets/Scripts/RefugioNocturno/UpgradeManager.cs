using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona los suministros (moneda), mejoras disponibles y compradas.
/// Calcula recompensas al final de cada noche.
/// Aplica efectos de mejoras a los sistemas del juego.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [Header("Economía")]
    [SerializeField] private int startingSupplies = 0;
    [SerializeField] private int rewardPerImitatorRejected = 2;
    [SerializeField] private int rewardPerHumanAccepted = 1;
    [SerializeField] private int bonusHighSecurity = 2;
    [SerializeField] private int bonusHighMorale = 1;
    [SerializeField] private int securityThresholdForBonus = 4;
    [SerializeField] private int moraleThresholdForBonus = 4;

    [Header("Mejoras disponibles")]
    [SerializeField] private List<UpgradeData> availableUpgrades = new List<UpgradeData>();

    private int currentSupplies;
    private readonly List<string> purchasedUpgradeIds = new List<string>();

    public int Supplies => currentSupplies;
    public IReadOnlyList<UpgradeData> AvailableUpgrades => availableUpgrades;
    public IReadOnlyList<string> PurchasedIds => purchasedUpgradeIds;

    public event Action<int> SuppliesChanged;
    public event Action<UpgradeData> UpgradePurchased;

    private void Awake()
    {
        currentSupplies = startingSupplies;

        if (availableUpgrades.Count == 0)
        {
            availableUpgrades = CreateDefaultUpgrades();
        }
    }

    public int CalculateNightReward(NightSummary summary, RefugeStats stats)
    {
        int reward = 0;
        reward += summary.imitatorsRejected * rewardPerImitatorRejected;
        reward += summary.humansAccepted * rewardPerHumanAccepted;

        if (stats != null)
        {
            if (stats.Security >= securityThresholdForBonus)
            {
                reward += bonusHighSecurity;
            }

            if (stats.Morale >= moraleThresholdForBonus)
            {
                reward += bonusHighMorale;
            }
        }

        return reward;
    }

    public void AwardSupplies(int amount)
    {
        currentSupplies += amount;
        SuppliesChanged?.Invoke(currentSupplies);
    }

    public bool CanAfford(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            return false;
        }

        return currentSupplies >= upgrade.cost;
    }

    public bool IsAlreadyPurchased(UpgradeData upgrade)
    {
        if (upgrade == null || upgrade.repeatable)
        {
            return false;
        }

        return purchasedUpgradeIds.Contains(upgrade.id);
    }

    public bool TryPurchase(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            return false;
        }

        if (!CanAfford(upgrade))
        {
            return false;
        }

        if (IsAlreadyPurchased(upgrade))
        {
            return false;
        }

        currentSupplies -= upgrade.cost;
        purchasedUpgradeIds.Add(upgrade.id);
        SuppliesChanged?.Invoke(currentSupplies);
        UpgradePurchased?.Invoke(upgrade);
        return true;
    }

    public bool HasUpgrade(UpgradeEffect effect)
    {
        foreach (UpgradeData upgrade in availableUpgrades)
        {
            if (upgrade.effect == effect && purchasedUpgradeIds.Contains(upgrade.id))
            {
                return true;
            }
        }

        return false;
    }

    public int GetUpgradeValue(UpgradeEffect effect)
    {
        foreach (UpgradeData upgrade in availableUpgrades)
        {
            if (upgrade.effect == effect && purchasedUpgradeIds.Contains(upgrade.id))
            {
                return upgrade.effectValue;
            }
        }

        return 0;
    }

    public List<UpgradeData> GetPurchasableUpgrades()
    {
        List<UpgradeData> result = new List<UpgradeData>();
        foreach (UpgradeData upgrade in availableUpgrades)
        {
            if (!IsAlreadyPurchased(upgrade))
            {
                result.Add(upgrade);
            }
        }
        return result;
    }

    private static List<UpgradeData> CreateDefaultUpgrades()
    {
        return new List<UpgradeData>
        {
            new UpgradeData
            {
                id = "extra_question",
                upgradeName = "Interrogatorio extendido",
                description = "+1 pregunta por visitante.",
                cost = 3,
                effect = UpgradeEffect.ExtraQuestion,
                effectValue = 1,
                drawback = "Cada pregunta consume mas tiempo."
            },
            new UpgradeData
            {
                id = "reinforced_lock",
                upgradeName = "Cerradura reforzada",
                description = "Reduce el daño a seguridad al aceptar imitadores.",
                cost = 5,
                effect = UpgradeEffect.ReinforcedLock,
                effectValue = 1,
                drawback = ""
            },
            new UpgradeData
            {
                id = "reinforced_lamp",
                upgradeName = "Lampara reforzada",
                description = "Observaciones mas claras. Menos resultados INCONCLUSO.",
                cost = 4,
                effect = UpgradeEffect.ReinforcedLamp,
                effectValue = 1,
                drawback = "No funciona si hay corte de luz."
            },
            new UpgradeData
            {
                id = "improved_microphone",
                upgradeName = "Microfono mejorado",
                description = "Revela tono de voz y respiracion del visitante.",
                cost = 4,
                effect = UpgradeEffect.ImprovedMicrophone,
                effectValue = 1,
                drawback = "Puede captar ruido de fondo."
            },
            new UpgradeData
            {
                id = "rationing",
                upgradeName = "Racionamiento",
                description = "Reduce consumo de comida por humano aceptado.",
                cost = 3,
                effect = UpgradeEffect.Rationing,
                effectValue = 1,
                drawback = "Baja la moral ligeramente."
            },
            new UpgradeData
            {
                id = "extra_guard",
                upgradeName = "Guardia extra",
                description = "Reduce perdida de seguridad por imitador.",
                cost = 5,
                effect = UpgradeEffect.ExtraGuard,
                effectValue = 1,
                drawback = "Consume comida extra por noche."
            },
            new UpgradeData
            {
                id = "short_wave_radio",
                upgradeName = "Radio de onda corta",
                description = "Desbloquea preguntas sobre refugios externos y lore.",
                cost = 4,
                effect = UpgradeEffect.ShortWaveRadio,
                effectValue = 1,
                drawback = "A veces transmite informacion falsa."
            },
            new UpgradeData
            {
                id = "thermal_detector",
                upgradeName = "Detector termico defectuoso",
                description = "Muestra temperatura corporal, pero con margen de error.",
                cost = 6,
                effect = UpgradeEffect.ThermalDetector,
                effectValue = 1,
                drawback = "Falla con lluvia intensa."
            },
            new UpgradeData
            {
                id = "operator_coffee",
                upgradeName = "Cafe para el operador",
                description = "Una pregunta extra por noche (una sola vez).",
                cost = 2,
                effect = UpgradeEffect.OperatorCoffee,
                effectValue = 1,
                repeatable = true,
                drawback = ""
            }
        };
    }
}
