using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondEffs : SpellEffects
{
    public override List<Tile> ValidTiles => new List<Tile>();
    public override void DoEffect(Tile t) { }
    public override bool CanBePlayed => false;

    public override EventHandler<Card.AddedToCardPileArgs> OnMoveToCardPile => delegate (object s, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
            card.owner.DrawCard();
    };
}
