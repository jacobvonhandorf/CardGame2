﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Structure.Controller.ManaPerTurn += 1;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        Structure.Controller.ManaPerTurn -= 1;
    };
}
