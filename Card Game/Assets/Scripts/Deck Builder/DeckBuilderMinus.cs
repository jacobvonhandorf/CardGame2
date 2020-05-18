using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderMinus : MonoBehaviour
{
    public DeckBuilderDeck deck;
    public Card card;

    private void OnMouseUpAsButton()
    {
        deck.removeCard(card);
    }
}
