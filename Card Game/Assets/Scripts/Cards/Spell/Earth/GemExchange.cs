using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemExchange : SpellCard
{
    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return new GemExchangeEffect(this);
    }

    public override bool canBePlayed()
    {
        return owner.hand.getAllCardsWithTag(Tag.Gem).Count > 0 && base.canBePlayed();
    }

    public override int getCardId()
    {
        return 27;
    }

    private class GemExchangeEffect : Effect
    {
        private SpellCard sourceCard;

        public GemExchangeEffect(SpellCard sourceCard)
        {
            this.sourceCard = sourceCard;
        }

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            int maxCardsToPick =  Mathf.Min(sourcePlayer.hand.getAllCardsWithTag(Tag.Gem).Count, 2);
            GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.hand.getAllCardsWithTag(Tag.Gem), new GemECardPickReceiver(sourcePlayer, sourceCard), 0, maxCardsToPick, false, "Select Gem cards to discard");
        }

        private class GemECardPickReceiver : CanReceivePickedCards
        {
            Player owner;
            private SpellCard sourceCard;

            public GemECardPickReceiver(Player owner, SpellCard sourceCard)
            {
                this.owner = owner;
                this.sourceCard = sourceCard;
            }

            public void receiveCardList(List<Card> cardList)
            {
                Debug.Log("Cards to discard count = " + cardList.Count);
                foreach (Card c in cardList)
                {
                    c.moveToCardPile(owner.graveyard, sourceCard);
                }

                owner.drawCards(cardList.Count);
            }
        }
    }

}
