using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTheCard : SpellCard
{
    public override int getCardId()
    {
        return 8;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures();
    }

    protected override Effect getEffect()
    {
        return new BounceEffect();
    }

    private class BounceEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.bounce();
        }
    }
}
