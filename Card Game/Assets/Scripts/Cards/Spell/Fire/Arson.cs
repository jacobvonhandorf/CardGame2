using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arson : SpellCard, Effect
{
    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        targetTile.structure.takeDamage(2);
    }

    public override int getCardId()
    {
        return 15;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithStructures(GameManager.Get().getOppositePlayer(owner));
    }

    protected override Effect getEffect()
    {
        return this;
    }
}
