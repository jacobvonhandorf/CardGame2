using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        card.owner.DrawCard();
    }

    public override bool CanBePlayed => card.owner.ControlledCreatures.FindAll(c => c.HasTag(Card.Tag.Arcane)).Count > 0;
}
