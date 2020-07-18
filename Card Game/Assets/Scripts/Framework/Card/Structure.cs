using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Card;

// abstract class for structures as they exist on the field

public abstract class Structure : MonoBehaviour, Damageable
{
    public Player owner;
    public Player controller;
    public Tile tile;
    public StructureCard sourceCard;
    public int effectActionsCost = 0;
    public int effectGoldCost = 0;
    public int effectManaCost = 0;

    // synced
    [SerializeField] private int health;
    [SerializeField] private int baseHealth;

    protected string cardName;
    private bool initialized = false;

    [SerializeField] protected StructureStatsGetter statsScript;
    private CounterController counterController;

    #region Events
    public event EventHandler<OnDefendArgs> E_OnDefend;
    public void TriggerOnDefendEvents(object sender, OnDefendArgs args) { if (E_OnDefend != null) E_OnDefend.Invoke(sender, args); }
    #endregion

    private void Awake()
    {
        counterController = sourceCard.getCounterController();
    }

    public void initialize()
    {
        if (initialized)
        {
            Debug.Log("Structures only need to be initialize once");
            return;
        }

        statsScript.setStructureStats(this);

        baseHealth = health;

        initialized = true;
    }

    public Transform getRootTransform()
    {
        return statsScript.getRootTransform();
    }

    public void takeDamage(int amount, Card source)
    {
        addHealth(-amount);
    }

    public void addHealth(int amount)
    {
        Debug.Log("adding health: " + amount);
        Debug.Log("Health before damage " + health);
        
        health += amount;
        statsScript.setHealth(health, baseHealth);
        Debug.Log("Health after damage " + health);
        if (health <= 0)
            GameManager.Get().destroyStructure(this, null);
    }
    public int getHealth()
    {
        return health;
    }
    public void setHealth(int amount)
    {
        health = amount;
        statsScript.setHealth(health, baseHealth);
        if (health <= 0)
            GameManager.Get().destroyStructure(this, null);
    }
    public void setBaseHealth(int amount)
    {
        baseHealth = amount;
        statsScript.setHealth(health, baseHealth);
    }
    public int getBaseHealth()
    {
        return baseHealth;
    }
    public string getCardName()
    {
        return sourceCard.getCardName();
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void sendToGrave(Card source)
    {
        sourceCard.isStructure = false;
        resetToBaseStats();
        sourceCard.moveToCardPile(owner.graveyard, null);
        sourceCard.removeGraphicsAndCollidersFromScene();
    }

    public void resetToBaseStats()
    {
        setHealth(baseHealth);
    }

    public void resetToBaseStatsWithoutSyncing()
    {
        health = baseHealth;
        statsScript.updateAllStats(this);
    }

    private void OnMouseUpAsButton()
    {
        if (GameManager.Get().activePlayer != controller || controller.isLocked())
            return;
        if (getEffect() == null)
            return;
        GameManager.Get().setUpStructureEffect(this);
    }

    private bool hovered = false;
    private void OnMouseEnter()
    {
        hovered = true;
        sourceCard.addToCardViewer(GameManager.Get().getCardViewer());
        StartCoroutine(checkHoverForTooltips());
    }
    private void OnMouseExit()
    {
        hovered = false;
        foreach (CardViewer cv in sourceCard.viewersDisplayingThisCard)
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
        foreach (CardViewer viewer in sourceCard.viewersDisplayingThisCard)
        {
            if (viewer != null)
            {
                viewer.showToolTips(sourceCard.toolTipInfos);
            }
        }
    }

    // used when instantiating from resources. Do not touch otherwise
    public void setStatsScript(StructureStatsGetter statsScript)
    {
        this.statsScript = statsScript;
    }

    /*
     * Returns true if this card has the the tag passed to this method
     */
    public bool hasTag(Tag tag)
    {
        return sourceCard.hasTag(tag);
    }

    /*
     * returns true if this card is the type passed to it
     */
    public bool isType(CardType type)
    {
        return sourceCard.isType(type);
    }
    #region Keyword
    public bool hasKeyword(Keyword keyword)
    {
        return sourceCard.hasKeyword(keyword);
    }
    public void addKeyword(Keyword keyword)
    {
        sourceCard.addKeyword(keyword);
    }
    public ReadOnlyCollection<Keyword> getKeywordList()
    {
        return sourceCard.getKeywordList();
    }
    public void removeKeyword(Keyword keyword)
    {
        sourceCard.removeKeyword(keyword);
    }
    #endregion

    public Vector2 getCoordinates()
    {
        return new Vector2(tile.x, tile.y);
    }
    public Player getController()
    {
        return controller;
    }

    public void updateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            statsScript.setAsAlly(GameManager.Get().activePlayer == controller);
        }
        else
        {
            statsScript.setAsAlly(NetInterface.Get().getLocalPlayer() == controller);
        }
    }

    public void resetForNewTurn()
    {
        updateFriendOrFoeBorder();
    }

    public Card getSourceCard()
    {
        return sourceCard;
    }

    public void addCounters(CounterClass counterType, int amount)
    {
        counterController.addCounters(counterType, amount);
        syncCounters(counterType);
    }
    public void removeCounters(CounterClass counterType, int amount)
    {
        counterController.removeCounters(counterType, amount);
        syncCounters(counterType);
    }
    public int hasCounter(CounterClass counterType)
    {
        return counterController.hasCounter(counterType);
    }
    public void syncCounters(CounterClass counterType)
    {
        NetInterface.Get().syncCounterPlaced(sourceCard, counterType, counterController.hasCounter(counterType));
    }
    // used by net interface for syncing
    public void recieveCountersPlaced(CounterClass counterType, int newCounters)
    {
        int currentCounters = counterController.hasCounter(counterType);
        if (currentCounters > newCounters)
            counterController.removeCounters(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            counterController.addCounters(counterType, newCounters - currentCounters);
        else
            Debug.LogError("Trying to set counters to a value it's already set to. This shouldn't happen under normal circumstances");
    }

    // MAY BE OVERWRITTEN
    public virtual void onCreatureRemoved(Creature c) { }
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
    public virtual List<Keyword> getInitialKeywords() { return new List<Keyword>(); }
    public virtual bool canDeployFrom() { return true; }

    // MUST BE OVERWRITTEN
    public abstract int cardId { get; }

}
