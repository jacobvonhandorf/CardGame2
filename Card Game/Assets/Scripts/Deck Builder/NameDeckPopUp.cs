using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NameDeckPopUp : MonoBehaviour
{
    [SerializeField] private GameObject glassBackground;
    [SerializeField] private InputField deckNameInputField;
    [SerializeField] private DeckBuilderDeck deckBuilderDeck;

    public void setUp()
    {
        gameObject.SetActive(true);
        glassBackground.SetActive(true);
        EventSystem.current.SetSelectedGameObject(deckNameInputField.gameObject);
    }


    private void OnGUI()
    {
        if (deckNameInputField.isFocused && deckNameInputField.text != "" && Input.GetKey(KeyCode.Return))
            StartCoroutine(submit());
    }

    private IEnumerator submit()
    {
        yield return null;
        string deckName = deckNameInputField.text;
        Debug.Log("Saving deck: " + deckName);
        deckBuilderDeck.save(deckName);
        gameObject.SetActive(false);
        glassBackground.SetActive(false);
    }

}
