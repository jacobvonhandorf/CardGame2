using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckPicker : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown deckSelectDropdown;
    // Start is called before the first frame update
    void Start()
    {
        deckSelectDropdown.AddOptions(DeckUtilities.getAllDeckNames());
    }
}
