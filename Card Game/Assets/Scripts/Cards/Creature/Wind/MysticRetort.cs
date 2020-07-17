using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysticRetort : SpellCard, Effect
{
    public override int cardId => 0;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        List<Creature> creaturesToBounce = new List<Creature>();
        foreach (Creature c in GameManager.Get().allCreatures) // need to move into a seperate list because bouncing removes from allCreatures
            creaturesToBounce.Add(c);
        foreach (Creature c in creaturesToBounce)
            c.bounce(this);
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }
}
