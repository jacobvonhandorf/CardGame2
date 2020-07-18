using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : SpellCard
{
    public override int cardId => 10;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(owner, true);

    protected override void doEffect(Tile t)
    {
        t.creature.hasMovedThisTurn = false;
        t.creature.hasDoneActionThisTurn = false;
        t.creature.updateHasActedIndicators();
        owner.addActions(1);
    }
}
