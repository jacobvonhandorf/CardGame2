using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Creature
{
    public override int cardId => 53;

    public override void onInitialization()
    {
        E_OnAttack += Thief_E_OnAttack;
    }

    private void Thief_E_OnAttack(object sender, OnAttackArgs e)
    {
        controller.addGold(1);
    }

    public override void onKillingACreature(Creature c)
    {
        controller.addGold(1);
    }
}
