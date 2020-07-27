using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspirationEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        card.owner.drawCards(2);
    }
}
