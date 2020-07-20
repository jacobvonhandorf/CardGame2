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
    public CardPicker prefab;

    private void Start()
    {
        // setup test
        GameObject go = Instantiate(new GameObject("Animations Queue"));
        go.AddComponent<InformativeAnimationsQueue>();

        XPickerBox.CreateAndQueue(0, 5, "Test", null, delegate (int x)
        {
            Debug.Log("X = " + x);
        });
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

