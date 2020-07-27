using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PityTheWeakEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        Card cardToAdd = null;
        foreach (Card c in owner.deck.getAllCardsWithType(Card.CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else if ((c as CreatureCard).creature.getAttack() < (cardToAdd as CreatureCard).creature.getAttack())
                cardToAdd = c;
        }
        if (cardToAdd == null)
        {
            Toaster.instance.doToast("No targets for " + card.cardName);
            return;
        }
        cardToAdd.moveToCardPile(owner.hand, card);
    }
}
