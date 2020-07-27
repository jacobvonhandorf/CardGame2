using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyCavalierEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(creature.controller, true), delegate (Tile t)
        {
            t.creature.bounce(card);
        });
    };
}
