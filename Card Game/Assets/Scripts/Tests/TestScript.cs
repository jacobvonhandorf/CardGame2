using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    public CardData testSpellData;
    public CardData testCreatureData;
    public CardData testStructureData;
    public Card testCard;

    private void Start()
    {
        // setup test
        
        //testCard = getTestStructureCard();
    }

    int testNum = 0;
    List<Delegate> testList;
    Card c = null;
    private void DoTestOnKeyPress()
    {
        /*
        // setting card viewer to cardData for all card types
        if (testNum == 0)
        {
            FindObjectOfType<CardViewer>().SetCard(testSpellData);
        }
        else if (testNum == 1)
        {
            FindObjectOfType<CardViewer>().SetCard(testCreatureData);
        }
        else if (testNum == 2)
        {
            FindObjectOfType<CardViewer>().SetCard(testStructureData);
        }
        else if (testNum == 3)
        {
            FindObjectOfType<CardViewer>().SetCard(getTestSpellCard());
        }
        else if (testNum == 4)
        {
            FindObjectOfType<CardViewer>().SetCard(getTestCreatureCard());
        }
        else if (testNum == 5)
        {
            c = getTestStructureCard();
            FindObjectOfType<CardViewer>().SetCard(c);
        }
        else if (testNum == 6)
        {
            (c as StructureCard).structure.Health = 12;
        }
        // setting card to viewer for each card type and making updates to the original card

        testNum++;
        // test code here
        //(testCard as StructureCard).structure.Health -= 1;
        */
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("performing test");
            DoTestOnKeyPress();
        }
    }

    private List<Card> getTestCardList()
    {
        List<Card> cardList = new List<Card>();
        for (int i = 0; i < 50; i++)
        {
            cardList.Add(getTestSpellCard());
        }
        return cardList;
    }

    private Card getTestSpellCard() => ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity);
    private Card getTestStructureCard() => ResourceManager.Get().InstantiateCardById(CardIds.Market);
    private Card getTestCreatureCard() => ResourceManager.Get().InstantiateCardById(CardIds.Mercenary);
    //private Card getTestCard() => ResourceManager.Get().instantiateCardById(68);
}

