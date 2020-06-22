using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPicker : MonoBehaviour, CanReceiveCardPick
{
    private List<Card> selectedCards;
    private int minCards, maxCards;
    private CanReceivePickedCards cardListReceiver;
    [SerializeField] private MyButton confirmButton;
    //[SerializeField] private GameObject pickableCardPrefab;
    [SerializeField] private CardPileViewer cardPileViewer;

    private void Start()
    {
        selectedCards = new List<Card>();
    }

    // return true if there are enough cards to meet min
    public bool setUp(List<Card> selectableCards, CanReceivePickedCards cardReceiver, int minCards, int maxCards, string headerText)
    {
        headerText = getHeaderString(minCards, maxCards, headerText);

        if (selectableCards.Count < minCards)
            return false;
        cardListReceiver = cardReceiver;
        this.minCards = minCards;
        this.maxCards = maxCards;
        // enable button if needed
        if (minCards == 0)
            confirmButton.gameObject.SetActive(true);
        else
            confirmButton.gameObject.SetActive(false);
        // setup individual card veiwers
        if (minCards == maxCards)
            cardPileViewer.setupAndShow(selectableCards, headerText, this);
        else if (maxCards < 999999)
            cardPileViewer.setupAndShow(selectableCards, headerText, this);
        else
            throw new NotImplementedException();
        return true;
    }

    private string getHeaderString(int minCards, int maxCards, string oldHeaderText)
    {
        string stringtoReturn = "";
        if (oldHeaderText != null)
            stringtoReturn += oldHeaderText + ": (";
        else
            stringtoReturn += "Select cards: (";

        if (minCards == maxCards)
            stringtoReturn += minCards + ")";
        else
            stringtoReturn += minCards + "-" + maxCards + ")";

        return stringtoReturn;
    }

    public void receiveCardPick(Card card)
    {
        selectedCards.Add(card);
        if (selectedCards.Count == maxCards)
            confirmPicks();
        else if (selectedCards.Count >= minCards)
            confirmButton.gameObject.SetActive(true);
    }

    public void removeCardPick(Card card)
    {
        selectedCards.Remove(card);
        if (selectedCards.Count < minCards)
            confirmButton.gameObject.SetActive(false);
    }

    // called when player is done selecting cards. Usually when they press a button or reach max number of cards
    public void confirmPicks()
    {
        cardListReceiver.receiveCardList(selectedCards);
        Destroy(gameObject);
        EffectsManager.Get().signalEffectFinished();
        GameManager.Get().setPopUpGlassActive(false);
    }
}
