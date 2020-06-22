using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGrasp : SpellCard, CanReceivePickedCards, Effect
{
    //public static bool alreadyActivatedThisTurn = false;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        GameManager.Get().queueCardPickerEffect(sourcePlayer, owner.deck.getAllCardsWithTag(Tag.Arcane), this, 1, 1, false, "Select a card to add to your hand");
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    public void receiveCardList(List<Card> cardList)
    {
        Card c = cardList[0];
        //owner.hand.addCardByEffect(c);
        c.moveToCardPile(owner.hand, true);
        foreach (Creature creature in GameManager.Get().getAllCreaturesControlledBy(owner))
        {
            if (creature.hasTag(Tag.Arcane))
            {
                owner.addMana(1);
                break;
            }
        }

        // handle OPT stuff
      //  alreadyActivatedThisTurn = true;
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

    protected override Effect getEffect()
    {
        return this;
    }

    /*
    public override bool additionalCanBePlayedChecks()
    {
        return !alreadyActivatedThisTurn;
    }*/

    public override int getCardId()
    {
        return 3;
    }
}
