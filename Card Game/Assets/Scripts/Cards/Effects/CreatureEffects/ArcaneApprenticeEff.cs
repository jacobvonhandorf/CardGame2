using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneApprenticeEff : CreatureEffects
{
    public override EmptyHandler onInitilization => delegate ()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    };

    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (creature.enabled)
        {
            creature.addCounters(Counters.arcane, 1);
        }
    }

    public override EmptyHandler activatedEffect => delegate ()
    {
        
        if (creature.hasDoneActionThisTurn)
        {
            GameManager.Get().showToast("You have already acted with this creature this turn");
            return;
        }

        int selectedValue = -1;
        QueueableCommand xPickCmd = XPickerBox.CreateAsCommand(1, creature.hasCounter(Counters.arcane), "How many counters to remove?", creature.controller, delegate (int x)
        {
            selectedValue = x;
        });
        QueueableCommand targetSelect = SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(creature.controller.getOppositePlayer(), false), delegate (Tile t)
        {
            creature.removeCounters(Counters.arcane, selectedValue);
            t.creature.takeDamage(selectedValue, creature.sourceCard);
            creature.hasDoneActionThisTurn = true;
        });
        new CompoundQueueableCommand.Builder().addCommand(xPickCmd).addCommand(targetSelect).BuildAndQueue();
    };
}
