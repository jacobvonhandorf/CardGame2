using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Creature
{
    public override int getCardId()
    {
        return 53;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAttack()
    {
        controller.addGold(1);
    }

    public override void onKillingACreature(Creature c)
    {
        controller.addGold(1);
    }
}
