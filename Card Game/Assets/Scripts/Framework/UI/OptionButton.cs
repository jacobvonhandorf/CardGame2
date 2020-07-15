using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MyButton
{
    private int index;
    private string text;
    //private OptionBoxHandler handler;
    private OptionSelectBox optionSelectBox;

    private void OnMouseUpAsButton()
    {
        optionSelectBox.submit(index, text);
    }

    public void setUp(int index, string text, OptionSelectBox optionSelectBox)
    {
        this.index = index;
        this.text = text;
        this.optionSelectBox = optionSelectBox;

        textMesh.text = text;
    }
}
