using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMage : Creature, CanReceivePickedCards
{
    public const int CARD_ID = 80;

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    private List<Card> cardReferences = new List<Card>();
    public override void onCreation()
    {

        cardReferences.Add(GameManager.Get().createCardById(PowerDraw.CARD_ID, controller));
        cardReferences.Add(GameManager.Get().createCardById(PowerBlast.CARD_ID, controller));
        cardReferences.Add(GameManager.Get().createCardById(PowerBounce.CARD_ID, controller));
        GameManager.Get().queueCardPickerEffect(controller, cardReferences, this, 1, 1, true, "Select a card to add to your hand");
    }

    public void receiveCardList(List<Card> cardList)
    {
        cardList[0].moveToCardPile(controller.hand, true);
        foreach (Card c in cardReferences)
        {
            if (c != cardList[0])
                GameManager.Get().destroyCard(c);
        }
        cardReferences.Clear();
    }

    public override List<Card.Tag> getTags()
    {
        return new List<Card.Tag>() { Card.Tag.Arcane };
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>() { Keyword.deploy };
    }
}
