using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPileViewer : MonoBehaviour
{
    public static CardPileViewer MainViewer { get; private set; }
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private bool isMainViewer = false;
    private CardScrollView scrollView;

    private void Awake()
    {
        scrollView = GetComponentInChildren<CardScrollView>();
        if (isMainViewer)
            MainViewer = this;
    }

    public void SetupAndShow(List<Card> cardList, string windowName)
    {
        foreach (Card c in cardList)
            scrollView.AddCard(c);

        headerText.text = windowName;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        scrollView.Clear();
    }
}
