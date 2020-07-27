using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianEffs : SpellEffects
{
    [SerializeField] private int damageAmount = 2;
    public override List<Tile> validTiles => new List<Tile>();
    public override void doEffect(Tile t) { }
    public override bool canBePlayed => false;

    public override EventHandler<Card.AddedToCardPileArgs> onMoveToCardPile => delegate (object s, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
            doEffect();
    };

    private void doEffect()
    {
        card.owner.drawCard();
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(card.owner.oppositePlayer, false), delegate (Tile t)
        {
            t.creature.takeDamage(damageAmount, card);
        });
    }
}
