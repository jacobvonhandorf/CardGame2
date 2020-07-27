using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivianEffs : CreatureEffects
{
    public override EventHandler<Card.AddedToCardPileArgs> onMoveToCardPile => delegate (object s, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
        {
            SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllDeployableTiles(card.owner), delegate (Tile t)
            {
                GameManager.Get().createCreatureOnTile(creature, t, card.owner);
            });
        }
    };
}
