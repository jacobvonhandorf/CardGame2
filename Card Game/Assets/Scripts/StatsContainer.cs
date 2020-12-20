using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

// Holds a map of stats and has an event that triggers when one of them are changed
public class StatsContainer : IHaveReadableStats
{
    public Dictionary<StatType, object> StatList { get; } = new Dictionary<StatType, object>();
    public UnityEvent E_OnStatChange { get; } = new UnityEvent();
    //public event EventHandler E_OnStatsChanged;

    public void RaiseEvent()
    {
        //E_OnStatsChanged?.Invoke(this, EventArgs.Empty);
        E_OnStatChange.Invoke();
    }

    public void SetValue(StatType type, object value)
    {
        if (!StatList.ContainsKey(type))
            StatList.Add(type, value);
        else
            StatList[type] = value;
        RaiseEvent();
    }

    public T GetValue<T>(StatType type)
    {
        if (StatList.TryGetValue(type, out object value))
            return (T)value;
        else
            return default;
    }

    public void AddType(StatType type)
    {
        if (!StatList.ContainsKey(type))
            StatList.Add(type, -1);
    }

    public object GetValue(StatType type)
    {
        if (StatList.TryGetValue(type, out object value))
            return value;
        else
            return null;
    }

    public bool TryGetValue(StatType statType, out object value) => StatList.TryGetValue(statType, out value);
}
