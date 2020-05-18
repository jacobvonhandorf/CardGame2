using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBriber : Creature
{
    public override int getCardId()
    {
        return 72;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        if (controller.graveyard.getAllCardsWithTag(Card.Tag.Gem).Count > 0)
        {
            addHealth(2);
            addAttack(2);
        }
    }
}
