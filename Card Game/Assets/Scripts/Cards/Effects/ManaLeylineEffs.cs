using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaLeylineEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        controller.increaseManaPerTurn(2);
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        controller.increaseManaPerTurn(-2);
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        controller.addActions(-1);
        controller.addMana(1);
    };
}
