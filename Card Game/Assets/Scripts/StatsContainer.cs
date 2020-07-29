using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class StatsContainer : MonoBehaviour
{
    public Dictionary<StatType, int> Stats { get; private set; } = new Dictionary<StatType, int>();

    public event EventHandler E_OnStatsChanged;

    public void raiseEvent()
    {
        E_OnStatsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void setValue(StatType type, int value)
    {
        if (!Stats.ContainsKey(type))
            Stats.Add(type, value);
        else
            Stats[type] = value;
        raiseEvent();
    }

    public void addType(StatType type)
    {
        if (!Stats.ContainsKey(type))
            Stats.Add(type, -1);
    }
}
