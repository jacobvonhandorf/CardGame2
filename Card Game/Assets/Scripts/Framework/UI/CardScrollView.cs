using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CardScrollView : MonoBehaviour
{
    public UnityEvent<CardViewer> OnCardAdded { get; } = new CardViewerEvent();

    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardViewer cardViewerPrefab;
    [SerializeField] private int numInitialCardPreviews = 0;
    [SerializeField] private float viewerScaleRatio;

    public List<CardViewer> ActiveViewers => loadedViewers.FindAll(cv => cv.gameObject.activeInHierarchy);
    private List<CardViewer> loadedViewers = new List<CardViewer>();
    private int numActiveViewers = 0;

    private void Start()
    {
        for (int i = 0; i < numInitialCardPreviews; i++)
        {
            AddNewCardViewer().gameObject.SetActive(false);
        }
        // testing        
    }

    public void AddCards(List<ICanBeCardViewed> cards)
    {
        foreach (ICanBeCardViewed c in cards)
            AddCard(c);
    }

    public void AddCard(ICanBeCardViewed card)
    {
        if (numActiveViewers < loadedViewers.Count)
        {
            // use existing viewer
            loadedViewers[numActiveViewers].gameObject.SetActive(true);
            loadedViewers[numActiveViewers].SetCard(card);
            numActiveViewers++;
        }
        else
        {
            // create new viewer
            AddNewCardViewer().SetCard(card);
            numActiveViewers++;
        }
    }

    public void Clear()
    {
        foreach (CardViewer c in loadedViewers)
            c.gameObject.SetActive(false);
        numActiveViewers = 0;
    }

    private CardViewer AddNewCardViewer()
    {
        CardViewer cv = Instantiate(cardViewerPrefab, cardContainer);
        cv.transform.localScale = cv.transform.localScale * viewerScaleRatio;
        loadedViewers.Add(cv);
        return cv;
    }
}
