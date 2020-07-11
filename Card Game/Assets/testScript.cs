using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour, CanReceivePickedCards
{
    public CardPicker prefab;

    private void Start()
    {
        // setup test
        CardPicker picker = Instantiate(prefab);
        picker.setUp(getTestCardList(), this, 0, 5, "Testing");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            doTestOnKeyPress();
    }

    private void doTestOnKeyPress()
    {

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

    private Card getTestCard()
    {
        return ResourceManager.Get().instantiateCardById(ManaWell.CARD_ID);
    }

    public void receiveCardList(List<Card> cardList)
    {
        throw new NotImplementedException();
    }
}

