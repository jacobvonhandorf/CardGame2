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
        int gemCount = controller.graveyard.getAllCardsWithTag(Card.Tag.Gem).Count;
        if (gemCount > 0)
        {
            addAttack(1);
            if (gemCount >= 3)
                addAttack(1);
        }
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }
}
