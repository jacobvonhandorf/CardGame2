using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alchemy : SpellCard
{
    public override int cardId => 16;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    protected override void doEffect(Tile t)
    {
        owner.addGold(2);
    }
}
