using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Creature
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
        GameManager.Get().queueCardPickerEffect(controller, controller.deck.getAllCardsWithTag(Card.Tag.Gem), new ETBReceiver(controller), 1, 1, false, "Select a Gem to add to your hand");
    }

    private class ETBReceiver : CanReceivePickedCards
    {
        private Player controller;

        public ETBReceiver(Player controller)
        {
            this.controller = controller;
        }

        public void receiveCardList(List<Card> cardList)
        {
            foreach (Card c in cardList)
            {
                controller.hand.addCardByEffect(c);
            }
        }
    }
}
