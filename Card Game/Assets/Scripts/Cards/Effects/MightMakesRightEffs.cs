using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class MightMakesRightEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        Deck deck = card.owner.deck;
        CreatureCard cardToAdd = null;
        foreach (CreatureCard c in deck.getAllCardsWithType(CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else
                if (cardToAdd.creature.getAttack() < c.creature.getAttack())
                cardToAdd = c;
        }
        if (cardToAdd != null)
        {
            cardToAdd.creature.addAttack(1);
            cardToAdd.creature.addHealth(1);
            cardToAdd.moveToCardPile(card.owner.hand, card);
        }
    }
}
