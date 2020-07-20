using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysticRetort : SpellCard
{
    public override int cardId => 0;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    protected override void doEffect(Tile t)
    {
        List<Creature> creaturesToBounce = new List<Creature>();
        foreach (Creature c in GameManager.Get().allCreatures) // need to move into a seperate list because bouncing removes from allCreatures
            creaturesToBounce.Add(c);
        foreach (Creature c in creaturesToBounce)
            c.bounce(this);
    }
}
