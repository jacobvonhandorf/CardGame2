using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneInfluenceEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.GetAllTilesWithCreatures(card.Owner, true);

    public override void DoEffect(Tile t)
    {
        t.creature.Counters.Add(CounterType.Arcane, 2);
        card.Owner.DrawCard();
    }
}
