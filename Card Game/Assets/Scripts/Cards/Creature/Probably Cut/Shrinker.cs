using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinker : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override Effect getEffect()
    {
        return null;
    }

    public override int getCardId()
    {
        return 54;
    }

    /*
    private class ShrinkerEffect : SingleTileTargetEffect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.addHealth(-1);
            targetCreature.addAttack(-1);
        }

        public bool canBeCancelled()
        {
            throw new System.NotImplementedException();
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            // List<Tile> validTargets = GameManager.Get().getAllTilesWithCreatures(oppositePlayer);
            List<Tile> validTargets = GameManager.Get().getAllTilesWithCreatures(false);
            validTargets.Remove(sourceTile);
            validTargets.RemoveAll(t => t.getDistanceTo(sourceTile) > 3);
            return validTargets;
        }
    }
    */
}
