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
    [SerializeField] public List<EmptyHandler> activatedEffects { get; } = new List<EmptyHandler>();

    [SerializeField] protected StructureStatsGetter statsScript;

    #region Events
    public event EventHandler E_OnDeployed;
    public void TriggerOnDeployEvents(object sender) { E_OnDeployed?.Invoke(sender, EventArgs.Empty); }
    public event EventHandler E_OnLeavesField;
    public void TriggerOnLeavesField(object sender) { E_OnLeavesField?.Invoke(sender, EventArgs.Empty); }
    #endregion

    public new void Awake()
    {
        base.Awake();
        statsScript = GetComponent<StructureStatsGetter>();
        Stats.addType(StatType.Health);
        Stats.addType(StatType.BaseHealth);
    }

    public void takeDamage(int amount, Card source)
    {
        Health -= amount;
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void sendToGrave(Card source)
    {
        resetToBaseStats();
        SourceCard.moveToCardPile(SourceCard.owner.graveyard, null);
        SourceCard.removeGraphicsAndCollidersFromScene();
    }

    public void resetToBaseStats()
    {
        Health = BaseHealth;
    }
    public void resetToBaseStatsWithoutSyncing()
    {
        setStatWithoutSyncing(StatType.Health, BaseHealth);
    }
    public void recieveStatsFromNet(int hp, int bhp, Player ctrl)
    {
        setStatWithoutSyncing(StatType.Health, hp);
        setStatWithoutSyncing(StatType.BaseHealth, bhp);

        Controller = ctrl;
    }

    private void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (GameManager.Get().activePlayer != Controller || Controller.isLocked())
            return;
        if (getEffect() == null)
            return;
        GameManager.Get().setUpStructureEffect(this);
    }

    private bool hovered = false;
    private void OnMouseEnter()
    {
        if (!enabled)
            return;
        hovered = true;
        SourceCard.addToCardViewer(GameManager.Get().getCardViewer());
        StartCoroutine(checkHoverForTooltips());
    }
    private void OnMouseExit()
    {
        if (!enabled)
            return;
        hovered = false;
        foreach (CardViewer cv in SourceCard.viewersDisplayingThisCard)
            cv.clearToolTips();

    }
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
        foreach (CardViewer viewer in SourceCard.viewersDisplayingThisCard)
        {
            if (viewer != null)
            {
                viewer.showToolTips(SourceCard.toolTipInfos);
            }
        }
    }

    // Returns true if this card has the the tag passed to this method
    public bool hasTag(Tag tag)
    {
        return SourceCard.Tags.Contains(tag);
    }

    #region Keyword
    public bool hasKeyword(Keyword keyword)
    {
        return SourceCard.hasKeyword(keyword);
    }
    public void addKeyword(Keyword keyword)
    {
        SourceCard.addKeyword(keyword);
    }
    public ReadOnlyCollection<Keyword> getKeywordList()
    {
        return SourceCard.getKeywordList();
    }
    public void removeKeyword(Keyword keyword)
    {
        SourceCard.removeKeyword(keyword);
    }
    #endregion

    public Vector2 getCoordinates()
    {
        return new Vector2(tile.x, tile.y);
    }
    public Player getController() => Controller;
    public void updateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            statsScript.setAsAlly(GameManager.Get().activePlayer == Controller);
        }
        else
        {
            statsScript.setAsAlly(NetInterface.Get().getLocalPlayer() == Controller);
        }
    }
    public void resetForNewTurn()
    {
        updateFriendOrFoeBorder();
    }
    #region Counters
    public void OnCountersAdded(CounterType counterType, int amount)
    {
        syncCounters(counterType);
        Debug.LogError("unimplemented");
    }
    public void syncCounters(CounterType counterType)
    {
        NetInterface.Get().syncCounterPlaced(SourceCard, counterType, Counters.amountOf(counterType));
    }
    // used by net interface for syncing
    public void recieveCountersPlaced(CounterType counterType, int newCounters)
    {
        int currentCounters = Counters.amountOf(counterType);
        if (currentCounters > newCounters)
            Counters.remove(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            Counters.add(counterType, newCounters - currentCounters);
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
