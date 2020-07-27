using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarnetEffs : SpellEffects
{
    public override List<Tile> validTiles => new List<Tile>();
    public override void doEffect(Tile t) { }
    public override bool canBePlayed => false;

    public override EventHandler<Card.AddedToCardPileArgs> onMoveToCardPile => delegate (object s, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
            owner.addGold(1);
    };
}
