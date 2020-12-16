using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyEffs : SpellEffects
{
    public int goldToAdd;
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        card.Owner.Gold += goldToAdd;
    }
}
