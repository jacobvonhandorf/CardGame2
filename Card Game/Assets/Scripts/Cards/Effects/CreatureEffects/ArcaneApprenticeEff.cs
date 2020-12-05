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
            creature.Counters.Add(CounterType.Arcane, 1);
        }
    }

    public override EmptyHandler activatedEffect => delegate ()
    {
        
        if (creature.hasDoneActionThisTurn)
        {
            GameManager.Get().ShowToast("You have already acted with this creature this turn");
            return;
        }

        int selectedValue = -1;
        QueueableCommand xPickCmd = XPickerBox.CreateAsCommand(1, creature.Counters.AmountOf(CounterType.Arcane), "How many counters to remove?", creature.Controller, delegate (int x)
        {
            selectedValue = x;
        });
        QueueableCommand targetSelect = SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(creature.Controller.getOppositePlayer(), false), delegate (Tile t)
        {
            creature.Counters.Remove(CounterType.Arcane, selectedValue);
            t.creature.TakeDamage(selectedValue, creature.SourceCard);
            creature.hasDoneActionThisTurn = true;
        });
        new CompoundQueueableCommand.Builder().addCommand(xPickCmd).addCommand(targetSelect).BuildAndQueue();
    };
}
