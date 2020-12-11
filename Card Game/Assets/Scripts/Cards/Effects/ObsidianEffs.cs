using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianEffs : SpellEffects
{
    [SerializeField] private int damageAmount = 2;
    public override List<Tile> ValidTiles => new List<Tile>();
    public override void DoEffect(Tile t) { }
    public override bool CanBePlayed => false;

    public override EventHandler<Card.AddedToCardPileArgs> OnMoveToCardPile => delegate (object s, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
            doEffect();
    };

    private void doEffect()
    {
        card.owner.DrawCard();
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(card.owner.OppositePlayer, false), delegate (Tile t)
        {
            t.creature.TakeDamage(damageAmount, card);
        });
    }
}
