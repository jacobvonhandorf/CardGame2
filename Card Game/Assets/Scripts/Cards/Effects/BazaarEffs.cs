using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazaarEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Controller.GoldPerTurn += 2;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        Controller.GoldPerTurn -= 2;
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        Controller.Gold += 1;
        Controller.Actions -= 1;
    };
}
