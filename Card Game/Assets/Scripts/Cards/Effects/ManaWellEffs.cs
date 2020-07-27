using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWellEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
    };

    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        structure.addCounters(Counters.well, 1);
        if (structure.hasCounter(Counters.well) == 3)
        {
            card.showInEffectsView();
            controller.addMana(1);
            structure.removeCounters(Counters.well, 3);
        }
    }
}
