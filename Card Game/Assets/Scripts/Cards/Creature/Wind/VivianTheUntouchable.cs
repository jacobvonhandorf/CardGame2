using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivianTheUntouchable : Creature
{
    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>() { Keyword.quick };
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Fairy);
        return tags;
    }

    public override void onInitialization()
    {
        sourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        sourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
    }

    private void SourceCard_E_AddedToCardPile(object sender, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
            SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllDeployableTiles(sourceCard.owner), delegate (Tile t)
            {
                GameManager.Get().createCreatureOnTile(this, t, sourceCard.owner, sourceCard);
            });
    }

    public override int getCardId()
    {
        return 45;
    }
}
