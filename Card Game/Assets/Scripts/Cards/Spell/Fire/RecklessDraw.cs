using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDraw : SpellCard, Effect
{
    public override int getCardId()
    {
        return 12;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.hand.getCardList(), new RecklessEff(sourcePlayer, this), 1, 1, false, "Select a card to discard");
    }

    private class RecklessEff : CanReceivePickedCards
    {
        private Player owner;
        private Card sourceCard;

        public RecklessEff(Player owner, Card sourceCard)
        {
            this.owner = owner;
            this.sourceCard = sourceCard;
        }

        public void receiveCardList(List<Card> cardList)
        {
            foreach(Card c in cardList)
            {
                c.moveToCardPile(owner.graveyard, sourceCard);
            }
            owner.drawCards(2);
        }
    }
}
