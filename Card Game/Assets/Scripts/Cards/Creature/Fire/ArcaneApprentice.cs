using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneApprentice : Creature, CanReceivePickedCards
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        GameManager.Get().queueCardPickerEffect(controller, controller.deck.getAllCardWithTagAndType(Card.Tag.Arcane, Card.CardType.Spell), this, 1, 1, "Select a card to add to your hand");
    }

    public void receiveCardList(List<Card> cardList)
    {
        controller.hand.addCardByEffect(cardList[0]);
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    public override int getCardId()
    {
        return 66;
    }
}
