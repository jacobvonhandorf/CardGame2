﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspirationEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        card.Owner.DrawCards(2);
    }
}
