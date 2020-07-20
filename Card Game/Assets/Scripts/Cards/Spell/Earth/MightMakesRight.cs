using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MightMakesRight : SpellCard
{
    public override int cardId => 17;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    protected override void doEffect(Tile t)
    {
        Deck deck = owner.deck;
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
            cardToAdd.moveToCardPile(owner.hand, this);
        }
    }
}
