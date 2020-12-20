using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneApprenticeEff : CreatureEffects
{
    public override EmptyHandler onInitilization => delegate ()
    {
        //GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    };
    /*
    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (creature.enabled)
        {
            creature.Counters.Add(CounterType.Arcane, 1);
        }
    }
    */

    public override EmptyHandler activatedEffect => delegate ()
    {
        
        if (!creature.ActionAvailable)
        {
            Toaster.Instance.DoToast("You have already acted with this creature this turn");
            return;
        }

        int selectedValue = -1;
        IQueueableCommand xPickCmd = XPickerBox.CreateAsCommand(1, creature.Counters.AmountOf(CounterType.Arcane), "How many counters to remove?", creature.Controller, delegate (int x)
        {
            selectedValue = x;
        });
        IQueueableCommand targetSelect = SingleTileTargetEffect.CreateCommand(Board.Instance.GetAllTilesWithCreatures(creature.Controller.OppositePlayer, false), delegate (Tile t)
        {
            creature.Counters.Remove(CounterType.Arcane, selectedValue);
            t.Creature.TakeDamage(selectedValue, creature.SourceCard);
            creature.ActionAvailable = false;
        });
        new CompoundQueueableCommand.Builder().AddCommand(xPickCmd).AddCommand(targetSelect).BuildAndQueue();
    };
}
