using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.getAllTilesWithCreatures(card.owner.getOppositePlayer(), false);
    public override bool canBePlayed => card.owner.hasCreatureWithTag(Card.Tag.Arcane);

    public override void doEffect(Tile t)
    {
        t.creature.takeDamage(2, card);
    }
}
