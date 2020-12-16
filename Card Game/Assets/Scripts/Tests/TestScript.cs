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
    public Tile tile1;
    public Tile tile2;
    public Tile tile3;
    public Player testPlayer;

    Creature c = null;

    private void Start()
    {
        // setup test

        //testCard = getTestStructureCard();
        c = (ResourceManager.Get().InstantiateCardById(CardIds.Engineer) as CreatureCard).Creature;
        c.SourceCard.Owner = testPlayer;
        c.Controller = testPlayer;
        testList.Add(delegate ()
        {

        });

        IScriptCard sCard = c.SourceCard;
    }

    int testNum = 0;
    List<Action> testList;
    private void DoTestOnKeyPress()
    {
        testList[testNum].Invoke();
        testNum++;
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
            cardList.Add(GetTestSpellCard());
        }
        return cardList;
    }

    private Card GetTestSpellCard() => ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity);
    private Card GetTestStructureCard() => ResourceManager.Get().InstantiateCardById(CardIds.Market);
    private Card GetTestCreatureCard() => ResourceManager.Get().InstantiateCardById(CardIds.Mercenary);
}

