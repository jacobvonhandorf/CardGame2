using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBounce : SpellCard, Effect, SingleTileTargetEffect
{
    public const int CARD_ID = 79;

    private bool firstBounceDone = false;
    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        // bounce the first creature
        targetCreature.bounce();

        if (firstBounceDone)
        {
            firstBounceDone = false;
            return;
        }
        firstBounceDone = true;

        // check again for legal targets
        // if there are then ask for second target
        List<Tile> remainingTargetTiles = GameManager.Get().getAllTilesWithCreatures();
        if (remainingTargetTiles.Count > 0)
        {
            GameManager.Get().setUpSingleTileTargetEffect(this, sourcePlayer, null, null, null, "Select a second creature to bounce", true);
        }
    }

    private class SecondBounce : SingleTileTargetEffect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.bounce();
        }

        public bool canBeCancelled()
        {
            return true;
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            return GameManager.Get().getAllTilesWithCreatures();
        }
    }

    // SpellCard
    public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
    {
        return GameManager.Get().getAllTilesWithCreatures();
    }

    public bool canBeCancelled()
    {
        return true;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    // SingleTileTargetEffect
    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures();
    }

    protected override Effect getEffect()
    {
        return this;
    }

}
