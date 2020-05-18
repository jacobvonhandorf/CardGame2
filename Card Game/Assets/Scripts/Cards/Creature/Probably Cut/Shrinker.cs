﻿using System.Collections;
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
        return new ShrinkerEffect();
    }

    public override int getCardId()
    {
        return 54;
    }

    private class ShrinkerEffect : SingleTileTargetEffect
    {
        public override void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.addHealth(-1);
            targetCreature.addAttack(-1);
        }

        public override List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            // List<Tile> validTargets = GameManager.Get().getAllTilesWithCreatures(oppositePlayer);
            List<Tile> validTargets = GameManager.Get().getAllTilesWithCreatures();
            validTargets.Remove(sourceTile);
            validTargets.RemoveAll(t => t.getDistanceTo(sourceTile) > 3);
            return validTargets;
        }
    }
}
