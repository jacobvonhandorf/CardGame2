using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckBuilderSearchBox : MonoBehaviour
{
    [SerializeField] private DeckBuilderCardsView cardsView;
    [SerializeField] TMP_InputField descriptionInput;
    [SerializeField] TMP_InputField nameInput;

    public void onSearchEdit()
    {
        CardFilterObject filter = cardsView.filter;
        filter.nameTextSearch = nameInput.text;
        cardsView.updateDisplayedCards(filter);
    }

    public void onDescriptionEdit()
    {
        CardFilterObject filter = cardsView.filter;
        filter.descriptionTextSearch = descriptionInput.text;
        cardsView.updateDisplayedCards(filter);
    }
}
