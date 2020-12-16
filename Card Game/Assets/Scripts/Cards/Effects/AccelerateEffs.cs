using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        card.Owner.DrawCard();
    }

    public override bool CanBePlayed => card.Owner.ControlledCreatures.FindAll(c => c.HasTag(Tag.Arcane)).Count > 0;
}
