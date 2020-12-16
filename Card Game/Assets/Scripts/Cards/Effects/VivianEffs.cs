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
            SingleTileTargetEffect.CreateAndQueue(GameManager.Instance.getAllDeployableTiles(card.Owner), delegate (Tile t)
            {
                GameManager.Instance.createCreatureOnTile(creature, t, card.Owner);
            });
        }
    };
}
