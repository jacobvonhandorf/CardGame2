using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadDeckPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DeckBuilderDeck deckBuilderDeck;

    public void submit()
    {
        // 0 is default value
        if (dropdown.value != 0)
        {
            gameObject.SetActive(false);
            deckBuilderDeck.load(dropdown.options[dropdown.value].text);
        }
    }

    public void setup()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>() { "" });
        dropdown.AddOptions(DeckUtilities.getAllDeckNames());
        gameObject.SetActive(true);
    }
}
