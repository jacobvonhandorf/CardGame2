using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivianTheUntouchable : Creature
{
    public override int cardId => 45;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public override void onInitialization()
    {
        Debug.LogError("vivan init");
        SourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        SourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
    }

    private void SourceCard_E_AddedToCardPile(object sender, Card.AddedToCardPileArgs e)
    {
        Debug.Log("Vivian added to hand");
        if (e.newCardPile is Hand && e.source != null)
            SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllDeployableTiles(SourceCard.owner), delegate (Tile t)
            {
                GameManager.Get().createCreatureOnTile(this, t, SourceCard.owner);
            });
    }
}
