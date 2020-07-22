using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureCard : Card
{
    public Creature creature;
    [SerializeField] private CreatureStatsGetter creatureStatsScript;
    [SerializeField] private CounterController counterController;
    //public bool isCreature = false; // true when is being treated as a creature. False when treated as card

    public override int cardId => creature.cardId;
    public override CardType getCardType() => CardType.Creature;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllDeployableTiles(owner);
    public CounterController getCounterController() => counterController;
    public override List<Keyword> getInitialKeywords() => creature.getInitialKeywords();

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("In creatureCard awake");
        creature = GetComponent<Creature>();
        creatureStatsScript = GetComponent<CreatureStatsGetter>();
    }

    private void OnDisable()
    {
        Debug.Log("CreatureCard disabled");
    }

    public override void initialize()
    {
        creature.initialize();
        foreach (Keyword k in getInitialKeywords())
            addKeyword(k);

        onInitialization(); // keep this on last line
    }

    public override void play(Tile t)
    {
        GameManager gameManager = GameManager.Get();

        gameManager.createCreatureOnTile(creature, t, owner); // this makes the assumption that a card will always be played by it's owner
        setSpritesToSortingLayer(SpriteLayers.Creature);
        creatureStatsScript.setTextSortingLayer(SpriteLayers.CreatureAbove);
        phaseOut();
        owner.hand.resetCardPositions();
        GameEvents.TriggerCreaturePlayedEvents(null, new GameEvents.CreaturePlayedArgs() { creature = creature });
    }

    internal void swapToCreature(Tile onTile)
    {
        // disable card functionality
        enabled = false;

        // enable creature functionality
        creature.enabled = true;

        // resize
        creatureStatsScript.swapToCreature(this, onTile);

        // set card pile to board
        moveToCardPile(Board.instance, null); // null to signal by game mechanics
    }

    internal void swapToCard()
    {
        // enable card functinality
        enabled = true;

        // disable creature functionality
        creature.enabled = true;

        // resize
        creatureStatsScript.swapToCard();

        // no longer a creature so forget the tile it's on
        creature.currentTile = null;
        // and remove it from allCreatures
        GameManager.Get().allCreatures.Remove(creature);

        // counters don't say on cards when they aren't creatures so clear them
        counterController.clearAll();
    }

    public override void resetToBaseStats()
    {
        base.resetToBaseStats();
        creature.resetToBaseStats();
    }
    public override void resetToBaseStatsWithoutSyncing()
    {
        base.resetToBaseStatsWithoutSyncing();
        creature.resetToBaseStatsWithoutSyncing();
    }

    protected override List<Tag> getTags()
    {
        return creature.getTags();
    }

    // returns true if the card can be played right now
    public override bool canBePlayed()
    {
        if (!creature.additionalCanBePlayedChecks())
        {
            Debug.Log("additional can be played checks return false");
            return false;
        }
        // check if the player can pay the costs
        if (!ownerCanPayCosts())
        {
            Debug.Log("owner can play costs return false");
            return false;
        }
        else
            return true;
    }
}
