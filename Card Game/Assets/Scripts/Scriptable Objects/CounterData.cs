using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "New Counter Type", menuName = "Counter Data")]
public class CounterData : ScriptableObject
{
    [SerializeField] private Color borderColor = new Color(0, 0, 0, 1);
    public Color BorderColor { get { return borderColor; } }
    [SerializeField] private Color fillColor = new Color(1, 1, 1, 1);
    public Color FillColor { get { return fillColor; } }
    [SerializeField] private string toolTipMessage;
    public string ToolTip { get { return toolTipMessage; } }
    [SerializeField] public CounterType type;
    public CounterType CType { get { return type; } }
}
