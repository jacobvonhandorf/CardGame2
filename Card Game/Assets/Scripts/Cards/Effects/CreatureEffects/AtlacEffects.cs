using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlacEffects : CreatureEffects
{
    public override EventHandler<Card.AddedToCardPileArgs> onMoveToCardPile => delegate (object sender, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Graveyard)
        {
            creature.baseAttack += 3;
            creature.baseHealth += 3;
            creature.setHealth(creature.baseHealth);
            creature.setAttack(creature.baseAttack);

            card.moveToCardPile(card.owner.deck, card);
            card.owner.deck.shuffle();
        }
    };
}
