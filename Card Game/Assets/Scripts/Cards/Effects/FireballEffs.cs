using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEffs : SpellEffects
{
    public int damageAmount = 5;

    public override List<Tile> validTiles => Board.instance.getAllTilesWithCreatures(owner.oppositePlayer, false);

    public override void doEffect(Tile t)
    {
        t.creature.takeDamage(damageAmount, card);
    }

    public override bool canBePlayed => owner.controlledCreatures.Find(c => c.hasTag(Card.Tag.Arcane));
}
