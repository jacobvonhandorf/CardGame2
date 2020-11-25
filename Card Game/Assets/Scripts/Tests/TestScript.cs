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
    public CardData testCardData;
    //public Card testCard;

    private void Start()
    {
        // setup test
        //testCard = getTestCard();
        CardBuilder.Instance.BuildFromCardData(testCardData);
    }

    private void doTestOnKeyPress()
    {
        // test code here
        //testCard.GetComponent<CardToPermanentConverter>().doConversion(new Vector3(0, 0));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("performing test");
            doTestOnKeyPress();
        }
    }

    private List<Card> getTestCardList()
    {
        List<Card> cardList = new List<Card>();
        for (int i = 0; i < 50; i++)
        {
            cardList.Add(getTestCard());
        }
        return cardList;
    }

    private Card getTestCard() => ResourceManager.Get().instantiateCardById((int)CardIds.RingOfEternity);
    //private Card getTestCard() => ResourceManager.Get().instantiateCardById(68);
}

