﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    public CardPicker prefab;

    private void Start()
    {
        // setup test
    }

    private void doTestOnKeyPress()
    {
        // test code here
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            doTestOnKeyPress();
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
}

