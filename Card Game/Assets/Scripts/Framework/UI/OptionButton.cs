using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    private int index;
    private string text;
    private OptionSelectBox optionSelectBox;

    public void SetUp(int index, string text, OptionSelectBox optionSelectBox)
    {
        this.index = index;
        this.text = text;
        this.optionSelectBox = optionSelectBox;

        GetComponent<Button>().onClick.AddListener(OnClick);
        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void OnClick()
    {
        optionSelectBox.Submit(index, text);
    }
}
