using UnityEngine;

/// <summary>
/// Filters observation data based on lamp status and sensor upgrades.
/// Generates a deterministic "observed profile" per visitor that both
/// DialogueUI and SuspicionSystem use — ensuring the player sees the
/// same information the game uses to calculate suspicion.
/// </summary>
public static class ObservationFilter
{
    private const float NoiseChanceWithoutLamp = 0.30f;
    private const float NoiseChanceWithLamp = 0.08f;

    private static readonly string[] NoiseLabels = new[]
    {
        "INCONCLUSO",
        "NO VISIBLE",
        "FALLA DE LUZ"
    };

    /// <summary>
    /// Creates a filtered copy of the observation profile.
    /// Fields may be replaced with noise labels if the lamp is not active.
    /// Seeded by visitor name + night number for determinism (no flickering).
    /// </summary>
    public static ObservationProfile Filter(
        ObservationProfile raw,
        int nightNumber,
        string visitorName,
        bool hasLamp,
        bool blackoutActive)
    {
        if (raw == null)
        {
            return new ObservationProfile();
        }

        // If blackout active, lamp doesn't help
        bool lampEffective = hasLamp && !blackoutActive;
        float noiseChance = lampEffective ? NoiseChanceWithLamp : NoiseChanceWithoutLamp;

        // Deterministic seed based on visitor identity and night
        int seed = (visitorName ?? "").GetHashCode() ^ (nightNumber * 7919);
        System.Random rng = new System.Random(seed);

        ObservationProfile filtered = new ObservationProfile
        {
            wetClothes = MaybeNoise(raw.wetClothes, noiseChance, rng),
            tremor = MaybeNoise(raw.tremor, noiseChance, rng),
            evasiveLook = MaybeNoise(raw.evasiveLook, noiseChance, rng),
            visibleWounds = MaybeNoise(raw.visibleWounds, noiseChance, rng),
            behavior = MaybeNoiseBehavior(raw.behavior, noiseChance, rng),
            answers = raw.answers, // Answers are heard, not visual — no noise
            voiceTone = raw.voiceTone,
            breathingPattern = raw.breathingPattern,
            bodyTemperature = raw.bodyTemperature
        };

        return filtered;
    }

    private static string MaybeNoise(string value, float noiseChance, System.Random rng)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (rng.NextDouble() < noiseChance)
        {
            return NoiseLabels[rng.Next(NoiseLabels.Length)];
        }

        return value;
    }

    private static string MaybeNoiseBehavior(string value, float noiseChance, System.Random rng)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Behavior is slightly harder to obscure — halve the noise chance
        if (rng.NextDouble() < noiseChance * 0.5f)
        {
            return "INCONCLUSO";
        }

        return value;
    }
}
