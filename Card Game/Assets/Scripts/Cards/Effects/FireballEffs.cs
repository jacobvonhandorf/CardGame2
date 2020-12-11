using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEffs : SpellEffects
{
    public int damageAmount = 5;

    public override List<Tile> ValidTiles => Board.instance.GetAllTilesWithCreatures(Owner.OppositePlayer, false);

    public override void DoEffect(Tile t)
    {
        t.creature.TakeDamage(damageAmount, card);
    }

    public override bool CanBePlayed => Owner.ControlledCreatures.Find(c => c.HasTag(Card.Tag.Arcane));
}
