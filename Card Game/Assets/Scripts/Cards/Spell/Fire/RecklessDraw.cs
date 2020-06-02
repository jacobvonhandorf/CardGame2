using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDraw : SpellCard
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
        return new MyEffect();
    }

    private class MyEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.hand.getCardList(), new RecklessEff(sourcePlayer), 1, 1, false, "Select a card to discard");
        }
    }

    private class RecklessEff : CanReceivePickedCards
    {
        private Player owner;

        public RecklessEff(Player owner)
        {
            this.owner = owner;
        }

        public void receiveCardList(List<Card> cardList)
        {
            foreach(Card c in cardList)
            {
                c.moveToCardPile(owner.graveyard, true);
            }
            owner.drawCards(2);
        }
    }
}
