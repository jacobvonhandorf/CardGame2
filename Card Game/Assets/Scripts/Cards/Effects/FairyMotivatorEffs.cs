using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMotivatorEffs : CreatureEffects
{
    public override EventHandler onDeploy => onDeployEffect;

    private void onDeployEffect(object sender, EventArgs e)
    {
        foreach (Card c in creature.Controller.Hand.GetAllCardsWithType(CardType.Creature))
        {
            (c as CreatureCard).Creature.AttackStat += 1;
            (c as CreatureCard).Creature.Health += 1;
        }
    }
}
