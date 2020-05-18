using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MyButton
{
    private int index;
    private string text;
    private OptionBoxHandler handler;
    private OptionSelectBox optionSelectBox;

    private void OnMouseUpAsButton()
    {
        handler.receiveOptionBoxSelection(index, text);
        optionSelectBox.close();
    }

    public void setUp(int index, string text, OptionBoxHandler handler, OptionSelectBox optionSelectBox)
    {
        this.index = index;
        this.text = text;
        this.handler = handler;
        this.optionSelectBox = optionSelectBox;

        textMesh.text = text;
    }
}
