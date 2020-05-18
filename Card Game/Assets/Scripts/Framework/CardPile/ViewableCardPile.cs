using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewableCardPile : CardPile
{
    public CardPileViewer cardPileViewer;

    private void OnMouseDown()
    {
        Debug.Log("Card Pile clicked");
        if (cardList == null)
        {
            cardList = new List<Card>();
        }
        cardPileViewer.setupAndShow(cardList, "Viewing: " + getPileName());
    }

    protected abstract string getPileName();
}
