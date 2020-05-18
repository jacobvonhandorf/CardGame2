using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewerForDeckBuilder : CardViewer
{
    public DeckBuilderDeck deckBeingBuilt;

    private void OnMouseUpAsButton()
    {
        deckBeingBuilt.addCard(sourceCard);
    }
}
