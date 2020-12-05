using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyFortressEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        controller.increaseActionsPerTurn(1);
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        controller.increaseActionsPerTurn(-1);
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        if (controller.GetActions() < 1)
        {
            Toaster.instance.doToast("You do not have enough actions to activate " + card.CardName);
            return;
        }

        Creature ownedCreature = null;
        QueueableCommand ownedSelect = SingleTileTargetEffect.CreateCommand(Board.instance.getAllTilesWithCreatures(controller, true), delegate (Tile t)
        {
            ownedCreature = t.creature;
        });
        QueueableCommand opponentSelect = SingleTileTargetEffect.CreateCommand(Board.instance.getAllTilesWithCreatures(controller.OppositePlayer, false), delegate (Tile t)
        {
            Creature opponentCreature = t.creature;
            controller.subtractActions(1);
            ownedCreature.Bounce(card);
            opponentCreature.Bounce(card);
        });
        new CompoundQueueableCommand.Builder().addCommand(ownedSelect).addCommand(opponentSelect).BuildAndQueue();
    };
}
