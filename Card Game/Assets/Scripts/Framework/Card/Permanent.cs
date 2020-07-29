using UnityEngine;
using System.Collections;
using System;

// structures and creatures
public abstract class Permanent : MonoBehaviour
{
    public Tile tile { get; set; }
    public Player controller { get; set; }
    public CounterController Counters { get; private set; }
    public Card SourceCard { get; private set; }
    public int BaseHealth { get { return Stats.Stats[StatType.BaseHealth]; } set { Stats.setValue(StatType.BaseHealth, value); needToSync = true; } }
    public int Health { get { return Stats.Stats[StatType.Health]; } set { Stats.setValue(StatType.Health, value); needToSync = true; } }

    public StatsContainer Stats { get; private set; }

    protected bool needToSync = false;

    protected void Awake()
    {
        Counters = GetComponentInChildren<CounterController>();
        //statsScript = GetComponent<StructureStatsGetter>();
        SourceCard = GetComponent<Card>();
        Stats = GetComponent<StatsContainer>();
        enabled = false;
    }

    #region Events
    public event EventHandler<OnDefendArgs> E_OnDefend;
    public void TriggerOnDefendEvents(object sender, OnDefendArgs args) { if (E_OnDefend != null) E_OnDefend.Invoke(sender, args); }

    public event EventHandler<CounterAddedArgs> E_CounterAdded;
    public class CounterAddedArgs : EventArgs { public CounterType counterKind; }
    public void TriggerOnCounterAddedEvents(object sender, CounterAddedArgs args) { E_CounterAdded?.Invoke(sender, args); }
    #endregion

    public void checkDeath()
    {
        if (Health <= 0)
            GameManager.Get().kill(this);
    }

    public void setStatWithoutSyncing(StatType type, int value)
    {
        Stats.Stats[type] = value;
        Stats.raiseEvent();
    }
}
