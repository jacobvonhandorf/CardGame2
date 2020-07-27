using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBriberEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        int gemCount = creature.controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count;
        if (gemCount > 0)
        {
            creature.addAttack(1);
            if (gemCount >= 3)
                creature.addAttack(1);
        }
    };
}
