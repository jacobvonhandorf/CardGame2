using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBriber : Creature
{
    public override int cardId => 72;

    public override void onCreation()
    {
        int gemCount = controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count;
        if (gemCount > 0)
        {
            addAttack(1);
            if (gemCount >= 3)
                addAttack(1);
        }
    }

    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy, Keyword.defender1 };
}
