using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Card;

// abstract class for structures as they exist on the field
public class Structure : Permanent, Damageable, ICanReceiveCounters
{
    protected string cardName;
    private bool initialized = false;
    public PermanentCardVisual CardVisual { get { return (PermanentCardVisual)SourceCard.CardVisuals; } }
    [SerializeField] public List<EmptyHandler> activatedEffects { get; } = new List<EmptyHandler>();

    public Vector2 Coordinates => new Vector2(Tile.x, Tile.y);

    #region Events
    public event EventHandler E_OnDeployed;
    public void TriggerOnDeployEvents(object sender) { E_OnDeployed?.Invoke(sender, EventArgs.Empty); }
    public event EventHandler E_OnLeavesField;
    public void TriggerOnLeavesField(object sender) { E_OnLeavesField?.Invoke(sender, EventArgs.Empty); }
    #endregion

    public new void Awake()
    {
        base.Awake();
        Stats.AddType(StatType.Health);
        Stats.AddType(StatType.BaseHealth);
    }

    public void TakeDamage(int amount, Card source)
    {
        Health -= amount;
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void sendToGrave(Card source)
    {
        resetToBaseStats();
        SourceCard.MoveToCardPile(SourceCard.owner.Graveyard, null);
        SourceCard.removeGraphicsAndCollidersFromScene();
    }

    public void resetToBaseStats()
    {
        Health = BaseHealth;
    }
    public void resetToBaseStatsWithoutSyncing()
    {
        SetStatWithoutSyncing(StatType.Health, BaseHealth);
    }
    public void recieveStatsFromNet(int hp, int bhp, Player ctrl)
    {
        SetStatWithoutSyncing(StatType.Health, hp);
        SetStatWithoutSyncing(StatType.BaseHealth, bhp);

        Controller = ctrl;
    }

    private void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (GameManager.Get().activePlayer != Controller || Controller.IsLocked())
            return;
        if (getEffect() == null)
            return;
        GameManager.Get().setUpStructureEffect(this);
    }

    private bool hovered = false;
    [SerializeField] private float hoverTimeForToolTips = .5f;
    private float timePassed = 0;
    IEnumerator checkHoverForTooltips()
    {
        while (timePassed < hoverTimeForToolTips)
        {
            timePassed += Time.deltaTime;
            if (!hovered)
                yield break;
            else
                yield return null;
        }
        timePassed = 0;
        // if we get here then enough time has passed so tell cardviewers to display tooltips
    }

    public void UpdateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
            CardVisual.SetIsAlly(GameManager.Get().activePlayer == Controller);
        else
            CardVisual.SetIsAlly(NetInterface.Get().localPlayer == Controller);
    }
    public override void ResetForNewTurn()
    {
        UpdateFriendOrFoeBorder();
    }
    #region Counters
    public void OnCountersAdded(CounterType counterType, int amount)
    {
        syncCounters(counterType);
        Debug.LogError("unimplemented");
    }
    public void syncCounters(CounterType counterType)
    {
        NetInterface.Get().SyncCounterPlaced(SourceCard, counterType, Counters.AmountOf(counterType));
    }
    // used by net interface for syncing
    public void recieveCountersPlaced(CounterType counterType, int newCounters)
    {
        int currentCounters = Counters.AmountOf(counterType);
        if (currentCounters > newCounters)
            Counters.Remove(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            Counters.Add(counterType, newCounters - currentCounters);
        else
            Debug.LogError("Trying to set counters to a value it's already set to. This shouldn't happen under normal circumstances");
    }
    #endregion
    #region Overideable
    // MAY BE OVERWRITTEN
    public virtual void onAnyStructurePlayed(Structure s) { }
    public virtual void onAnyCreaturePlayed(Structure s) { }
    public virtual void onAnyCreatureDeath(Creature c) { }
    public virtual void onAnyStructureDeath(Structure s) { }
    public virtual void onPlaced() { }
    public virtual void onRemoved() { }
    public virtual void onDefend() { }
    public virtual void onDamaged() { }
    public virtual void onTurnStart() { }
    public virtual Effect getEffect() { return null; }
    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this structure then do them in this method. Return true if can be played
    public virtual List<Tag> getTags() { return new List<Tag>(); }
    public virtual List<KeywordData> getInitialKeywords() { return new List<KeywordData>(); }
    public virtual bool canDeployFrom() { return true; }
    #endregion
}
