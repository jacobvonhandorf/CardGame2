using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBriberEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        int gemCount = creature.Controller.Hand.GetAllCardsWithTag(Tag.Gem).Count;
        if (gemCount > 0)
        {
            creature.AttackStat += 1;
            if (gemCount >= 3)
                creature.AttackStat += 1;
        }
    };
}
