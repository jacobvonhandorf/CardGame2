using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyNecromancer : Creature
{
    public override int cardId => 47;

    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            c.moveToCardPile(controller.hand, sourceCard);
        }
    }

    public override List<Keyword> getInitialKeywords()
    {
        throw new System.Exception("Not implemented");
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
