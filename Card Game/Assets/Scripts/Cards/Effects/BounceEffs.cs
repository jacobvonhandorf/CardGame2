﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.GetAllTilesWithCreatures(false);

    public override void DoEffect(Tile t)
    {
        t.Creature.Bounce(card);
    }
}
