using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerEffects : CreatureEffects
{
    public override EmptyHandler activatedEffect => delegate ()
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
        List<Tile> validTargets = GameManager.Get().getLegalStructurePlacementTiles(creature.Controller);
        validTargets.RemoveAll(t => t.GetDistanceTo(creature.Tile) > 1);
        validTargets.RemoveAll(t => t.creature != null);
        IQueueableCommand selectTileCmd = SingleTileTargetEffect.CreateCommand(validTargets, delegate (Tile t)
        {
            creature.Counters.Remove(CounterType.Build, 1);
            StructureCard structureToPlace = GameManager.Get().createCardById(selectedCardId, creature.Controller) as StructureCard;
            GameManager.Get().createStructureOnTile(structureToPlace.structure, t, creature.Controller, structureToPlace);
            creature.hasDoneActionThisTurn = true;
            creature.UpdateHasActedIndicators();
        });
        new CompoundQueueableCommand.Builder().AddCommand(selectCommand).AddCommand(selectTileCmd).BuildAndQueue();
    };
}
