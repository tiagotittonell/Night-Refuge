using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Carga y cachea sprites desde Resources/UI/ para uso en paneles runtime.
/// Centraliza el acceso a sprites de upgrade, sospecha, eventos, botones y paneles.
/// </summary>
public static class UISpriteLoader
{
    private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    // --- Upgrade Icons ---
    public static Sprite GetUpgradeIcon(UpgradeEffect effect)
    {
        string path = GetUpgradeIconPath(effect);
        return string.IsNullOrEmpty(path) ? null : Load(path);
    }

    public static Sprite SuppliesCoin => Load("UI/Upgrades/supplies_coin");

    // --- Suspicion Icons ---
    public static Sprite SuspicionLow => Load("UI/Suspicion/suspicion_low");
    public static Sprite SuspicionMedium => Load("UI/Suspicion/suspicion_medium");
    public static Sprite SuspicionHigh => Load("UI/Suspicion/suspicion_high");

    public static Sprite GetSuspicionIcon(SuspicionLevel level)
    {
        switch (level)
        {
            case SuspicionLevel.Low: return SuspicionLow;
            case SuspicionLevel.Medium: return SuspicionMedium;
            case SuspicionLevel.High: return SuspicionHigh;
            default: return null;
        }
    }

    // --- Event Backgrounds ---
    public static Sprite GetEventBackground(InterEventType eventType)
    {
        string path = GetEventBackgroundPath(eventType);
        return string.IsNullOrEmpty(path) ? null : Load(path);
    }

    // --- Buttons ---
    public static Sprite ButtonNormal => Load("UI/Buttons/button_normal");
    public static Sprite ButtonHover => Load("UI/Buttons/button_hover");
    public static Sprite ButtonDisabled => Load("UI/Buttons/button_disabled");

    // --- Panels ---
    public static Sprite PanelFrame => Load("UI/Panels/panel_frame");
    public static Sprite ShopBackground => Load("UI/Shop/shop_background");
    public static Sprite LogBackground => Load("UI/Panels/log_background");

    // --- Core load method ---
    private static Sprite Load(string resourcePath)
    {
        if (cache.TryGetValue(resourcePath, out Sprite cached))
        {
            return cached;
        }

        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
        {
            cache[resourcePath] = sprite;
        }

        return sprite;
    }

    private static string GetUpgradeIconPath(UpgradeEffect effect)
    {
        switch (effect)
        {
            case UpgradeEffect.ExtraQuestion: return "UI/Upgrades/extra_question";
            case UpgradeEffect.ReinforcedLock: return "UI/Upgrades/reinforced_lock";
            case UpgradeEffect.ReinforcedLamp: return "UI/Upgrades/reinforced_lamp";
            case UpgradeEffect.ImprovedMicrophone: return "UI/Upgrades/improved_microphone";
            case UpgradeEffect.Rationing: return "UI/Upgrades/rationing";
            case UpgradeEffect.ShortWaveRadio: return "UI/Upgrades/short_wave_radio";
            case UpgradeEffect.ThermalDetector: return "UI/Upgrades/thermal_detector";
            case UpgradeEffect.ExtraGuard: return "UI/Upgrades/extra_guard";
            case UpgradeEffect.OperatorCoffee: return "UI/Upgrades/operator_coffee";
            default: return null;
        }
    }

    private static string GetEventBackgroundPath(InterEventType eventType)
    {
        switch (eventType)
        {
            case InterEventType.NoiseInDucts: return "UI/Events/event_noise_ducts";
            case InterEventType.PartialBlackout: return "UI/Events/event_blackout";
            case InterEventType.ShelterProtest: return "UI/Events/event_protest";
            default: return "UI/Events/event_blackout_alt";
        }
    }

    /// <summary>
    /// Limpia la caché (útil al reiniciar la partida).
    /// </summary>
    public static void ClearCache()
    {
        cache.Clear();
    }
}
