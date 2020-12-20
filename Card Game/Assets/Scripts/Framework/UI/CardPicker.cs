using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CardPicker : MonoBehaviour
{
    public static CardPicker pickerPrefab;

    private List<ICanBeCardViewed> selectedCards = new List<ICanBeCardViewed>();
    private int minCards, maxCards;
    private Action<List<Card>> handler;
    [SerializeField] private Button confirmButton;
    [SerializeField] private CardScrollView scrollView;
    [SerializeField] private TextMeshProUGUI headerTextMesh;
    [SerializeField] private Vector2 outlineSize;
    [SerializeField] private Color outlineColor;
    public bool Finished { get; private set; }

    #region Command
    public static void CreateAndQueue(List<ICanBeCardViewed> pickableCards, int minCards, int maxCards, string headerMessage, Player owner, Action<List<Card>> handler)
        => InformativeAnimationsQueue.Instance.AddAnimation(CreateCommand(pickableCards, minCards, maxCards, headerMessage, owner, handler));
    public static void CreateAndQueue(List<Card> pickableCards, int minCards, int maxCards, string headerText, Player owner, Action<List<Card>> handler)
        => CreateAndQueue(pickableCards.Cast<ICanBeCardViewed>().ToList(), minCards, maxCards, headerText, owner, handler);
    public static IQueueableCommand CreateCommand(List<Card> pickableCards, int minCards, int maxCards, string headerMessage, Player owner, Action<List<Card>> handler)
        => CreateCommand(pickableCards.Cast<ICanBeCardViewed>().ToList(), minCards, maxCards, headerMessage, owner, handler);
    public static IQueueableCommand CreateCommand(List<ICanBeCardViewed> pickableCards, int minCards, int maxCards, string headerText, Player owner, Action<List<Card>> handler)
        => new CardPickerCmd(pickableCards, minCards, maxCards, headerText, owner, handler);

    public static void CreateAndQueue(IReadOnlyList<ICanBeCardViewed> pickableCards, int minCards, int maxCards, string headerMessage, Player owner, Action<List<Card>> handler)
        => InformativeAnimationsQueue.Instance.AddAnimation(CreateCommand(pickableCards, minCards, maxCards, headerMessage, owner, handler));
    public static IQueueableCommand CreateCommand(IReadOnlyList<ICanBeCardViewed> pickableCards, int minCards, int maxCards, string headerText, Player owner, Action<List<Card>> handler)
        => new CardPickerCmd(pickableCards, minCards, maxCards, headerText, owner, handler);

    private class CardPickerCmd : IQueueableCommand
    {
        public bool IsFinished => picker.Finished || forceFinished;
        private CardPicker picker;
        private bool forceFinished = false;

        IReadOnlyList<ICanBeCardViewed> pickableCards;
        int minCards;
        int maxCards;
        string headerText;
        Action<List<Card>> handler;
        Player owner;

        public CardPickerCmd(IReadOnlyList<ICanBeCardViewed> pickableCards, int minCards, int maxCards, string headerText, Player owner, Action<List<Card>> handler)
        {
            this.pickableCards = pickableCards;
            this.minCards = minCards;
            this.maxCards = maxCards;
            this.headerText = headerText;
            this.handler = handler;
            this.owner = owner;
        }

        public void Execute()
        {
            if (owner != NetInterface.Get().localPlayer)
            {
                forceFinished = true;
                return;
            }
            CardPicker cardPicker = Instantiate(PrefabHolder.Instance.cardPicker, MainCanvas.Instance.transform);
            picker = cardPicker;
            cardPicker.SetUp(pickableCards, minCards, maxCards, headerText, handler);
            UIEvents.EnableUIBlocker.Invoke();
        }
    }
    #endregion

    public bool SetUp(IReadOnlyList<ICanBeCardViewed> pickableCards, int minCards, int maxCards, string headerMessage, Action<List<Card>> handler)
    {
        if (pickableCards.Count < minCards)
            return false;
        UIEvents.EnableUIBlocker.Invoke();
        headerTextMesh.text = GetHeaderString(minCards, maxCards, headerMessage);

        this.handler = handler;
        this.minCards = minCards;
        this.maxCards = maxCards;
        // enable button if needed
        if (minCards == 0)
            confirmButton.gameObject.SetActive(true);
        else
            confirmButton.gameObject.SetActive(false);
        // setup individual card veiwers
        foreach (ICanBeCardViewed c in pickableCards)
            scrollView.AddCard(c);
        foreach (CardViewer c in scrollView.ActiveViewers)
            c.E_OnClicked.AddListener(ToggleCardPicked);
        return true;
    }

    private string GetHeaderString(int minCards, int maxCards, string headerMessage)
    {
        string stringtoReturn = "";
        if (headerMessage != null)
            stringtoReturn += headerMessage + ": (";
        else
            stringtoReturn += "Select cards: (";

        if (minCards == maxCards)
            stringtoReturn += minCards + ")";
        else
            stringtoReturn += minCards + "-" + maxCards + ")";

        return stringtoReturn;
    }

    public void ToggleCardPicked(CardViewer viewer)
    {
        if (selectedCards.Contains(viewer.CurrentlyDisplayed))
            DeselectViewer(viewer);
        else
            SelectViewer(viewer);
    }

    private void SelectViewer(CardViewer viewer)
    {
        selectedCards.Add(viewer.CurrentlyDisplayed);
        if (selectedCards.Count == maxCards)
            ConfirmPicks();
        else if (selectedCards.Count >= minCards)
            confirmButton.gameObject.SetActive(true);
        GameObject cardVis = viewer.GetComponentInChildren<StatChangePropogator>().gameObject;
        cardVis.gameObject.AddComponent<Image>();
        Outline outline = cardVis.gameObject.AddComponent<Outline>();
        outline.effectDistance = outlineSize;
        outline.effectColor = outlineColor;
    }

    private void DeselectViewer(CardViewer viewer)
    {
        selectedCards.Remove(viewer.CurrentlyDisplayed);
        if (selectedCards.Count < minCards)
            confirmButton.gameObject.SetActive(false);

        GameObject cardVis = viewer.GetComponentInChildren<StatChangePropogator>().gameObject;
        Destroy(cardVis.GetComponent<Outline>());
        Destroy(cardVis.GetComponent<Image>());
    }

    public void ConfirmPicks()
    {
        List<Card> confirmedPicks = new List<Card>();
        foreach (ICanBeCardViewed card in selectedCards)
            confirmedPicks.Add(card.AsCard);
        handler.Invoke(confirmedPicks);
        Finished = true;
        UIEvents.DisableUIBlocker.Invoke();
        Destroy(gameObject);
    }
}
