using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTheFires : SpellCard, Effect, CanReceivePickedCards
{
    //public static bool alreadyActivatedThisTurn = false;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.hand.getCardList(), this, 0, sourcePlayer.hand.getCardList().Count, false , "Select cards to discard");
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    public void receiveCardList(List<Card> cardList)
    {
        int numDiscardedCards = cardList.Count;
        foreach (Card c in cardList)
        {
            c.moveToCardPile(owner.graveyard, this);
        }
        owner.addMana(numDiscardedCards);
        owner.drawCard();

        // handle OPT stuff
        //alreadyActivatedThisTurn = true;
        //EffectActuator resetEffect = new EffectActuator();
        //resetEffect.effect = new ResetOPTRestriction();
        //GameManager.Get().beginningOfTurnEffectsList.Add(resetEffect);
    }

    private class ResetOPTRestriction : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            //alreadyActivatedThisTurn = false;
        }
    }

    public override bool additionalCanBePlayedChecks()
    {
        //if (alreadyActivatedThisTurn)
        //    return false;
        foreach (Creature c in GameManager.Get().getAllCreaturesControlledBy(owner))
        {
            if (c.hasTag(Tag.Arcane))
                return true;
        }
        return false;
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public override int getCardId()
    {
        return 6;
    }
}
