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
                Destroy(c.gameObject);
            return;
        }
        foreach (Card card in GetComponentsInChildren<CreatureCard>())
        {
            card.owner = deckOwner;
            //(card as CreatureCard).creature.owner = deckOwner; Creatures don't know owner anymore
            (card as CreatureCard).Creature.Controller = deckOwner;
            card.MoveToCardPile(this, null);
        }
        foreach (Card card in GetComponentsInChildren<StructureCard>())
        {
            card.owner = deckOwner;
            (card as StructureCard).structure.SourceCard.owner = deckOwner;
            (card as StructureCard).structure.Controller = deckOwner;
            AddCard(card);
            card.MoveToCardPile(this, null);
        }
        foreach (Card card in GetComponentsInChildren<SpellCard>())
        {
            card.owner = deckOwner;
            AddCard(card);
            card.MoveToCardPile(this, null);
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
        NetInterface.Get().SyncDeckOrder(this);
    }

    protected override void OnCardAdded(Card c)
    {
        c.removeGraphicsAndCollidersFromScene(); // card in deck should not be clickable
        c.positionOnScene = transform.position;
        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    protected override void OnCardRemoved(Card c)
    {
        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    public void PrintCardList() // used for debugging
    {
        Debug.Log("Printing deck cardList");
        foreach(Card c in cardList)
        {
            Debug.Log(c.transform.name);
        }
    }

    // only use when loading a new deck
    public void ClearDeck()
    {
        foreach (Card c in cardList)
        {
            Destroy(c.gameObject);
        }
        cardList.Clear();
        cardCountText.text = "0";
    }
}
