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
        QueueableCommand selectCommand = OptionSelectBox.CreateCommand(options, "Select which structure you would like to place", creature.controller, delegate (int selectedIndex, string selectedOption)
        {
            if (selectedIndex == 0)
                selectedCardId = Market.CARD_ID;
            else if (selectedIndex == 1)
                selectedCardId = Altar.CARD_ID;
            else
                selectedCardId = CommandTower.CARD_ID;
        });
        Debug.Log(creature.controller);
        List<Tile> validTargets = GameManager.Get().getLegalStructurePlacementTiles(creature.controller);
        validTargets.RemoveAll(t => t.getDistanceTo(creature.currentTile) > 1);
        validTargets.RemoveAll(t => t.creature != null);
        QueueableCommand selectTileCmd = SingleTileTargetEffect.CreateCommand(validTargets, delegate (Tile t)
        {
            creature.removeCounters(Counters.build, 1);
            StructureCard structureToPlace = GameManager.Get().createCardById(selectedCardId, creature.controller) as StructureCard;
            GameManager.Get().createStructureOnTile(structureToPlace.structure, t, creature.controller, structureToPlace);
            creature.hasDoneActionThisTurn = true;
            creature.updateHasActedIndicators();
        });
        new CompoundQueueableCommand.Builder().addCommand(selectCommand).addCommand(selectTileCmd).BuildAndQueue();
    };
}
