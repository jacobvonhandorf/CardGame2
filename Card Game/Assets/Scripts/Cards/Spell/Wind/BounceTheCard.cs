using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTheCard : SpellCard, Effect
{
    public override int getCardId()
    {
        return 8;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(false);
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        targetCreature.bounce(this);
    }
}
