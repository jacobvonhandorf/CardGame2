using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlacTheImmortal : Creature
{
    public override int cardId => 28;

    public override void onSentToGrave()
    {
        baseAttack += 3;
        baseHealth += 3;
        setHealth(baseHealth);
        setAttack(baseAttack);

        sourceCard.moveToCardPile(sourceCard.owner.deck, sourceCard);
        sourceCard.owner.deck.shuffle();
    }
}
