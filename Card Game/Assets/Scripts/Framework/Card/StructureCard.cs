using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureCard : Card
{
    [HideInInspector] public Structure structure;
    public bool isStructure;
    public CounterController CounterController => counterCountroller;
    [SerializeField] private CounterController counterCountroller;

    public new PermanentCardVisual CardVisuals { get { return (PermanentCardVisual)base.CardVisuals; } }
    public override List<Tile> LegalTargetTiles => GameManager.Instance.getLegalStructurePlacementTiles(Owner);
    public override CardType CardType => CardType.Structure;

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
        GameManager.Instance.createStructureOnTile(structure, t, Owner, this);
    }

    public void swapToStructure(Tile onTile)
    {
        // disable card functionality
        enabled = false;

        // enable creature functionality
        structure.enabled = true;

        // resize
        Debug.Log("Tile position " + onTile.transform.position);
        CardVisuals.ResizeToPermanent(onTile.transform.position);

        isStructure = true;
    }

    internal void swapToCard()
    {
        // enable card functionality
        enabled = true;

        // disable structure functionality
        structure.enabled = false;

        // resize
        CardVisuals.ResizeToCard();

        // set tile back to null because no longer on field
        structure.Tile = null;

        isStructure = false;
    }

    public override void ResetToBaseStats()
    {
        base.ResetToBaseStats();
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

}
