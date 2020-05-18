using System;
using System.Collections;
using System.Collections.Generic;
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

    public void takeDamage(int amount)
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
            GameManager.Get().destroyStructure(this);
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
            GameManager.Get().destroyStructure(this);
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
        return cardName;
    }

    public void sendToGrave()
    {
        sourceCard.isStructure = false;
        resetToBaseStats();
        sourceCard.moveToCardPile(owner.graveyard);
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
        if (GameManager.Get().activePlayer != controller || controller.locked)
            return;
        if (getEffect() == null)
            return;
        GameManager.Get().setUpStructureEffect(this);
    }

    private void OnMouseEnter()
    {
        GameManager.Get().getCardViewer().setCard(sourceCard);
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

    public bool hasKeyword(CardKeywords keyword)
    {
        return sourceCard.hasKeyword(keyword);
    }

    public void addKeyword(CardKeywords keyword)
    {
        sourceCard.addKeyword(keyword);
    }

    public Vector2 getCoordinates()
    {
        return new Vector2(tile.x, tile.y);
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
    }
    public void removeCounters(CounterClass counterType, int amount)
    {
        counterController.removeCounters(counterType, amount);
    }
    public int hasCounter(CounterClass counterType)
    {
        return counterController.hasCounter(counterType);
    }

    // MAY BE OVERWRITTEN
    public virtual void onCreatureAdded(Creature c){ }
    public virtual void onCreatureRemoved(Creature c) { }
    public virtual void onAnyStructurePlayed(Structure s) { }
    public virtual void onAnyCreaturePlayed(Structure s) { }
    public virtual void onAnyCreatureDeath(Creature c) { }
    public virtual void onAnyStructureDeath(Structure s) { }
    public virtual void onAnySpellCast(SpellCard spell) { }
    public virtual void onPlaced() { }
    public virtual void onRemoved() { }
    public virtual void onDefend() { }
    public virtual void onDamaged() { }
    public virtual void onTurnStart() { }
    public virtual Effect getEffect() { return null; }
    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this structure then do them in this method. Return true if can be played
    public virtual List<Tag> getTags() { return new List<Tag>(); }


    // MUST BE OVERWRITTEN
    public abstract bool canDeployFrom();
    public abstract bool canWalkOn();
    public abstract int getCardId();

}
