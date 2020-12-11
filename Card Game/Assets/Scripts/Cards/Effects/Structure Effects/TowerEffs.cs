using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Structure.Controller.ActionsPerTurn += 1;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        Structure.Controller.ActionsPerTurn -= 1;
    };
}
