using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private TextMeshPro descriptionText;

    public void setup(string headerText, string descriptionText)
    {
        this.headerText.text = headerText;
        this.descriptionText.text = descriptionText;
    }
}
