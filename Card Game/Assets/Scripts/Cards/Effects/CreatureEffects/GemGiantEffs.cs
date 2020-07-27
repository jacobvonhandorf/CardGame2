using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGiantEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        int numGemsInHand = creature.controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count;
        creature.addAttack(numGemsInHand);
        creature.addHealth(numGemsInHand * 2);
    };
}
