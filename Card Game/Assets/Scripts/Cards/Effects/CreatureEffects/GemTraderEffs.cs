using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTraderEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        CardPicker.CreateAndQueue(creature.controller.hand.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a card to shuffle into your deck", creature.controller, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
            {
                c.moveToCardPile(creature.controller.deck, card);
                creature.controller.deck.shuffle();
            }
            foreach (Card c in creature.controller.deck.getCardList())
            {
                if (c.hasTag(Card.Tag.Gem))
                {
                    c.moveToCardPile(creature.controller.hand, card);
                    break;
                }
            }
        });
    };

    public override EventHandler onDeath => delegate (object s, EventArgs e)
    {
        Card obsidian = GameManager.Get().createCardById((int)CardIds.Obsidian, creature.controller);
        obsidian.moveToCardPile(creature.controller.deck, card);
        creature.controller.deck.shuffle();
    };
}
