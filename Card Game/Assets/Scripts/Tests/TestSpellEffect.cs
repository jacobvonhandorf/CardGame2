using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.getAllTilesWithCreatures(card.owner.getOppositePlayer(), false);
    public override bool canBePlayed => card.owner.HasCreatureWithTag(Card.Tag.Arcane);

    public override void doEffect(Tile t)
    {
        t.creature.TakeDamage(2, card);
    }
}
