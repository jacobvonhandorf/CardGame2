using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritRejuvinationEffs : SpellEffects
{
    public override List<Tile> validTiles => GameManager.Get().getAllTilesWithCreatures(owner, true);

    public override void doEffect(Tile t)
    {
        t.creature.Bounce(card);
        owner.DrawCards(2);
    }
}
