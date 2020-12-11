using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewableCardPile : CardPile
{
    private void OnMouseDown()
    {
        Debug.Log("Card Pile clicked");
        CardPileViewer.MainViewer.SetupAndShow(cardList, "Viewing: " + PileName);
    }

    protected abstract string PileName { get; }
}
