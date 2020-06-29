using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadDeckPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DeckBuilderDeck deckBuilderDeck;

    private string previousDeck1 = null;
    private string previousDeck2 = null;

    private void Start()
    {
        setup();
    }

    public void submit()
    {
        // 0 is default value
        if (dropdown.value != 0)
        {
            //gameObject.SetActive(false);
            //deckBuilderDeck.load(dropdown.options[dropdown.value].text);
            deckBuilderDeck.loadWithSavedChangesCheck(dropdown.options[dropdown.value].text);
            previousDeck2 = previousDeck1;
            previousDeck1 = dropdown.options[dropdown.value].text;
        }
    }

    public void setup()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>() { "" });
        dropdown.AddOptions(DeckUtilities.getAllDeckNames());
        gameObject.SetActive(true);
    }

    public void setToValue(string value)
    {
        int index = 0;
        foreach (TMP_Dropdown.OptionData option in dropdown.options)
        {
            if (option.text == value)
            {
                dropdown.SetValueWithoutNotify(index);
                break;
            }
            index++;
        }

    }

    public void setToPreviousValue()
    {
        setToValue(previousDeck2);
    }
}
