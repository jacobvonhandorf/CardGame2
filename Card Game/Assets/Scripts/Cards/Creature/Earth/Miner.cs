using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Creature, CanReceivePickedCards
{
    public override int getCardId()
    {
        return 68;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        GameManager.Get().queueCardPickerEffect(controller, controller.deck.getAllCardsWithTag(Card.Tag.Gem), this, 1, 1, false, "Select a Gem to add to your hand");

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
            Keyword.deploy,
            Keyword.armored1
        };
    }

}
