using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneInfluenceEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.getAllTilesWithCreatures(card.owner, true);

    public override void doEffect(Tile t)
    {
        t.creature.Counters.add(CounterType.Arcane, 2);
        card.owner.drawCard();
    }
}
