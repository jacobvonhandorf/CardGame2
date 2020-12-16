using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyCavalierEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        SingleTileTargetEffect.CreateAndQueue(Board.Instance.GetAllTilesWithCreatures(creature.Controller, true), delegate (Tile t)
        {
            t.creature.Bounce(card);
        });
    };
}
