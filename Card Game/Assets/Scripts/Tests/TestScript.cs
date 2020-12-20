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

    private void Start()
    {
        // setup test

        CreatureCard creatureCard = GetTestCreatureCard();
        StructureCard structureCard = GetTestStructureCard();

        testList.Add(delegate ()
        {
            creatureCard.Creature.CreateOnTile(tile1);
        });
        testList.Add(delegate ()
        {
            creatureCard.Creature.Move(tile2);
        });
        testList.Add(delegate ()
        {
            creatureCard.Creature.SyncMove(tile3);
        });
        testList.Add(delegate ()
        {
            structureCard.Structure.CreateOnTile(tile1);
        });
        testList.Add(delegate ()
        {
            GetTestStructureCard().Structure.SyncCreateOnTile(tile2);
        });
    }

    int testNum = 0;
    List<Action> testList = new List<Action>();
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

    private SpellCard GetTestSpellCard() => ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity) as SpellCard;
    private StructureCard GetTestStructureCard() => ResourceManager.Get().InstantiateCardById(CardIds.Market) as StructureCard;
    private CreatureCard GetTestCreatureCard() => ResourceManager.Get().InstantiateCardById(CardIds.Mercenary) as CreatureCard;
}

