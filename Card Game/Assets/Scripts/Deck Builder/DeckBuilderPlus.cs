using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderPlus : MonoBehaviour
{
    public DeckBuilderDeck deck;
    public Card card;

    private void OnMouseUpAsButton()
    {
        deck.addCard(card);
    }
}
