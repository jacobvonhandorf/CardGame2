using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazaarEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        controller.increaseGoldPerTurn(2);
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        controller.increaseGoldPerTurn(-2);
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        controller.addGold(1);
        controller.subtractActions(1);
    };
}
