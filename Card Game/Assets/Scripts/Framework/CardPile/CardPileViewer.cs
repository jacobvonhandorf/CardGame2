using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPileViewer : MonoBehaviour
{
    private const float xScale = .4f;
    private const float yScale = .4f;
    private const float xPosOffset = -3.15f;
    private const float yPosOffset = 6f;
    private const float xPosCoeff = 1.9f;
    private const float yPosCoeff = -2.5f;
    private const int cardsPerRow = 5;

    [SerializeField] private TextMeshPro headerText;
    [SerializeField] protected GameObject previewPrefab; // use if more card previews need to be added
    [SerializeField] private int numInitialCardPreviews = 0;
    [SerializeField] private Transform scrollObject;
    [SerializeField] private SpriteScroller scroller;

    private List<CardViewerForPiles> cardPreviews;

    private void Awake()
    {
        if (cardPreviews == null)
            cardPreviews = new List<CardViewerForPiles>();
        // set up a pool of card viewers for later
        for (int i = 0; i < numInitialCardPreviews; i++)
        {
            addNewCardPreview().gameObject.SetActive(false);
        }
    }

    protected CardViewerForPiles addNewCardPreview()
    {
        int index = cardPreviews.Count;

        Vector3 newScale = new Vector3();
        newScale.x = xScale;
        newScale.y = yScale;

        Vector3 newPosition = new Vector3();
        newPosition.x = xPosOffset + (index % cardsPerRow) * xPosCoeff;
        newPosition.y = yPosOffset + (index / cardsPerRow) * yPosCoeff;
        newPosition.z = -1.1f; // -1.1 allows for them to be hovered/clicked

        CardViewerForPiles newCardViewer = Instantiate(previewPrefab, transform).GetComponent<CardViewerForPiles>();
        newCardViewer.transform.localScale = newScale;
        newCardViewer.transform.localPosition = newPosition;
        newCardViewer.transform.parent = scrollObject;
        newCardViewer.gameObject.SetActive(false);
        cardPreviews.Add(newCardViewer);

        return newCardViewer;
    }

    public float scrollerOffset = -3;
    public void setupAndShow(List<Card> cardList, string windowName)
    {
        if (cardPreviews == null)
            cardPreviews = new List<CardViewerForPiles>();
        // instantiate new a card previews as needed
        while (cardList.Count > cardPreviews.Count)
        {
            addNewCardPreview();
        }

        int index = 0;
        foreach (Card c in cardList)
        {
            cardPreviews[index].SetCard(c);
            index++;
        }

        headerText.text = windowName;
        scroller.maxY = -(index / cardsPerRow) * yPosCoeff + scrollerOffset;
        scroller.setMinAndMax(scroller.minY, scroller.maxY);
        scroller.updateContentPosition(new Vector3(-999, -999, 0)); // move scroller to the top
        gameObject.SetActive(true);
    }

    // setup method for card picker
    public void setupAndShow(List<Card> cardList, string windowName, CanReceiveCardPick receiver)
    {
        // do normal setup
        setupAndShow(cardList, windowName);

        // setup card viewers
        foreach (CardViewerForPiles c in cardPreviews)
        {
            PickableCard pickableCard = c.GetComponent<PickableCard>();
            pickableCard.setUp(receiver, c.SourceCard);
        }
    }

    private Ray ray;
    private RaycastHit hit;
    // check to see if mouse is over
    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer("Card Viewer")))
        {
            GameManager.Get().getCardViewer().SetCard((hit.transform.gameObject.GetComponent<CardViewer>()).SourceCard);
        }
    }


    public void close()
    {
        gameObject.SetActive(false);

        // hide all the card viewers because they should only show up if they are populated with new cards
        foreach(CardViewerForPiles cardViewer in cardPreviews)
        {
            cardViewer.gameObject.SetActive(false);
        }
    }
}
