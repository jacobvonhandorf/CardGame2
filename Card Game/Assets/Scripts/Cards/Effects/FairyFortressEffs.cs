using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyFortressEffs : StructureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Controller.ActionsPerTurn += 1;
    };
    public override EventHandler onLeavesField => delegate (object s, EventArgs e)
    {
        Controller.ActionsPerTurn -= 1;
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        if (Controller.Actions < 1)
        {
            Toaster.Instance.DoToast("You do not have enough actions to activate " + Card.CardName);
            return;
        }

        Creature ownedCreature = null;
        IQueueableCommand ownedSelect = SingleTileTargetEffect.CreateCommand(Board.Instance.GetAllTilesWithCreatures(Controller, true), delegate (Tile t)
        {
            ownedCreature = t.creature;
        });
        IQueueableCommand opponentSelect = SingleTileTargetEffect.CreateCommand(Board.Instance.GetAllTilesWithCreatures(Controller.OppositePlayer, false), delegate (Tile t)
        {
            Creature opponentCreature = t.creature;
            Controller.Actions -= 1;
            ownedCreature.Bounce(Card);
            opponentCreature.Bounce(Card);
        });
        new CompoundQueueableCommand.Builder().AddCommand(ownedSelect).AddCommand(opponentSelect).BuildAndQueue();
    };
}
