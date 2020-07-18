using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureCard : Card
{
    public Structure structure;
    public bool isStructure;
    [SerializeField] private CounterController counterCountroller;
    public override List<Tile> legalTargetTiles => GameManager.Get().getLegalStructurePlacementTiles(owner);
    public override CardType getCardType() => CardType.Structure;
    public override int cardId => structure.cardId;

    protected override void Start()
    {
        base.Start();
    }

    public override void initialize()
    {
        structure.initialize();
        onInitialization();
    }


    protected override List<Tag> getTags()
    {
        return structure.getTags();
    }

    public override void play(Tile t)
    {
        GameManager.Get().createStructureOnTile(structure, t, owner, this);
        owner.hand.resetCardPositions();
    }

    public void swapToStructure(Tile onTile)
    {
        // change structure so that it is rendered beneath cards in hand
        setSpritesToSortingLayer(SpriteLayers.Creature);

        // disable card functionality
        gameObject.SetActive(false);

        // enable creature functionality
        structure.gameObject.SetActive(true);

        // initialize the structure if it hasn't already been initialized
        structure.initialize();

        // resize
        if (!isStructure)
            (cardStatsScript as StructureStatsGetter).swapToStructure(onTile);
        //cardStatsScript.switchBetweenCreatureOrCard(this);

        isStructure = true;
    }

    internal void swapToCard()
    {
        // enable card functionality
        gameObject.SetActive(true);

        // disable structure functionality
        structure.gameObject.SetActive(false);

        // resize
        if (isStructure)
            (cardStatsScript as StructureStatsGetter).swapToCard();

        // set tile back to null because no longer on field
        structure.tile = null;

        isStructure = false;
    }

    public override void resetToBaseStats()
    {
        base.resetToBaseStats();
        structure.resetToBaseStats();
    }

    public override void resetToBaseStatsWithoutSyncing()
    {
        base.resetToBaseStatsWithoutSyncing();
        structure.resetToBaseStatsWithoutSyncing();
    }

    // returns true if the card can be played right now
    public override bool canBePlayed()
    {
        if (!structure.additionalCanBePlayedChecks())
            return false;
        // check if the player can pay the costs
        if (!ownerCanPayCosts())
            return false;
        else
            return true;
    }

    public CounterController getCounterController() => counterCountroller;

    public override List<Keyword> getInitialKeywords()
    {
        return structure.getInitialKeywords();
    }
}
