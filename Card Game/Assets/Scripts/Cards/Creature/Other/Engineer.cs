using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Engineer : Creature, Effect
{
    public const int CARD_ID = 61;
    public override int cardId => CARD_ID;

    public override Effect getEffect()
    {
        return this;
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.quick,
            Keyword.untargetable
        };
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        List<string> options = new List<string>
        {
            "Market",
            "Altar",
            "Tower"
        };

        int selectedCardId = -1;
        QueueableCommand selectCommand = OptionSelectBox.CreateCommand(options, "Select which structure you would like to place", controller, delegate (int selectedIndex, string selectedOption)
        {
            if (selectedIndex == 0)
                selectedCardId = Market.CARD_ID;
            else if (selectedIndex == 1)
                selectedCardId = Altar.CARD_ID;
            else
                selectedCardId = CommandTower.CARD_ID;
        });
        List<Tile> validTargets = GameManager.Get().getLegalStructurePlacementTiles(sourcePlayer);
        validTargets.RemoveAll(t => t.getDistanceTo(sourceTile) > 1);
        validTargets.RemoveAll(t => t.creature != null);
        QueueableCommand selectTileCmd = SingleTileTargetEffect.CreateCommand(validTargets, delegate (Tile t)
        {
            removeCounters(Counters.build, 1);
            StructureCard structureToPlace = GameManager.Get().createCardById(selectedCardId, controller) as StructureCard;
            GameManager.Get().createStructureOnTile(structureToPlace.structure, t, controller, structureToPlace);
            hasDoneActionThisTurn = true;
            updateHasActedIndicators();
        });
        new CompoundQueueableCommand.Builder().addCommand(selectCommand).addCommand(selectTileCmd).BuildAndQueue();
    }
}
