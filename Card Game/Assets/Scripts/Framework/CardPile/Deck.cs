using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Deck : CardPile, IScriptDeck
{
    private System.Random rng = new System.Random();

    public void Shuffle()
    {
        int n = cardList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card card = cardList[k];
            cardList[k] = cardList[n];
            cardList[n] = card;
        }
        NetInterface.Get().SyncDeckOrder(this);
    }

    public void PlaceCardAtPosition(Card c, int position, Card source)
    {
        if (!cardList.Remove(c))
            c.MoveToCardPile(this, source); // if the card isn't already in the list then move it there
        if (position < cardList.Count)
            cardList.Insert(position, c);
        else
            cardList.Insert(cardList.Count - 1, c);
        NetInterface.Get().SyncDeckOrder(this);
    }

    protected override void OnCardAdded(Card c)
    {
        c.removeGraphicsAndCollidersFromScene(); // card in deck should not be clickable
        c.positionOnScene = transform.position;
    }

    // only use when loading a new deck
    public void ClearDeck()
    {
        foreach (Card c in cardList)
        {
            Destroy(c.gameObject);
        }
        cardList.Clear();
    }
}
