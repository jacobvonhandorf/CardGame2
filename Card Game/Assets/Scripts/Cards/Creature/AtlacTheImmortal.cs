using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlacTheImmortal : Creature
{
    public override int getCardId()
    {
        return 28;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onSentToGrave()
    {
        baseAttack += 3;
        baseHealth += 3;
        resetToBaseStats();

        Debug.Log("Card pile when sent to grave " + sourceCard.getCardPile());
        sourceCard.moveToCardPile(sourceCard.owner.deck);
        sourceCard.owner.deck.shuffle();
    }
}
