using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTrader : Creature
{
    public override int cardId => 70;
    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy };

    public override void onCreation()
    {
        CardPicker.CreateAndQueue(controller.hand.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a card to shuffle into your deck", controller, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
            {
                c.moveToCardPile(controller.deck, sourceCard);
                controller.deck.shuffle();
            }
            foreach (Card c in controller.deck.getCardList())
            {
                if (c.hasTag(Card.Tag.Gem))
                {
                    c.moveToCardPile(controller.hand, sourceCard);
                    break;
                }
            }
        });
    }

    public override void onDeath()
    {
        Card obsidian = GameManager.Get().createCardById(Obsidian.CARD_ID, controller);
        obsidian.moveToCardPile(controller.deck, sourceCard);
        controller.deck.shuffle();
    }
}
