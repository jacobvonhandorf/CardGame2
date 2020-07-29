using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGiantEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        int numGemsInHand = creature.controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count;
        creature.AttackStat += numGemsInHand;
        creature.Health += numGemsInHand * 2;
    };
}
