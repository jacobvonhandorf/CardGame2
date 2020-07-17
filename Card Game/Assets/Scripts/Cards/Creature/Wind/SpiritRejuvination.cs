using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritRejuvination : SpellCard, Effect
{
    public override int cardId => 1;

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(owner);
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        targetCreature.bounce(this);
        sourcePlayer.drawCards(2);
    }
}
