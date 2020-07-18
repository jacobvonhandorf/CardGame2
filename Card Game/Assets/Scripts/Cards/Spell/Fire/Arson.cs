using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arson : SpellCard
{
    public override int cardId => 15;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithStructures(GameManager.Get().getOppositePlayer(owner));

    protected override void doEffect(Tile t)
    {
        t.structure.takeDamage(2, this);
    }
}
