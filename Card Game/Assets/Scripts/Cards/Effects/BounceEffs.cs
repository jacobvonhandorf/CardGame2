using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.getAllTilesWithCreatures(false);

    public override void doEffect(Tile t)
    {
        t.creature.Bounce(card);
    }
}
