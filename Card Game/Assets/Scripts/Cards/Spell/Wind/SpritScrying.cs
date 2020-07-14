using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritScrying : SpellCard, Effect
{
    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        CardPicker.CreateAndQueue(owner.hand.getAllCardsWithTag(Tag.Fairy), 1, 1, "Select a card to add to your hand", owner, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(owner.hand, this);
        });
    }

    public override int getCardId()
    {
        return 2;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }

    /*
    private class SpiritScryingEffect : Effect
    {
        Card sourceCard;

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.deck.getAllCardsWithTag(Tag.Fairy), new MyCardPickReceiver(sourcePlayer, sourceCard), 1, 1, false, "Select a card to add to hand");
        }

        private class MyCardPickReceiver : CanReceivePickedCards
        {
            Player player;
            Card sourceCard;

            public MyCardPickReceiver(Player player, Card sourceCard)
            {
                this.player = player;
                this.sourceCard = sourceCard;
            }

            public void receiveCardList(List<Card> cardList)
            {
                if (cardList.Count > 0)
                    sourceCard.moveToCardPile(player.hand, sourceCard);
            }
        }
    }
    */
}
