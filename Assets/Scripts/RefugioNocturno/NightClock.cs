using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Reloj funcional de la noche. Cada acción consume tiempo.
/// Al llegar a cierta hora, la tensión sube o la noche termina.
/// No es un reloj arcade: el avance es por acción, no en tiempo real.
/// </summary>
public class NightClock : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int startHour = 1;
    [SerializeField] private int startMinute = 30;
    [SerializeField] private int endHour = 5;
    [SerializeField] private int endMinute = 0;

    [Header("Costos de tiempo (minutos)")]
    [SerializeField] private int minutesPerVisitorArrival = 8;
    [SerializeField] private int minutesPerQuestion = 4;
    [SerializeField] private int minutesPerDecision = 3;
    [SerializeField] private int minutesPerInterEvent = 5;

    [Header("Tensión por hora avanzada")]
    [SerializeField] private int tensionThresholdHour = 3;
    [Tooltip("Minutos extra que consumen las acciones después del umbral de tensión")]
    [SerializeField] private int extraMinutesAfterThreshold = 2;

    [Header("UI")]
    [SerializeField] private TMP_Text clockTmpText;
    [SerializeField] private Text clockLegacyText;

    private int currentHour;
    private int currentMinute;
    private bool nightExpired;

    public event Action<string> ClockUpdated;
    public event Action NightTimeExpired;
    public bool IsExpired => nightExpired;
    public string CurrentTimeString => FormatTime(currentHour, currentMinute);

    public void StartNight(int hour, int minute)
    {
        currentHour = hour;
        currentMinute = minute;
        nightExpired = false;
        UpdateUI();
    }

    public void StartNight(string clockTime)
    {
        ParseClockTime(clockTime, out int hour, out int minute);
        StartNight(hour, minute);
    }

    public void ConsumeVisitorArrival()
    {
        AdvanceMinutes(minutesPerVisitorArrival);
    }

    public void ConsumeQuestion()
    {
        AdvanceMinutes(GetEffectiveMinutes(minutesPerQuestion));
    }

    public void ConsumeDecision()
    {
        AdvanceMinutes(GetEffectiveMinutes(minutesPerDecision));
    }

    public void ConsumeInterEvent()
    {
        AdvanceMinutes(minutesPerInterEvent);
    }

    private int GetEffectiveMinutes(int baseMinutes)
    {
        if (currentHour >= tensionThresholdHour)
        {
            return baseMinutes + extraMinutesAfterThreshold;
        }
        return baseMinutes;
    }

    private void AdvanceMinutes(int minutes)
    {
        if (nightExpired)
        {
            return;
        }

        currentMinute += minutes;

        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;
        }

        if (HasReachedEnd())
        {
            nightExpired = true;
            NightTimeExpired?.Invoke();
        }

        UpdateUI();
    }

    private bool HasReachedEnd()
    {
        if (currentHour > endHour)
        {
            return true;
        }

        if (currentHour == endHour && currentMinute >= endMinute)
        {
            return true;
        }

        return false;
    }

    private void UpdateUI()
    {
        string timeText = FormatTime(currentHour, currentMinute);

        if (clockTmpText != null)
        {
            clockTmpText.text = timeText;
        }

        if (clockLegacyText != null)
        {
            clockLegacyText.text = timeText;
        }

        // Also update via auto-binding
        if (clockTmpText == null && clockLegacyText == null)
        {
            EnsureBindings();
            if (clockTmpText != null)
            {
                clockTmpText.text = timeText;
            }
            if (clockLegacyText != null)
            {
                clockLegacyText.text = timeText;
            }
        }

        ClockUpdated?.Invoke(timeText);
    }

    private void EnsureBindings()
    {
        GameObject clockObj = GameObject.Find("ClockStatic");
        if (clockObj != null)
        {
            if (clockTmpText == null)
            {
                clockTmpText = clockObj.GetComponent<TMP_Text>();
            }

            if (clockLegacyText == null)
            {
                clockLegacyText = clockObj.GetComponent<Text>();
            }
        }
    }

    private static string FormatTime(int hour, int minute)
    {
        string period = hour < 12 ? "AM" : "PM";
        int displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);
        return $"{displayHour:D2}:{minute:D2} {period}";
    }

    private static void ParseClockTime(string clockTime, out int hour, out int minute)
    {
        hour = 1;
        minute = 30;

        if (string.IsNullOrWhiteSpace(clockTime))
        {
            return;
        }

        string cleaned = clockTime.Trim().ToUpperInvariant().Replace("AM", "").Replace("PM", "").Trim();
        string[] parts = cleaned.Split(':');

        if (parts.Length >= 2)
        {
            int.TryParse(parts[0], out hour);
            int.TryParse(parts[1].Trim(), out minute);
        }

        // Handle PM
        if (clockTime.ToUpperInvariant().Contains("PM") && hour < 12)
        {
            hour += 12;
        }
    }
}
