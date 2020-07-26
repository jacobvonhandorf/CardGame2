using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderMinus : MonoBehaviour
{
    public DeckBuilderDeck deck;
    public int cardId;

    private void OnMouseUpAsButton()
    {
        deck.removeCard(cardId);
    }
}
