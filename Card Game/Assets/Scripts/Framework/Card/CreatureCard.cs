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
        GameManager gameManager = GameManager.Instance;
        gameManager.createCreatureOnTile(Creature, t, Owner); // this makes the assumption that a card will always be played by it's owner
        GameEvents.TriggerCreaturePlayedEvents(null, new GameEvents.CreaturePlayedArgs() { creature = Creature });
    }

    public void SwapToCreature(Tile onTile)
    {
        // disable card functionality
        enabled = false;

        // enable creature functionality
        Creature.enabled = true;

        // set card pile to board
        if (Board.Instance != null)
            MoveToCardPile(Board.Instance, null); // null to signal by game mechanics

        // resize
        cardToPermanentConverter.DoConversion(onTile.transform.position);

        // attach hover and click handlers
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

        // remove hover and click handlers
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
