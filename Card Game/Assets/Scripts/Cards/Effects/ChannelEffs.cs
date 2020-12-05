using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class ChannelEffs : SpellEffects
{
    public int numCards = 3;
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        int numCardsToReduce = numCards;
        foreach (Card c in card.owner.deck.getCardList())
        {
            if (c.IsType(CardType.Spell))
            {
                c.ManaCost -= 1;
                if (c.ManaCost < 0)
                    c.ManaCost = 0;
                numCardsToReduce--;
                if (numCardsToReduce == 0)
                    break;
            }
        }
        card.owner.DrawCard();
    }
}
