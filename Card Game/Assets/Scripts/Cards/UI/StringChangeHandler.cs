using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StringChangeHandler : StatChangeListener
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    protected override void onValueUpdated(object value)
    {
        textMesh.text = (string)value + "";
    }
}
