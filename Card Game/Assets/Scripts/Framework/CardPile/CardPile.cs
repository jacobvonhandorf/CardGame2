using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

/*
 * This is a parent class for all objects that are a pile of cards ex: deck, hand, grave
 */

public class CardPile : MonoBehaviour
{
    [SerializeField] protected bool ownedByLocalPlayer; // used for Net Code
    protected List<Card> cardList;

    protected void Awake()
    {
        nullCheckList();
        NetInterface.Get().registerCardPile(this, ownedByLocalPlayer);
    }

    // this method is dangerous to call. If possible use Card.moveToCardPile()
    public void addCard(Card c)
    {
        nullCheckList();
        if (cardList.Contains(c)) // trying to add a card that's already in the list
            return;
        cardList.Add(c);

        c.getRootTransform().SetParent(transform);
        onCardAdded(c);
    }

    // this method is dangerous to call. If possible use Card.moveToCardPile()
    public void addCards(List<Card> newCards)
    {
        foreach (Card c in newCards)
        {
            addCard(c);
        }
    }

    protected void nullCheckList()
    {
        if (cardList == null)
            cardList = new List<Card>();
    }

    // IF YOU CALL THIS METHOD MAKE SURE YOU MOVE THIS CARD TO ANOTHER PILE
    // It's probably better to call Card.moveToCardPile()
    public Card removeCard(Card cardToRemove)
    {
        foreach (Card c in cardList)
        {
            if (c == cardToRemove)
            {
                cardList.Remove(c);
                onCardRemoved(c);
                return c;
            }
        }
        return null;
    }

    protected virtual void onCardAdded(Card c)
    {
        // throw new NotImplementedException();
        Debug.Log("Should probably override onCardAdded");
    }

    protected virtual void onCardRemoved(Card c)
    {
        Debug.Log("OnCardRemoved called and not overriden");
    }

    /*
     * Returns a list of all cards in this pile that have a specified tag
     */
    public List<Card> getAllCardsWithTag(Tag tag)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
            if (c.hasTag(tag))
                returnList.Add(c);
        return returnList;
    }

    public List<Card> getAllCardsWithType(CardType type)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
            if (c.isType(type))
                returnList.Add(c);
        return returnList;
    }

    public List<Card> getAllCardWithTagAndType(Tag tag, CardType type)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
        {
            if (c.hasTag(tag) && c.isType(type))
                returnList.Add(c);
        }
        return returnList;
    }

    public List<Card> getAllCardsWithLessThanOrEqualCost(int cost)
    {
        List<Card> returnList = new List<Card>();
        foreach (Card c in cardList)
        {
            if (c.getTotalCost() <= cost)
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
            if (c.getTotalCost() <= max && c.getTotalCost() >= min)
                returnList.Add(c);
        }
        return returnList;
    }

    public List<Card> getCardList()
    {
        return cardList;
    }

    // only call this from NetInterface
    public void syncOrderFromNetwork(List<Card> newCardList)
    {
        this.cardList = newCardList;
    }

}
