using System;
using System.Collections.Generic;

/// <summary>
/// Registro de cada visitante procesado durante la partida.
/// Permite al jugador revisar decisiones pasadas y aprender patrones.
/// </summary>
[Serializable]
public class VisitorRecord
{
    public int nightNumber;
    public string visitorName;
    public bool wasAccepted;
    public SuspicionLevel suspicionAtDecision;
    public List<string> observedClues = new List<string>();
    public List<string> questionsAsked = new List<string>();
    public string decisionFeedback;
    public string timestamp;

    public VisitorRecord(int night, string name, bool accepted, SuspicionLevel suspicion)
    {
        nightNumber = night;
        visitorName = name;
        wasAccepted = accepted;
        suspicionAtDecision = suspicion;
        timestamp = DateTime.Now.ToString("HH:mm");
    }
}
