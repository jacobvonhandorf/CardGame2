using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritRejuvination : SpellCard
{
    public override int getCardId()
    {
        return 1;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(owner);
    }

    protected override Effect getEffect()
    {
        return new RejuvEff();
    }

    private class RejuvEff : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.bounce();
            sourcePlayer.drawCards(2);
        }
    }
}
