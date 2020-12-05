using UnityEngine;
using System.Collections;
using System;
using static Card;

// structures and creatures
public abstract class Permanent : MonoBehaviour
{
    public Tile Tile { get; set; }
    public Vector2 Coordinates { get { return new Vector2(Tile.x, Tile.y); } }
    public Player Controller { get; set; }
    public CounterController Counters { get; private set; }
    public Card SourceCard { get; private set; }
    public int BaseHealth { get { return (int)Stats.StatList[StatType.BaseHealth]; } set { Stats.SetValue(StatType.BaseHealth, value); needToSync = true; } }
    public int Health { get { return (int)Stats.StatList[StatType.Health]; } set { Stats.SetValue(StatType.Health, value); needToSync = true; } }
    public StatsContainer Stats { get { return SourceCard.Stats; } }

    protected bool needToSync = false;

    protected void Awake()
    {
        Counters = GetComponentInChildren<CounterController>();
        SourceCard = GetComponent<Card>();
        enabled = false;
    }

    #region Events
    public event EventHandler<OnDefendArgs> E_OnDefend;
    public void TriggerOnDefendEvents(object sender, OnDefendArgs args) { if (E_OnDefend != null) E_OnDefend.Invoke(sender, args); }

    public event EventHandler<CounterAddedArgs> E_CounterAdded;
    public class CounterAddedArgs : EventArgs { public CounterType counterKind; }
    public void TriggerOnCounterAddedEvents(object sender, CounterAddedArgs args) { E_CounterAdded?.Invoke(sender, args); }
    #endregion

    public void CheckDeath()
    {
        if (Health <= 0)
            GameManager.Get().kill(this);
    }

    public bool HasTag(Tag tag) => SourceCard.Tags.Contains(tag);
    public bool IsType(CardType type) => SourceCard.IsType(type);

    public void SetStatWithoutSyncing(StatType type, int value)
    {
        Stats.StatList[type] = value;
        Stats.RaiseEvent();
    }
}
