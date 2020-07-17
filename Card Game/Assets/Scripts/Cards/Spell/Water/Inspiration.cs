using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspiration : SpellCard
{
    public override int cardId => 9;

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return new InspirationEffect();
    }

    private class InspirationEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            sourcePlayer.drawCards(2);
        }
    }
}
