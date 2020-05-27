using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureCard : Card
{
    public Creature creature;
    [SerializeField] private CreatureStatsGetter creatureStatsScript;
    [SerializeField] private CounterController counterCountroller;
    public bool isCreature = false; // true when is being treated as a creature. False when treated as card

    protected override void Start()
    {
        base.Start();
        //creature.initialize();
    }

    public override void initialize()
    {
        creature.initialize();
    }

    public override void play(Tile t)
    {
        GameManager gameManager = GameManager.Get();

        gameManager.createCreatureOnTile(creature, t, owner, this); // this makes the assumption that a card will always be played by it's owner
        setSpritesToSortingLayer(SpriteLayers.Creature);
        creatureStatsScript.setTextSortingLayer(SpriteLayers.CreatureAbove);
        phaseOut();
        owner.hand.resetCardPositions();
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllDeployableTiles(owner);
    }

    public override CardType getCardType()
    {
        return CardType.Creature;
    }

    internal void swapToCreature()
    {
        //creature.initialize();

        // disable card functionality
        gameObject.SetActive(false);

        // enable creature functionality
        creature.gameObject.SetActive(true);

        // resize
        creatureStatsScript.swapToCreature(this);

        // set card pile to null so it is no longer in hand
        currentCardPile = null;

        isCreature = true;
    }

    internal void swapToCard()
    {
        // enable card functinality
        gameObject.SetActive(true);

        // disable creature functionality
        creature.gameObject.SetActive(false);

        // resize
        creatureStatsScript.swapToCard(this);

        isCreature = false;
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

    public override void onCardDrawn()
    {
        creature.onCardDrawn();
    }

    public override void onSentToGrave()
    {
        creature.onSentToGrave();
    }

    protected override List<Tag> getTags()
    {
        return creature.getTags();
    }

    public override void onCardAddedByEffect()
    {
        creature.onCardAddedToHandByEffect();
    }
    public override void onAnyCreaturePlayed(Creature c)
    {
        creature.onAnyCreaturePlayed(c);
    }

    public override void onAnySpellCast(SpellCard s)
    {
        Debug.Log("On any spell cast in creature");
        creature.onAnySpellCast(s);
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

    public CounterController getCounterController()
    {
        return counterCountroller;
    }

    public override int getCardId()
    {
        return creature.getCardId();
    }
}
