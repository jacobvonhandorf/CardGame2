using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneNoviceEffs : CreatureEffects
{
    public override EmptyHandler onInitilization => delegate ()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    };

    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (creature.enabled && creature.Counters.AmountOf(CounterType.Arcane) > 0 && e.spell.owner == creature.Controller)
        {
            creature.Controller.DrawCard();
            creature.Counters.Remove(CounterType.Arcane, 1);
        }
    }

    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        creature.Counters.Add(CounterType.Arcane, 1);
    };
}
