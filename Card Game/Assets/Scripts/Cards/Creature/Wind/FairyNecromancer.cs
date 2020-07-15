using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyNecromancer : Creature
{
    public override int getCardId()
    {
        return 47;
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

    public override void onCreation()
    {
        throw new System.Exception("Not implemented");
        //GameManager.Get().queueCardPickerEffect(controller, controller.graveyard.getAllCardWithTagAndType(Card.Tag.Fairy, Card.CardType.Creature), this, 1, 1, false, "Choose a card to add to your hand");
    }

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            c.moveToCardPile(controller.hand, sourceCard);
        }
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
