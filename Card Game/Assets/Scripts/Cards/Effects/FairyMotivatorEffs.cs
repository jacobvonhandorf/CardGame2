using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMotivatorEffs : CreatureEffects
{
    public override EventHandler onDeploy => onDeployEffect;

    private void onDeployEffect(object sender, EventArgs e)
    {
        foreach (Card c in creature.controller.hand.getAllCardsWithType(Card.CardType.Creature))
        {
            (c as CreatureCard).creature.AttackStat += 1;
            (c as CreatureCard).creature.Health += 1;
        }
    }
}
