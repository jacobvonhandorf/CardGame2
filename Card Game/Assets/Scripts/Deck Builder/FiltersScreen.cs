using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiltersScreen : MonoBehaviour
{
    [SerializeField] DeckBuilderCardsView cardsView;

    public void toggleCardType(string cardType)
    {
        toggleCardType((CardType) System.Enum.Parse(typeof(CardType), cardType, true));
    }

    public void toggleElement(string element)
    {
        toggleElement((ElementIdentity)System.Enum.Parse(typeof(ElementIdentity), element, true));
    }

    public void toggleCardType(CardType cardType)
    {
        CardFilterObject filter = cardsView.filter;
        // no list exists
        if (filter.cardTypes == null)
        {
            List<CardType> types = new List<CardType>();
            types.Add(cardType);
            filter.cardTypes = types;
        }
        // list exists but doesn't have type
        else if (!filter.cardTypes.Contains(cardType))
        {
            filter.cardTypes.Add(cardType);
        }
        else
        {
            // type is in list
            filter.cardTypes.Remove(cardType);
        }
    }

    public void toggleTotalCost(int totalCost)
    {
        CardFilterObject filter = cardsView.filter;
        // no list exists
        if (filter.totalCosts == null)
        {
            List<int> costs = new List<int>();
            costs.Add(totalCost);
            filter.totalCosts = costs;
        }
        // list exists but doesn't have totalCost
        else if (!filter.totalCosts.Contains(totalCost))
        {
            filter.totalCosts.Add(totalCost);
        }
        else
        {
            // cost is in list already
            filter.totalCosts.Remove(totalCost);
        }
    }

    public void toggleElement(ElementIdentity element)
    {
        CardFilterObject filter = cardsView.filter;
        // no list exists
        if (filter.elements == null)
        {
            List<ElementIdentity> elements = new List<ElementIdentity>();
            elements.Add(element);
            filter.elements = elements;
        }
        else if (!filter.elements.Contains(element))
        {
            Debug.Log("Adding element");
            filter.elements.Add(element);
        }
        else
        {
            Debug.Log("Removing element");
            filter.elements.Remove(element);
        }
        foreach (ElementIdentity id in filter.elements)
        {
            Debug.Log(id);
        }
    }

    public void submit()
    {
        cardsView.updateDisplayedCards(cardsView.filter);
    }
}
