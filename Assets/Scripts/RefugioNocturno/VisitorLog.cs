using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mantiene un registro de todos los visitantes procesados durante la partida.
/// Permite consultar el historial para detectar patrones y tomar mejores decisiones.
/// </summary>
public class VisitorLog : MonoBehaviour
{
    private readonly List<VisitorRecord> records = new List<VisitorRecord>();

    public IReadOnlyList<VisitorRecord> Records => records;
    public int TotalVisitors => records.Count;

    public void RegisterVisitor(VisitorRecord record)
    {
        if (record != null)
        {
            records.Add(record);
        }
    }

    public List<VisitorRecord> GetRecordsForNight(int nightNumber)
    {
        List<VisitorRecord> result = new List<VisitorRecord>();
        foreach (VisitorRecord record in records)
        {
            if (record.nightNumber == nightNumber)
            {
                result.Add(record);
            }
        }
        return result;
    }

    public void Clear()
    {
        records.Clear();
    }
}
