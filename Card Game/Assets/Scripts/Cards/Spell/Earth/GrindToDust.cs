using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindToDust : SpellCard
{
    public override int cardId => 19;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(owner), false);

    protected override void doEffect(Tile t)
    {
        t.creature.takeDamage(owner.hand.getAllCardsWithTag(Tag.Gem).Count, this);
    }
}
