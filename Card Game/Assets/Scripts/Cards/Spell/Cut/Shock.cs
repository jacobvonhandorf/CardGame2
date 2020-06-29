using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : SpellCard
{
    public override int getCardId()
    {
        return 2;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(false);
    }

    protected override Effect getEffect()
    {
        return new ShockEffect();
    }

    private class ShockEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.takeDamage(2);
        }
    }
}
