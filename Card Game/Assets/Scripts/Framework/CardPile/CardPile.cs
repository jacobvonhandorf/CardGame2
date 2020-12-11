using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Card;

//This is a parent class for all objects that are a pile of cards ex: deck, hand, grave
public abstract class CardPile : MonoBehaviour
{
    public IReadOnlyList<Card> CardList => cardList;
    public UnityEvent numCardsChanged = new UnityEvent();
    [SerializeField] protected bool ownedByLocalPlayer; // used for Net Code
    protected List<Card> cardList = new List<Card>();

    protected void Awake()
    {
        NetInterface.Get().RegisterCardPile(this, ownedByLocalPlayer);
    }

    // this method is dangerous to call. If possible use Card.moveToCardPile()
    public void AddCard(Card c)
    {
        if (cardList.Contains(c)) // trying to add a card that's already in the list
            return;
        cardList.Add(c);

        c.transform.SetParent(transform);
        OnCardAdded(c);
        numCardsChanged.Invoke();
    }

    // this method is dangerous to call. If possible use Card.moveToCardPile()
    public void AddCards(List<Card> newCards)
    {
        foreach (Card c in newCards)
        {
            AddCard(c);
        }
    }

    // IF YOU CALL THIS METHOD MAKE SURE YOU MOVE THIS CARD TO ANOTHER PILE
    // It's probably better to call Card.moveToCardPile()
    public Card RemoveCard(Card cardToRemove)
    {
        foreach (Card c in cardList)
        {
            if (c == cardToRemove)
            {
                cardList.Remove(c);
                OnCardRemoved(c);
                numCardsChanged.Invoke();
                return c;
            }
        }
        return null;
    }

    protected virtual void OnCardAdded(Card c)
    {
        // throw new NotImplementedException();
        // Debug.Log("Should probably override onCardAdded");
    }

    protected virtual void OnCardRemoved(Card c)
    {
        // Debug.Log("OnCardRemoved called and not overriden");
    }

    public List<Card> GetAllCardsWithTag(Tag tag) => cardList.FindAll(c => c.Tags.Contains(tag));

    public List<Card> GetAllCardsWithType(CardType type) => cardList.FindAll(c => c.IsType(type));

    public List<Card> GetAllCardWithTagAndType(Tag tag, CardType type)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
        {
            if (c.Tags.Contains(tag) && c.IsType(type))
                returnList.Add(c);
        }
        return returnList;
    }

    public List<Card> getAllCardsWithLessThanOrEqualCost(int cost)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
        {
            if (c.TotalCost <= cost)
            {
                returnList.Add(c);
            }
        }
        return returnList;
    }

    public List<Card> getAllCardsWithinTotalCostRange(int min, int max)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
        {
            if (c.TotalCost <= max && c.TotalCost >= min)
                returnList.Add(c);
        }
        return returnList;
    }

    // only call this from NetInterface
    public void SyncOrderFromNetwork(List<Card> newCardList)
    {
        cardList = newCardList;
    }

    public void PrintCardList()
    {
        foreach (Card c in cardList)
        {
            Debug.Log(c.transform.name);
        }
    }
}
