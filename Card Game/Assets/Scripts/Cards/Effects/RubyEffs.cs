using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyEffs : SpellEffects
{
    public override List<Tile> ValidTiles => new List<Tile>();
    public override void DoEffect(Tile t) { }
    public override bool CanBePlayed => false;

    public override EventHandler<Card.AddedToCardPileArgs> OnMoveToCardPile => delegate (object s, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
            activate();
    };

    private void activate()
    {
        SingleTileTargetEffect.CreateAndQueue(Board.Instance.GetAllTilesWithCreatures(Owner), delegate (Tile t)
        {
            t.creature.AttackStat += 1;
            t.creature.Health += 1;
        });
    }
}
