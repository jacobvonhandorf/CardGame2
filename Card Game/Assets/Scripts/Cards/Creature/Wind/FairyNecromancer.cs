using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyNecromancer : Creature
{
    public override int cardId => 47;

    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            c.moveToCardPile(controller.hand, SourceCard);
        }
    }
}
