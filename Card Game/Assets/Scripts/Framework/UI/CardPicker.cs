using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPicker : MonoBehaviour, CanReceiveCardPick
{
    public static CardPicker pickerPrefab;

    private List<Card> selectedCards;
    private int minCards, maxCards;
    private CanReceivePickedCards cardListReceiver;
    private CardListHandler handler;
    [SerializeField] private MyButton confirmButton;
    [SerializeField] private CardPileViewer cardPileViewer;
    private bool finished = false;

    #region Command
    public static void CreateAndQueue(List<Card> pickableCards, int minCards, int maxCards, string headerText, Player owner, CardListHandler handler)
    {
        InformativeAnimationsQueue.instance.addAnimation(CreateCommand(pickableCards, minCards, maxCards, headerText, owner, handler));
    }
    // used for creating compound commands
    public static QueueableCommand CreateCommand(List<Card> pickableCards, int minCards, int maxCards, string headerText, Player owner, CardListHandler handler)
    {
        return new CardPickerCmd(pickableCards, minCards, maxCards, headerText, owner, handler);
    }
    private class CardPickerCmd : QueueableCommand
    {
        public override bool isFinished => picker.isFinished();
        public bool finished = false;
        private CardPicker picker;

        List<Card> pickableCards;
        int minCards;
        int maxCards;
        string headerText;
        CardListHandler handler;
        Player owner;

        public CardPickerCmd(List<Card> pickableCards, int minCards, int maxCards, string headerText, Player owner, CardListHandler handler)
        {
            this.pickableCards = pickableCards;
            this.minCards = minCards;
            this.maxCards = maxCards;
            this.headerText = headerText;
            this.handler = handler;
            this.owner = owner;
        }

        public override void execute()
        {
            if (owner != NetInterface.Get().getLocalPlayer())
                return;
            CardPicker cardPicker = Instantiate(GameManager.Get().cardPickerPrefab, new Vector3(0, 0, -1), Quaternion.identity);
            picker = cardPicker;
            cardPicker.setUp(pickableCards, handler, minCards, maxCards, headerText);
            GameManager.Get().setPopUpGlassActive(true);
        }
    }
    #endregion

    private void Awake()
    {
        selectedCards = new List<Card>();
    }

    public bool setUp(List<Card> pickableCards, CardListHandler handler, int minCards, int maxCards, string headerText)
    {
        if (pickableCards.Count < minCards)
            return false;
        headerText = getHeaderString(minCards, maxCards, headerText);

        this.handler = handler;
        this.minCards = minCards;
        this.maxCards = maxCards;
        // enable button if needed
        if (minCards == 0)
            confirmButton.gameObject.SetActive(true);
        else
            confirmButton.gameObject.SetActive(false);
        // setup individual card veiwers
        if (minCards == maxCards)
            cardPileViewer.setupAndShow(pickableCards, headerText, this);
        else if (maxCards < 999999)
            cardPileViewer.setupAndShow(pickableCards, headerText, this);
        else
            throw new NotImplementedException();
        return true;
    }

    public bool isFinished()
    {
        return finished;
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
        handler.Invoke(selectedCards);
        //cardListReceiver.receiveCardList(selectedCards);
        finished = true;
        //EffectsManager.Get().signalEffectFinished();
        GameManager.Get().setPopUpGlassActive(false);
        Destroy(gameObject);
    }

}
