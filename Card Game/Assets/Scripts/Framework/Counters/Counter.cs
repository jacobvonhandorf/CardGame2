using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// what card script should be able to use
// creature.addCounter(Counters.counterName);
// creature needs to add, remove and check for counters

public static class Counters
{
    public static CounterClass build { get; } = new CounterClass.BuildCounter();
    public static CounterClass well { get; } = new CounterClass.WellCounter();

    // when adding a new counter add it to this map for netcode to work with it
    public static Dictionary<int, CounterClass> counterMap = new Dictionary<int, CounterClass>()
    {
        { build.id(), build },
        { well.id(), well }
    };
}

public abstract class CounterClass
{
    public abstract Color borderColor();
    public abstract Color fillColor();
    public abstract int id();
    public abstract string tooltip();

    public class BuildCounter : CounterClass
    {
        public override Color borderColor() => new Color(0, 0, 0);
        public override Color fillColor() => new Color(1, 1, 1);
        public override int id() => 1;
        public override string tooltip() => "Used to make basic structures";
    }
    public class WellCounter : CounterClass
    {
        public override Color borderColor() => new Color(.047f, .36f, .678f);
        public override Color fillColor() => new Color(.039f, .714f, .75f);
        public override int id() => 2;
        public override string tooltip() => "Spend 3 to gain 1 mana";
    }
}

public class CounterList
{
    protected Dictionary<CounterClass, int> counterAmounts = new Dictionary<CounterClass, int>();

    public virtual void addCounter(CounterClass counterType)
    {
        addCounters(counterType, 1);
    }
    public virtual void addCounters(CounterClass counterType, int amount)
    {
        if (counterAmounts.ContainsKey(counterType))
        {
            counterAmounts[counterType] += amount;
        }
        else
        {
            counterAmounts.Add(counterType, amount);
        }
    }
    public virtual void removeCounter(CounterClass counterType)
    {
        if (counterAmounts.ContainsKey(counterType))
        {
            counterAmounts[counterType]--;
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
    public virtual void removeCounters(CounterClass counterType, int amount)
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
    public int hasCounter(CounterClass counterType)
    {
        if (counterAmounts.TryGetValue(counterType, out int value))
        {
            return value;
        }
        else
        {
            return 0;
        }
    }
    public void clear()
    {
        counterAmounts.Clear();
    }
}