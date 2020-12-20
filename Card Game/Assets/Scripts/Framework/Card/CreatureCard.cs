using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureCard : Card, IScriptCreatureCard
{
    public Creature Creature { get; private set; }
    [SerializeField] private CounterController counterController;

    public override CardType CardType => CardType.Creature;
    public override List<Tile> LegalTargetTiles => GameManager.Instance.getAllDeployableTiles(Owner);
    public CounterController CounterController { get { return counterController; } }
    public new PermanentCardVisual CardVisuals { get { return (PermanentCardVisual)base.CardVisuals; } }

    IScriptCreature IScriptCreatureCard.Creature => Creature;

    private CardToPermanentConverter cardToPermanentConverter;

    protected override void Awake()
    {
        base.Awake();
        Creature = GetComponent<Creature>();
        cardToPermanentConverter = GetComponent<CardToPermanentConverter>();
    }

    public override void Initialize()
    {
        Creature.enabled = false;
        onInitilization?.Invoke(); // keep this on last line
    }

    public override void Play(Tile t)
    {
        Creature.CreateOnTile(t);
    }

    public void SwapToCreature(Tile onTile)
    {
        enabled = false;
        Creature.enabled = true;

        // set card pile to board
        if (Board.Instance != null)
            MoveToCardPile(Board.Instance, null); // null to signal by game mechanics

        // resize
        cardToPermanentConverter.DoConversion(onTile.transform.position);
        Debug.Log("aaaaa");
        //Creature.SynCreatureOnTile(onTile);
    }

    public void SwapToCard()
    {
        // enable card functinality
        enabled = true;

        // disable creature functionality
        Creature.enabled = false;

        // resize
        CardVisuals.ResizeToCard();

        // no longer a creature so forget the tile it's on
        Creature.Tile = null;

        // counters don't say on cards when they aren't creatures so clear them
        counterController.Clear();
    }

    public override void ResetToBaseStats()
    {
        base.ResetToBaseStats();
        Creature.ResetToBaseStats();
    }
    public override void resetToBaseStatsWithoutSyncing()
    {
        base.resetToBaseStatsWithoutSyncing();
        Creature.ResetToBaseStatsWithoutSyncing();
    }

    // returns true if the card can be played right now
    public override bool CanBePlayed()
    {
        if (!Creature.additionalCanBePlayedChecks())
        {
            Debug.Log("additional can be played checks return false");
            return false;
        }
        // check if the player can pay the costs
        if (!OwnerCanPayCosts())
        {
            Debug.Log("owner can't play costs return false");
            return false;
        }
        else
            return true;
    }
}
