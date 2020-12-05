using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureCard : Card
{
    [HideInInspector] public Structure structure;
    public bool isStructure;
    [SerializeField] private CounterController counterCountroller;
    [SerializeField] private PermanentCardVisual cardVisual;

    public new PermanentCardVisual CardVisuals { get { return (PermanentCardVisual)cardVisuals; } }
    public override List<Tile> LegalTargetTiles => GameManager.Get().getLegalStructurePlacementTiles(owner);
    public override CardType getCardType() => CardType.Structure;

    protected override void Awake()
    {
        base.Awake();
        structure = GetComponent<Structure>();
    }

    public override void Initialize()
    {
        onInitilization?.Invoke();
        onInitilization = null;
    }

    public override void Play(Tile t)
    {
        GameManager.Get().createStructureOnTile(structure, t, owner, this);
        owner.hand.resetCardPositions();
    }

    public void swapToStructure(Tile onTile)
    {
        // change structure so that it is rendered beneath cards in hand
        //setSpritesToSortingLayer(SpriteLayers.Creature);

        // disable card functionality
        enabled = false;

        // enable creature functionality
        structure.enabled = true;

        // initialize the structure if it hasn't already been initialized
        //structure.initialize();

        // resize
        cardVisual.ResizeToPermanent(onTile.transform.position);

        isStructure = true;
    }

    internal void swapToCard()
    {
        // enable card functionality
        enabled = true;

        // disable structure functionality
        structure.enabled = false;

        // resize
        cardVisual.ResizeToCard();

        // set tile back to null because no longer on field
        structure.Tile = null;

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
    public override bool CanBePlayed()
    {
        if (!structure.additionalCanBePlayedChecks())
            return false;
        // check if the player can pay the costs
        if (!OwnerCanPayCosts())
            return false;
        else
            return true;
    }

    public CounterController getCounterController() => counterCountroller;
}
