using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritScrying : SpellCard
{
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
        return new SpiritScryingEffect();
    }

    private class SpiritScryingEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.deck.getAllCardsWithTag(Tag.Fairy), new MyCardPickReceiver(sourcePlayer), 1, 1, "Select a card to add to hand");
        }

        private class MyCardPickReceiver : CanReceivePickedCards
        {
            Player player;

            public MyCardPickReceiver(Player player)
            {
                this.player = player;
            }

            public void receiveCardList(List<Card> cardList)
            {
                if (cardList.Count > 0)
                    player.addCardToHandByEffect(cardList[0]);
            }
        }
    }
}
