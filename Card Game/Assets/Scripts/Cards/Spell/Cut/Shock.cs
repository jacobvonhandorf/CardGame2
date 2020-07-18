using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : SpellCard
{
    public override int cardId => 2;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(false);

    protected override void doEffect(Tile t)
    {
        t.creature.takeDamage(2, this);
    }
}
