using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        card.owner.drawCard();
    }

    public override bool canBePlayed => card.owner.getAllControlledCreatures().FindAll(c => c.hasTag(Card.Tag.Arcane)).Count > 0;
}
