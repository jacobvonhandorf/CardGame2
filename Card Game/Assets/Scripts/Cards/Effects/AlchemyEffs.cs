using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyEffs : SpellEffects
{
    public int goldToAdd;
    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        card.owner.Gold += goldToAdd;
    }
}
