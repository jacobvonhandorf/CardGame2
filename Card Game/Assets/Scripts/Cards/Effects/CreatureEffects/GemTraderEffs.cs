using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTraderEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        CardPicker.CreateAndQueue(creature.Controller.hand.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a card to shuffle into your deck", creature.Controller, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
            {
                c.moveToCardPile(creature.Controller.deck, card);
                creature.Controller.deck.shuffle();
            }
            foreach (Card c in creature.Controller.deck.getCardList())
            {
                if (c.Tags.Contains(Card.Tag.Gem))
                {
                    c.moveToCardPile(creature.Controller.hand, card);
                    break;
                }
            }
        });
    };

    public override EventHandler onDeath => delegate (object s, EventArgs e)
    {
        Card obsidian = GameManager.Get().createCardById((int)CardIds.Obsidian, creature.Controller);
        obsidian.moveToCardPile(creature.Controller.deck, card);
        creature.Controller.deck.shuffle();
    };
}
