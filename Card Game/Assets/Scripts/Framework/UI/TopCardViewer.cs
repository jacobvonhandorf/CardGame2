using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCardViewer : MonoBehaviour
{
    [SerializeField] private FirstOrLast topCardLocation;
    [SerializeField] private CardPile source;
    [SerializeField] private CardViewer targetViewer;

    public void UpdateCard()
    {
        if (source.CardList.Count == 0)
            targetViewer.gameObject.SetActive(false);
        else
            if (topCardLocation == FirstOrLast.First)
                targetViewer.SetCard(source.CardList[0]);
            else
                targetViewer.SetCard(source.CardList[source.CardList.Count - 1]);
    }
}

public enum FirstOrLast
{
    First,
    Last
}