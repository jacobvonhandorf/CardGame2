using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindToDustEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.GetAllTilesWithCreatures(card.Owner.OppositePlayer, false);

    public override void DoEffect(Tile t)
    {
        t.Creature.TakeDamage(card.Owner.Hand.GetAllCardsWithTag(Tag.Gem).Count, card);
    }
}
