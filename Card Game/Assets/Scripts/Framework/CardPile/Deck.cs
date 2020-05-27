using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Deck : CardPile
{
    // WHEN A METHOD IS ADDED TO PLACE A CARD AT A SPECIFIC POSITION, IT MUST SYNC DECK ORDER IN NET INTERFACE
    private System.Random rng = new System.Random();
    [SerializeField] public TextMeshPro cardCountText;
    [SerializeField] public Player deckOwner;

    protected new void Awake()
    {
        base.Awake();
        if (cardList == null)
            cardList = new List<Card>();
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            foreach (Card c in GetComponentsInChildren<Card>())
                Destroy(c.getRootTransform().gameObject);
            return;
        }
        foreach (Card card in GetComponentsInChildren<CreatureCard>())
        {
            card.owner = deckOwner;
            //(card as CreatureCard).creature.owner = deckOwner; Creatures don't know owner anymore
            (card as CreatureCard).creature.controller = deckOwner;
            card.moveToCardPile(this);
        }
        foreach (Card card in GetComponentsInChildren<StructureCard>())
        {
            card.owner = deckOwner;
            (card as StructureCard).structure.owner = deckOwner;
            (card as StructureCard).structure.controller = deckOwner;
            addCard(card);
            card.moveToCardPile(this);
        }
        foreach (Card card in GetComponentsInChildren<SpellCard>())
        {
            card.owner = deckOwner;
            addCard(card);
            card.moveToCardPile(this);
        }
        shuffle();
    }

    private void Start()
    {
        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    public void shuffle()
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
        foreach(Card c in cardList)
        {
            c.hide();
        }
        NetInterface.Get().syncDeckOrder(this);
    }
    /*
    public Card draw()
    {
        if (cardList.Count == 0)
        {
            GameManager.Get().playerHasDrawnOutDeck(deckOwner);
            return null;
        }
        return removeCard(cardList[0]);
    }*/

        /*
    public List<Card> draw(int numCards)
    {
        List<Card> returnList = new List<Card>();
        for (int i = 0; i < numCards; i++)
            returnList.Add(draw());
        return returnList;
    }*/

    protected override void onCardAdded(Card c)
    {
        c.removeGraphicsAndCollidersFromScene(); // card in deck should not be clickable
        c.positionOnScene = transform.position;
        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    // usually what will be used for drawing
    internal Card getTopCard()
    {
        return cardList[0];
    }

    protected override void onCardRemoved(Card c)
    {
        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    public void printCardList() // used for debugging
    {
        Debug.Log("Printing deck cardList");
        foreach(Card c in cardList)
        {
            Debug.Log(c.getRootTransform().name);
        }
    }

    // only use when loading a new deck
    public void clearDeck()
    {
        foreach (Card c in cardList)
        {
            Destroy(c.getRootTransform().gameObject);
        }
        cardList.Clear();
        cardCountText.text = "0";
    }

}
