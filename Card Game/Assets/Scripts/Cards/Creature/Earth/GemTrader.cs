using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTrader : Creature
{
    public override int getCardId()
    {
        return 70;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        CanReceivePickedCards receiver = new Receiver(this);
        if (controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count > 0)
            GameManager.Get().queueCardPickerEffect(controller, controller.hand.getAllCardsWithTag(Card.Tag.Gem), receiver, 1, 1, false, "Select a Gem to discard");
    }

    public override void onDeath()
    {
        Card obsidian = GameManager.Get().createCardById(Obsidian.CARD_ID, controller);
        obsidian.moveToCardPile(controller.deck, sourceCard);
        controller.deck.shuffle();
    }

    private class Receiver : CanReceivePickedCards
    {
        private Creature creature;

        public Receiver(Creature creature)
        {
            this.creature = creature;
        }

        public void receiveCardList(List<Card> cardList)
        {
            foreach (Card c in cardList)
            {
                c.moveToCardPile(creature.controller.deck, creature.sourceCard);
                creature.controller.deck.shuffle();
            }
            foreach (Card c in creature.controller.deck.getCardList())
            {
                if (c.hasTag(Card.Tag.Gem))
                {
                    c.moveToCardPile(creature.controller.hand, creature.sourceCard);
                    break;
                }
            }
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
