using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBounce : SpellCard
{
    public const int CARD_ID = 79;
    public override int cardId => CARD_ID;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(false);

    protected override void doEffect(Tile t)
    {
        List<Tile> validTargets = GameManager.Get().getAllTilesWithCreatures(false);
        validTargets.Remove(t.creature.currentTile);
        if (validTargets.Count > 0)
            SingleTileTargetEffect.CreateAndQueue(validTargets, delegate (Tile targetTile)
            {
                targetTile.creature.bounce(this);
                t.creature.bounce(this);
            });
    }
}
