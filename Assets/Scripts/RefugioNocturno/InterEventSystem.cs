using System;
using System.Collections.Generic;
using UnityEngine;

public enum InterEventType
{
    None,
    NoiseInDucts,
    ShelterProtest,
    IntermittentRadio,
    PartialBlackout,
    InteriorDoorKnock,
    AcceptedPersonInfo,
    FalseRumor,
    DistantScream,
    SilenceBreak
}

/// <summary>
/// Un evento breve que ocurre entre visitantes para aumentar tensión
/// y afectar recursos del refugio.
/// </summary>
[Serializable]
public class InterVisitorEvent
{
    public InterEventType eventType = InterEventType.None;

    [TextArea(2, 4)]
    public string narrativeText;

    [Header("Efectos en recursos")]
    public int foodChange;
    public int securityChange;
    public int moraleChange;
    public int populationChange;

    [Header("Condiciones")]
    [Tooltip("Probabilidad de que ocurra (0-1). 1 = siempre ocurre.")]
    [Range(0f, 1f)]
    public float probability = 1f;

    [Tooltip("Seguridad mínima para que pueda ocurrir (-1 = sin límite)")]
    public int minSecurityToTrigger = -1;

    [Tooltip("Solo ocurre si se aceptó al visitante previo")]
    public bool requiresPreviousAccepted;
}

/// <summary>
/// Sistema que gestiona y ejecuta eventos entre visitantes.
/// Se inserta en el flujo entre el fin de un visitante y la aparición del siguiente.
/// </summary>
public class InterEventSystem : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float eventDisplayDuration = 4.5f;

    public event Action<InterVisitorEvent> EventTriggered;
    public event Action EventDismissed;

    private List<InterVisitorEvent> pendingEvents = new List<InterVisitorEvent>();
    private NightData currentNight;
    private bool lastVisitorWasAccepted;
    private int currentSecurityLevel = 5;
    private bool blackoutActiveThisNight;

    public float EventDisplayDuration => eventDisplayDuration;
    public bool IsBlackoutActive => blackoutActiveThisNight;

    public void SetNightEvents(NightData night)
    {
        currentNight = night;
        pendingEvents.Clear();
        blackoutActiveThisNight = false;

        if (night != null && night.interEvents != null)
        {
            pendingEvents.AddRange(night.interEvents);
        }
    }

    public void UpdateContext(bool visitorAccepted, int security)
    {
        lastVisitorWasAccepted = visitorAccepted;
        currentSecurityLevel = security;
    }

    /// <summary>
    /// Intenta seleccionar un evento para mostrar entre visitantes.
    /// Devuelve null si no hay evento válido.
    /// </summary>
    public InterVisitorEvent TryGetNextEvent()
    {
        if (pendingEvents.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < pendingEvents.Count; i++)
        {
            InterVisitorEvent candidate = pendingEvents[i];

            if (!MeetsConditions(candidate))
            {
                continue;
            }

            if (candidate.probability < 1f && UnityEngine.Random.value > candidate.probability)
            {
                continue;
            }

            pendingEvents.RemoveAt(i);
            return candidate;
        }

        return null;
    }

    public void DismissEvent()
    {
        EventDismissed?.Invoke();
    }

    public void NotifyEventTriggered(InterVisitorEvent interEvent)
    {
        // Track blackout state for lamp system
        if (interEvent.eventType == InterEventType.PartialBlackout)
        {
            blackoutActiveThisNight = true;
        }

        EventTriggered?.Invoke(interEvent);
    }

    private bool MeetsConditions(InterVisitorEvent interEvent)
    {
        if (interEvent.requiresPreviousAccepted && !lastVisitorWasAccepted)
        {
            return false;
        }

        if (interEvent.minSecurityToTrigger >= 0 && currentSecurityLevel > interEvent.minSecurityToTrigger)
        {
            return false;
        }

        return true;
    }
}
