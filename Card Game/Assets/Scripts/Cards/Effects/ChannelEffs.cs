﻿using System.Collections;
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
            if (c.isType(CardType.Spell))
            {
                c.setManaCost(c.getManaCost() - 1);
                if (c.getManaCost() < 0)
                    c.setManaCost(0);
                numCardsToReduce--;
                if (numCardsToReduce == 0)
                    break;
            }
        }
        card.owner.drawCard();
    }
}