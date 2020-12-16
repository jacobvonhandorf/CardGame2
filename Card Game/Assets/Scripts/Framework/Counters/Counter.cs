using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Counters
{
    private static Dictionary<CounterType, CounterData> counterData;

    public static CounterData GetData(CounterType type)
    {
        if (counterData == null)
            SetupMap();
        return counterData[type];
    }
    private static void SetupMap()
    {
        counterData = new Dictionary<CounterType, CounterData>();
        CounterData[] data = Resources.LoadAll<CounterData>("");
        foreach (CounterData d in data)
            counterData.Add(d.CType, d);
    }
}

public class CounterList
{
    
    protected Dictionary<CounterType, int> counterAmounts = new Dictionary<CounterType, int>();
    public IReadOnlyDictionary<CounterType, int> CounterMap { get { return counterAmounts; } }

    public void AddCounters(CounterType counterType, int amount)
    {
        if (counterAmounts.ContainsKey(counterType))
            counterAmounts[counterType] += amount;
        else
            counterAmounts.Add(counterType, amount);
    }
    public virtual void RemoveCounters(CounterType counterType, int amount)
    {
        if (counterAmounts.ContainsKey(counterType))
        {
            counterAmounts[counterType] -= amount;
            if (counterAmounts[counterType] == 0)
                counterAmounts.Remove(counterType);
            else if (counterAmounts[counterType] < 0)
                throw new Exception("Trying to remove counter that doesn't exist");
        }
        else
        {
            throw new Exception("Trying to remove counters that don't exist");
        }
    }
    public int AmountOf(CounterType counterType)
    {
        if (counterAmounts.TryGetValue(counterType, out int value))
            return value;
        else
            return 0;
    }
    public void Clear()
    {
        counterAmounts.Clear();
    }
}