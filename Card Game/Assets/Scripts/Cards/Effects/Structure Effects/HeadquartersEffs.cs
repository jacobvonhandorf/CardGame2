using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquartersEffs : StructureEffects
{
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        structure.controller.makeLose();
    };
}
