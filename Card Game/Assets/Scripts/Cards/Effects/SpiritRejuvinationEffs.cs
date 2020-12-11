using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritRejuvinationEffs : SpellEffects
{
    public override List<Tile> ValidTiles => GameManager.Get().getAllTilesWithCreatures(Owner, true);

    public override void DoEffect(Tile t)
    {
        t.creature.Bounce(card);
        Owner.DrawCards(2);
    }
}
