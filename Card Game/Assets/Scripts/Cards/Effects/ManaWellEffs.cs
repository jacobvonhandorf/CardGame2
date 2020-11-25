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
        structure.Counters.add(CounterType.Arcane, 1);
        if (structure.Counters.amountOf(CounterType.Arcane) == 3)
        {
            card.showInEffectsView();
            controller.addMana(1);
            structure.Counters.remove(CounterType.Arcane, 3);
        }
    }
}
