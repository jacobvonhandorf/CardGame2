using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureCard : Card
{
    public Structure Structure { get; private set; }
    public CounterController CounterController => counterCountroller;
    [SerializeField] private CounterController counterCountroller;

    public new PermanentCardVisual CardVisuals { get { return (PermanentCardVisual)base.CardVisuals; } }
    public override List<Tile> LegalTargetTiles => GameManager.Instance.getLegalStructurePlacementTiles(Owner);
    public override CardType CardType => CardType.Structure;

    protected override void Awake()
    {
        base.Awake();
        Structure = GetComponent<Structure>();
    }

    public override void Initialize()
    {
        onInitilization?.Invoke();
        onInitilization = null;
    }

    public override void Play(Tile t)
    {
        Structure.CreateOnTile(t);
    }

    public void SwapToStructure(Tile onTile)
    {
        enabled = false;
        Structure.enabled = true;

        // resize
        CardVisuals.ResizeToPermanent(onTile.transform.position);
        //Structure.SyncCreateOnTile(onTile);
    }

    public void SwapToCard()
    {
        // enable card functionality
        enabled = true;

        // disable structure functionality
        Structure.enabled = false;

        // resize
        CardVisuals.ResizeToCard();

        // set tile back to null because no longer on field
        Structure.Tile = null;
    }

    public override void ResetToBaseStats()
    {
        base.ResetToBaseStats();
        Structure.resetToBaseStats();
    }

    public override void resetToBaseStatsWithoutSyncing()
    {
        base.resetToBaseStatsWithoutSyncing();
        Structure.resetToBaseStatsWithoutSyncing();
    }

    // returns true if the card can be played right now
    public override bool CanBePlayed()
    {
        if (!Structure.additionalCanBePlayedChecks())
            return false;
        if (!OwnerCanPayCosts())
            return false;
        else
            return true;
    }
}
