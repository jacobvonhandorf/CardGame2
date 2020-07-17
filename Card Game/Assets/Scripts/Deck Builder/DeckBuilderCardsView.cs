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
    private List<Card> cardList; // list of cards being displayed on screen. Used for updating filter
    private List<Card> allCards; // all cards even ones not being displayed on screen
    private List<CardViewerForDeckBuilder> cardViewers;
    public CardFilterObject filter;

    private Dictionary<int, Card> cardIdMap = new Dictionary<int, Card>();
    private Dictionary<Card, CardViewerForDeckBuilder> cardToViewerMap = new Dictionary<Card, CardViewerForDeckBuilder>();

    public static DeckBuilderCardsView instance;

    private void Awake()
    {
        // get a list of all cards that need to be displayed
        //getAllCardsFromResources();
        cardViewers = new List<CardViewerForDeckBuilder>();
        allCards = getAllCardsFromResources();
        allCards.Sort(new CardComparator());
        cardList = new List<Card>();
        cardList.AddRange(allCards);
        setup(allCards);
        filter = new CardFilterObject();
        instance = this;
    }

    public float xOffset = -10f;
    public float xCoeff = 2.1f;
    public float yOffset = 1;
    public float yCoeff = -3f;
    public float scrollerCoeff = -3;
    public float scrollerOffset = -3;
    public void setup(List<Card> cardList) // cards must be instantiated before calling this
    {
        int index = 0;
        foreach (Card c in cardList)
        {
            CardViewerForDeckBuilder cardViewer;
            if (index < cardViewers.Count)
            {
                cardViewer = cardViewers[index];
                cardViewer.gameObject.SetActive(true);
            }
            else
                cardViewer = Instantiate(cardViewerPrefab, contentTransform);
            cardViewer.setCard(c);
            cardViewer.deckBeingBuilt = deck;
            if (!cardToViewerMap.ContainsKey(c))
                cardToViewerMap.Add(c, cardViewer);
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

    private List<Card> getAllCardsFromResources()
    {
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        List<Card> returnList = new List<Card>();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>(cardsPath);
        foreach (GameObject prefab in loadedPrefabs)
        {
            GameObject newGameObject = Instantiate(prefab, cardContainer);
            Card newCard = newGameObject.GetComponentInChildren<Card>();
            if (newCard == null)
            {
                Debug.LogError(newGameObject + " had no Card component");
                continue;
            }
            cardIdMap.Add(newCard.cardId, newCard);
            newCard.removeGraphicsAndCollidersFromScene();
            returnList.Add(newCard);
        }
        long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Debug.Log("Loaded all cards in " + (endTime - startTime) + " ms");

        return returnList;
    }

    public Card getCardById(int id)
    {
        return cardIdMap[id];
    }

    public void notifyAddCard(Card card)
    {
        cardToViewerMap[card].incrementCountText();
    }
    public void notifyRemoveCard(Card card)
    {
        cardToViewerMap[card].decrementCountText();
    }

    private List<Card> filterCards(CardFilterObject filter)
    {

        List<Card> filteredCardList = new List<Card>();
        foreach (Card c in allCards)
        {
            // elements
            if (filter.elements != null && filter.elements.Count > 0)
                if (!filter.elements.Contains(c.getElementIdentity()))
                    continue;
            // total cost
            if (filter.totalCosts != null && filter.totalCosts.Count > 0)
                if (!filter.totalCosts.Contains(c.getTotalCost()))
                    continue;
            // card types
            if (filter.cardTypes != null && filter.cardTypes.Count > 0)
                if (!filter.cardTypes.Contains(c.getCardType()))
                    continue;
            // tags
            if (filter.tags != null && filter.tags.Count > 0)
            {
                bool hasTag = false;
                foreach (Card.Tag tag in filter.tags)
                    if (c.hasTag(tag))
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
                    if (c.hasKeyword(keyword))
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
                if (!c.getCardName().ToLower().Contains(filter.nameTextSearch.ToLower()))
                    continue;
            }
            // description
            if (filter.descriptionTextSearch != null && filter.descriptionTextSearch != "")
            {
                if (!c.getEffectText().ToLower().Contains(filter.descriptionTextSearch.ToLower()))
                    continue;
            }
            filteredCardList.Add(c);
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
