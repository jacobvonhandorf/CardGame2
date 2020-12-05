using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureCard : Card
{
    public Creature Creature { get; private set; }
    [SerializeField] private CounterController counterController;

    public override CardType getCardType() => CardType.Creature;
    public override List<Tile> LegalTargetTiles => GameManager.Get().getAllDeployableTiles(owner);
    public CounterController CounterController { get { return counterController; } }

    public new PermanentCardVisual CardVisuals { get { return (PermanentCardVisual)cardVisuals; } }

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
        GameManager gameManager = GameManager.Get();
        gameManager.createCreatureOnTile(Creature, t, owner); // this makes the assumption that a card will always be played by it's owner
        //setSpritesToSortingLayer(SpriteLayers.Creature);
        owner.hand.resetCardPositions();
        GameEvents.TriggerCreaturePlayedEvents(null, new GameEvents.CreaturePlayedArgs() { creature = Creature });
    }

    internal void SwapToCreature(Tile onTile)
    {
        // disable card functionality
        enabled = false;

        // enable creature functionality
        Creature.enabled = true;

        // resize
        //creatureStatsScript.swapToCreature(this, onTile);
        cardToPermanentConverter.DoConversion(onTile.transform.position);

        // set card pile to board
        MoveToCardPile(Board.instance, null); // null to signal by game mechanics
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
        // and remove it from allCreatures
        GameManager.Get().allCreatures.Remove(Creature);

        // counters don't say on cards when they aren't creatures so clear them
        counterController.Clear();
    }

    public override void resetToBaseStats()
    {
        base.resetToBaseStats();
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
            Debug.Log("owner can play costs return false");
            return false;
        }
        else
            return true;
    }
}
