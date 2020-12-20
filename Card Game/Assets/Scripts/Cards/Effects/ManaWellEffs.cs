using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWellEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        //GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        //GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
    };
    /*
    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        Structure.Counters.Add(CounterType.Arcane, 1);
        if (Structure.Counters.AmountOf(CounterType.Arcane) == 3)
        {
            Card.ShowInEffectsView();
            //controller.addMana(1);
            Structure.Counters.Remove(CounterType.Arcane, 3);
        }
    }
    */
}
