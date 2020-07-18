using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritRejuvination : SpellCard
{
    public override int cardId => 1;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(owner);

    protected override void doEffect(Tile t)
    {
        t.creature.bounce(this);
        owner.drawCards(2);
    }
}
