using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PityTheWeak : SpellCard
{
    public override int cardId => 7;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    protected override void doEffect(Tile t)
    {
        CreatureCard cardToAdd = null;
        foreach (CreatureCard c in owner.deck.getAllCardsWithType(CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else
                if (cardToAdd.creature.getAttack() < c.creature.getAttack())
                cardToAdd = c;
        }
        if (cardToAdd != null)
        {
            cardToAdd.setGoldCost(cardToAdd.getGoldCost() - 1);
            cardToAdd.moveToCardPile(owner.hand, this);
        }
        else
        {
            Debug.Log("Card to add is null");
        }
    }
}
