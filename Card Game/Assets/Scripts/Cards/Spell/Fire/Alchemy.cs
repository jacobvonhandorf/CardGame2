using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alchemy : SpellCard
{
    public override int cardId => 16;

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return new AlchemyEffect();
    }

    private class AlchemyEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            sourcePlayer.addGold(2);
        }
    }
}
