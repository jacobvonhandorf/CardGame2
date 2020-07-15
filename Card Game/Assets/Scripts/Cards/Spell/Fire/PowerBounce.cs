using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBounce : SpellCard, Effect
{
    public const int CARD_ID = 79;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        List<Tile> validTargets = GameManager.Get().getAllTilesWithCreatures(false);
        validTargets.Remove(targetCreature.currentTile);
        if (validTargets.Count > 0)
            SingleTileTargetEffect.CreateAndQueue(validTargets, delegate (Tile t)
            {
                targetCreature.bounce(this);
                t.creature.bounce(this);
            });
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(false);
    }
}
