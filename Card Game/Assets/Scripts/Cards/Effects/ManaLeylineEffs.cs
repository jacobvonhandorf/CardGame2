using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaLeylineEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Controller.ManaPerTurn += 2;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        Controller.ManaPerTurn -= 2;
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        Controller.Actions -= 1;
        Controller.Mana += 1;
    };
}
