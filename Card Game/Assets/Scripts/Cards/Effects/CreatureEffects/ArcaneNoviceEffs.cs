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
        if (creature.enabled && creature.hasCounter(Counters.arcane) > 0 && e.spell.owner == creature.controller)
        {
            creature.controller.drawCard();
            creature.removeCounters(Counters.arcane, 1);
        }
    }

    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        creature.addCounters(Counters.arcane, 1);
    };
}
