using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class IntChangeListener : StatChangeListener
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    protected override void onValueUpdated(object value)
    {
        if (textMesh != null)
            textMesh.text = (int)value + "";
    }
}
