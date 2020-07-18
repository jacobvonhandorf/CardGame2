using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspiration : SpellCard
{
    public override int cardId => 9;
    public override List<Tile> legalTargetTiles => GameManager.Get().allTiles();

    protected override void doEffect(Tile t)
    {
        owner.drawCards(2);
    }
}
