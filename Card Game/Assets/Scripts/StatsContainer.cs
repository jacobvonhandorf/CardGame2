using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System;

public class StatsContainer : MonoBehaviour
{
    public Dictionary<StatType, object> Stats { get; private set; } = new Dictionary<StatType, object>();

    public event EventHandler E_OnStatsChanged;

    public void raiseEvent()
    {
        E_OnStatsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void setValue(StatType type, object value)
    {
        if (!Stats.ContainsKey(type))
            Stats.Add(type, value);
        else
            Stats[type] = value;
        raiseEvent();
    }

    public T getValue<T>(StatType type)
    {
        if (Stats.TryGetValue(type, out object value))
            return (T)value;
        else
            return default;
    }

    public void addType(StatType type)
    {
        if (!Stats.ContainsKey(type))
            Stats.Add(type, -1);
    }
}
