using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : SpellCard
{
    public override int getCardId()
    {
        return 10;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(owner);
    }

    protected override Effect getEffect()
    {
        return new RushEffect();
    }

    private class RushEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.hasMovedThisTurn = false;
            targetCreature.hasDoneActionThisTurn = false;
            sourcePlayer.addActions(1);
        }
    }
}
