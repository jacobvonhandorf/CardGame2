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
                if (cardToAdd.creature.AttackStat < c.creature.AttackStat)
                cardToAdd = c;
        }
        if (cardToAdd != null)
        {
            cardToAdd.creature.AttackStat += 1;
            cardToAdd.creature.Health += 1;
            cardToAdd.moveToCardPile(card.owner.hand, card);
        }
    }
}
