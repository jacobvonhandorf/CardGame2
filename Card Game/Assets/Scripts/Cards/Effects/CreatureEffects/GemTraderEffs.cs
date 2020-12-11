﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTraderEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        CardPicker.CreateAndQueue(creature.Controller.Hand.GetAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a card to shuffle into your deck", creature.Controller, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
            {
                c.MoveToCardPile(creature.Controller.Deck, card);
                creature.Controller.Deck.shuffle();
            }
            foreach (Card c in creature.Controller.Deck.CardList)
            {
                if (c.Tags.Contains(Card.Tag.Gem))
                {
                    c.MoveToCardPile(creature.Controller.Hand, card);
                    break;
                }
            }
        });
    };

    public override EventHandler onDeath => delegate (object s, EventArgs e)
    {
        Card obsidian = GameManager.Get().createCardById((int)CardIds.Obsidian, creature.Controller);
        obsidian.MoveToCardPile(creature.Controller.Deck, card);
        creature.Controller.Deck.shuffle();
    };
}
