using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : Creature, CanReceivePickedCards
{
    public override int getCardId()
    {
        return 67;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAttack()
    {
        List<Card> deckList = controller.deck.getCardList();
        List<Card> revealedCards = new List<Card>();

        foreach (Card c in deckList)
        {
            if (revealedCards.Count < 5)
                revealedCards.Add(c);
            else
                break;
        }

        List<Card> pickableCards = new List<Card>();
        foreach (Card c in revealedCards)
        {
            if (c.hasTag(Card.Tag.Gem))
                pickableCards.Add(c);
        }
        if (pickableCards.Count > 0)
            GameManager.Get().queueCardPickerEffect(controller, pickableCards, this, 1, 1, false,  "Select a Gem to add to your hand");
        else
            GameManager.Get().showToast("Prospector found no Gems");
    }

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            controller.hand.addCardByEffect(c);
        }
    }
}
