using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTheCard : SpellCard
{
    public override int cardId => 8;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(false);

    protected override void doEffect(Tile t)
    {
        t.creature.bounce(this);
    }
}
