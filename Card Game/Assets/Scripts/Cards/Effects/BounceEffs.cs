using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.instance.GetAllTilesWithCreatures(false);

    public override void DoEffect(Tile t)
    {
        t.creature.Bounce(card);
    }
}
