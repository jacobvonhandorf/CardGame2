using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class MightMakesRightEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        Deck deck = card.Owner.Deck;
        CreatureCard cardToAdd = null;
        foreach (CreatureCard c in deck.GetAllCardsWithType(CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else
                if (cardToAdd.Creature.AttackStat < c.Creature.AttackStat)
                cardToAdd = c;
        }
        if (cardToAdd != null)
        {
            cardToAdd.Creature.AttackStat += 1;
            cardToAdd.Creature.Health += 1;
            cardToAdd.MoveToCardPile(card.Owner.Hand, card);
        }
    }
}
