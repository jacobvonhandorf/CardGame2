﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerEffects : CreatureEffects
{
    public override ActivatedEffect activatedEffect => new ActivatedEffect("Create Structure", delegate ()
    {
        List<string> options = new List<string>
        {
            "Market",
            "Altar",
            "Tower"
        };

        int selectedCardId = -1;
        IQueueableCommand selectCommand = OptionSelectBox.CreateCommand(options, "Select which structure you would like to place", creature.Controller, delegate (int selectedIndex, string selectedOption)
        {
            if (selectedIndex == 0)
                selectedCardId = (int)CardIds.Market;
            else if (selectedIndex == 1)
                selectedCardId = (int)CardIds.Altar;
            else
                selectedCardId = (int)CardIds.Tower;
        });
        Debug.Log(creature.Controller);
        List<Tile> validTargets = GameManager.Instance.getLegalStructurePlacementTiles(creature.Controller);
        validTargets.RemoveAll(t => t.GetDistanceTo(creature.Tile) > 1);
        validTargets.RemoveAll(t => t.Creature != null);
        IQueueableCommand selectTileCmd = SingleTileTargetEffect.CreateCommand(validTargets, delegate (Tile t)
        {
            creature.Counters.Remove(CounterType.Build, 1);
            (GameManager.Instance.CreateCardById(selectedCardId, creature.Controller) as StructureCard).Structure.CreateOnTile(t);
            creature.ActionAvailable = false;
            creature.UpdateHasActedIndicators();
        });
        new CompoundQueueableCommand.Builder().AddCommand(selectCommand).AddCommand(selectTileCmd).BuildAndQueue();
    });
}
