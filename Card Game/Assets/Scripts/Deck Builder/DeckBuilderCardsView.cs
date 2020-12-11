using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderCardsView : MonoBehaviour
{
    private const string cardsPath = "All Cards/Cards Visible In Deck Builder";
    [SerializeField] private CardViewerForDeckBuilder cardViewerPrefab;
    [SerializeField] private SpriteScroller scroller;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private DeckBuilderDeck deck;
    private List<CardData> cardList; // list of cards being displayed on screen. Used for updating filter
    private List<CardData> allCards; // all cards even ones not being displayed on screen
    private List<CardViewerForDeckBuilder> cardViewers;
    public CardFilterObject filter;

    private Dictionary<int, Card> cardIdMap = new Dictionary<int, Card>();
    private Dictionary<int, CardViewerForDeckBuilder> cardIdToViewerMap = new Dictionary<int, CardViewerForDeckBuilder>();

    public static DeckBuilderCardsView instance;

    private void Awake()
    {
        cardViewers = new List<CardViewerForDeckBuilder>();
        allCards = ResourceManager.Get().GetAllCardDataVisibleInDeckBuilder();
        allCards.Sort();
        cardList = new List<CardData>();
        cardList.AddRange(allCards);
        setup(cardList);
        filter = new CardFilterObject();
        instance = this;
    }

    public float xOffset = -10f;
    public float xCoeff = 2.1f;
    public float yOffset = 1;
    public float yCoeff = -3f;
    public float scrollerCoeff = -3;
    public float scrollerOffset = -3;
    public void setup(List<CardData> cardList) // cards must be instantiated before calling this
    {
        int index = 0;
        foreach (CardData data in cardList)
        {
            int id = data.id;
            CardViewerForDeckBuilder cardViewer;
            if (index < cardViewers.Count)
            {
                cardViewer = cardViewers[index];
                cardViewer.gameObject.SetActive(true);
            }
            else
                cardViewer = Instantiate(cardViewerPrefab, contentTransform);
            // cardViewer.SetCard(id); needs to be redone
            cardViewer.deckBeingBuilt = deck;
            if (!cardIdToViewerMap.ContainsKey(id))
                cardIdToViewerMap.Add(id, cardViewer);
            Vector3 newPosition = new Vector3(xOffset + (index % 5) * xCoeff, yOffset + yCoeff * (index / 5), -1);
            cardViewer.transform.localPosition = newPosition;
            if (!cardViewers.Contains(cardViewer))
                cardViewers.Add(cardViewer);

            index++;
        }
        // turn off unused card viewers
        for (; index < cardViewers.Count; index++)
            cardViewers[index].gameObject.SetActive(false);

        scroller.maxY = ((index - 4) / 5) * scrollerCoeff + scrollerOffset;
        Debug.Log(scroller.minY);
        scroller.updateContentPosition(new Vector3(-999, -999, 0)); // move scroller to the top
    }

    public Card getCardById(int id)
    {
        return cardIdMap[id];
    }

    public void notifyAddCard(int cardId)
    {
        cardIdToViewerMap[cardId].incrementCountText();
    }
    public void notifyRemoveCard(int cardId)
    {
        cardIdToViewerMap[cardId].decrementCountText();
    }

    private List<CardData> filterCards(CardFilterObject filter)
    {

        List<CardData> filteredCardList = new List<CardData>();
        foreach (CardData data in allCards)
        {
            // elements
            if (filter.elements != null && filter.elements.Count > 0)
                if (!filter.elements.Contains(data.elementalIdentity))
                    continue;
            // total cost
            if (filter.totalCosts != null && filter.totalCosts.Count > 0)
                if (!filter.totalCosts.Contains(data.TotalCost))
                    continue;
            // card types
            if (filter.cardTypes != null && filter.cardTypes.Count > 0)
                if (!filter.cardTypes.Contains(data.CardType))
                    continue;
            // tags
            if (filter.tags != null && filter.tags.Count > 0)
            {
                bool hasTag = false;
                foreach (Card.Tag tag in filter.tags)
                    if (data.tags.Contains(tag))
                    {
                        hasTag = true;
                        break;
                    }
                if (!hasTag)
                    continue;
            }
            // keywords
            if (filter.keywords != null && filter.keywords.Count > 0)
            {
                bool hasKeyword = false;
                foreach (Keyword keyword in filter.keywords)
                    if (data.keywords.Contains(keyword))
                    {
                        hasKeyword = true;
                        break;
                    }
                if (!hasKeyword)
                    continue;
            }
            // name
            if (filter.nameTextSearch != null && filter.nameTextSearch != "")
            {
                if (!data.cardName.ToLower().Contains(filter.nameTextSearch.ToLower()))
                    continue;
            }
            // description
            if (filter.descriptionTextSearch != null && filter.descriptionTextSearch != "")
            {
                if (!data.effectText.ToLower().Contains(filter.descriptionTextSearch.ToLower()))
                    continue;
            }
            filteredCardList.Add(data);
        }


        return filteredCardList;
    }

    public void updateDisplayedCards(CardFilterObject filter)
    {
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        cardList = filterCards(filter);
        setup(cardList);
        long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Debug.Log("Filtered in " + (endTime - startTime) + "ms");
    }

    public void addSearchText(string textToFilter)
    {
        filter.nameTextSearch = textToFilter;
        updateDisplayedCards(filter);
    }
}

public class CardFilterObject
{
    public List<Card.ElementIdentity> elements;
    public List<int> totalCosts;
    public List<Card.CardType> cardTypes;
    public List<Card.Tag> tags;
    public List<Keyword> keywords;
    public string nameTextSearch;
    public string descriptionTextSearch;
}
