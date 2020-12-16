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
            creature.BaseAttack += 3;
            creature.BaseHealth += 3;
            creature.Health = creature.BaseHealth;
            creature.AttackStat = creature.BaseAttack;

            card.MoveToCardPile(card.Owner.Deck, card);
            card.Owner.Deck.Shuffle();
        }
    };
}
