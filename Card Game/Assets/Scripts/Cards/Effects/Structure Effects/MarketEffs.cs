using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Structure.Controller.GoldPerTurn += 1;
    };

    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        Structure.Controller.GoldPerTurn -= 1;
    };
}
