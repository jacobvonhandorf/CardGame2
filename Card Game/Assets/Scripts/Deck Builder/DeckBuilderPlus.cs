using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderPlus : MonoBehaviour
{
    public DeckBuilderDeck deck;
    public int cardId;

    private void OnMouseUpAsButton()
    {
        deck.addCard(cardId);
    }
}
