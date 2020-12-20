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
    public OptionSelectBox optionBox;

    private void Start()
    {
        // setup test

        List<string> options = new List<string>()
        {
            "Option A",
            "Option B",
            "Option C",
        };
        testList.Add(delegate ()
        {
        });

        OptionSelectBox.CreateAndQueue(options, "Test test test test test test", null, delegate (int index, string option)
        {
            Debug.Log(index + " " + option);
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

