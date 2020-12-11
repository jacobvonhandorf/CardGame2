using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGiantEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        int numGemsInHand = creature.Controller.Hand.GetAllCardsWithTag(Card.Tag.Gem).Count;
        creature.AttackStat += numGemsInHand;
        creature.Health += numGemsInHand * 2;
    };
}
