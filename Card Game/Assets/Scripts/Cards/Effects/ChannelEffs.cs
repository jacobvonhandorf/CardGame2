using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class ChannelEffs : SpellEffects
{
    public int numCards = 3;
    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        int numCardsToReduce = numCards;
        foreach (Card c in card.owner.Deck.CardList)
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
