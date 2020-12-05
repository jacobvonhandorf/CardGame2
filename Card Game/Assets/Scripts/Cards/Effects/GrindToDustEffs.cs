using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindToDustEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.getAllTilesWithCreatures(card.owner.OppositePlayer, false);

    public override void doEffect(Tile t)
    {
        t.creature.TakeDamage(card.owner.hand.getAllCardsWithTag(Card.Tag.Gem).Count, card);
    }
}
