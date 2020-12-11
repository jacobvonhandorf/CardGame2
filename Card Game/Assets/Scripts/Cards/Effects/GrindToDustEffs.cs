using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindToDustEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.instance.GetAllTilesWithCreatures(card.owner.OppositePlayer, false);

    public override void DoEffect(Tile t)
    {
        t.creature.TakeDamage(card.owner.Hand.GetAllCardsWithTag(Card.Tag.Gem).Count, card);
    }
}
