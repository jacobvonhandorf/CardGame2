using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class IntChangeListener : StatChangeListener
{
    public UnityEvent<int> ValueChanged = new IntEvent();

    public int Value { get; private set; }
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    protected override void OnValueUpdated(object value)
    {
        Value = (int)value;
        ValueChanged.Invoke(Value);
        if (textMesh != null)
            textMesh.text = (int)value + "";
    }
}
