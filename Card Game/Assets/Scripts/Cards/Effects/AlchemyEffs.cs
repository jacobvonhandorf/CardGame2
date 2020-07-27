using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyEffs : SpellEffects
{
    public int goldToAdd;
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        card.owner.addGold(goldToAdd);
    }
}
