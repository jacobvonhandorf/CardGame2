using UnityEngine;
using System.Collections;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine.Events;

// structures and creatures
public abstract class Permanent : MonoBehaviour, IScriptPermanent
{
    public Tile Tile { get; set; }
    public Vector2 Coordinates { get { return new Vector2(Tile.X, Tile.Y); } }
    public Player Controller { get; set; }
    public CounterController Counters { get; private set; }
    public Card SourceCard { get; private set; }
    public int BaseHealth { get { return (int)Stats.StatList[StatType.BaseHealth]; } set { Stats.SetValue(StatType.BaseHealth, value); needToSync = true; } }
    public int Health { get { return (int)Stats.StatList[StatType.Health]; } set { Stats.SetValue(StatType.Health, value); needToSync = true; } }
    public StatsContainer Stats { get { return SourceCard.Stats; } }
    public List<ActivatedEffect> ActivatedEffects { get; } = new List<ActivatedEffect>();
    public PermanentCardVisual CardVisual { get { return (PermanentCardVisual)SourceCard.CardVisuals; } }
    IScriptCard IScriptPermanent.SourceCard => SourceCard;
    IScriptPlayer IScriptPermanent.Controller { get { return Controller; } set { Controller = (Player)value; } }
    IScriptTile IScriptPermanent.Tile => Tile;
    protected bool needToSync = false;
    public bool CanDeployFrom { get; set; }

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

    public UnityEvent<Card> E_OnDamaged { get; } = new CardEvent(); // card passed is the source of the damage
    public UnityEvent E_OnDeployed { get; } = new UnityEvent();
    #endregion

    public abstract void ResetForNewTurn();

    public void CheckDeath()
    {
        if (Health <= 0)
            GameManager.Instance.kill(this);
    }

    public void TakeDamage(int damage, Card source)
    {
        if (damage == 0) // dealing 0 damage is illegal :)
            return;

        // check for ward on adjacent allies
        List<Tile> adjacentTiles = Tile.AdjacentTiles;
        foreach (Tile t in adjacentTiles)
        {
            if (t.Creature != null && t.Creature.HasKeyword(Keyword.Ward))
            {
                t.Creature.TakeWardDamage(damage);
                return;
            }
        }

        // subtract armored damage
        if (HasKeyword(Keyword.Armored1))
            damage--;
        TakeDamageActual(damage, source);
    }
    public void TakeWardDamage(int damage)
    {
        if (HasKeyword(Keyword.Armored1))
            damage--;
        TakeDamageActual(damage, null);
    }
    private void TakeDamageActual(int damage, Card source)
    {
        GameManager.Instance.showDamagedText(transform.position, damage);
        Health -= damage;
        E_OnDamaged.Invoke(source);
        if (Health <= 0)
            GameManager.Instance.kill(this);
    }

    public bool HasTag(Tag tag) => SourceCard.Tags.Contains(tag);
    public bool IsType(CardType type) => SourceCard.IsType(type);

    public void SetStatWithoutSyncing(StatType type, int value)
    {
        Stats.StatList[type] = value;
        Stats.RaiseEvent();
    }

    public void RemoveFromCurrentTile()
    {
        if (Tile != null)
        {
            Tile.Permanent = null;
            Tile = null;
        }
        else
        {
            Debug.Log("Trying to remove from tile while not on tile");
        }
    }

    public void UpdateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
            CardVisual.SetIsAlly(GameManager.Instance?.ActivePlayer == Controller);
        else
            CardVisual.SetIsAlly(NetInterface.Get().localPlayer == Controller);
    }

    public void OnCountersAdded(CounterType counterType, int amount)
    {
        if (GameManager.gameMode == GameManager.GameMode.online)
            SyncCounters(counterType);
        TriggerOnCounterAddedEvents(this, new CounterAddedArgs() { counterKind = counterType });
    }
    public void SyncCounters(CounterType counterType)
    {
        NetInterface.Get().SyncCounterPlaced(SourceCard, counterType, Counters.AmountOf(counterType));
    }
    // used by net interface for syncing
    public void RecieveCountersPlaced(CounterType counterType, int newCounters)
    {
        int currentCounters = Counters.AmountOf(counterType);
        if (currentCounters > newCounters)
            Counters.Remove(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            Counters.Add(counterType, newCounters - currentCounters);
        else
            Debug.Log("Trying to set counters to a value it's already set to. This shouldn't happen under normal circumstances");
    }

    protected void SetCoordinates(Tile tile)
    {
        TransformStruct ts = new TransformStruct(SourceCard.TransformManager.transform);
        ts.position = tile.transform.position;
        //ts.position = tile.GetComponent<RectTransform>().rect.center;
        ts.position.x += 45; // these adjustments are needed because tile pivot is in the corner
        ts.position.y -= 40; // if tile size is changed these need to change too
        ts.useLocalPosition = false;
        SourceCard.TransformManager.SetTransform(ts);
        //SourceCard.TransformManager.MoveToInformativeAnimation(ts);
    }

    #region Keywords
    public void AddKeyword(Keyword k) => SourceCard.AddKeyword(k);
    public void RemoveKeyword(Keyword k) => SourceCard.RemoveKeyword(k);
    public bool HasKeyword(Keyword k) => SourceCard.HasKeyword(k);
    public ReadOnlyCollection<Keyword> KeywordList => SourceCard.KeywordList;
    #endregion
}
