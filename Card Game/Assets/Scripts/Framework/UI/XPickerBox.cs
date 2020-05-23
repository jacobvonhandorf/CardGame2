using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XPickerBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro xValueText;
    [SerializeField] private TextMeshPro headerTextMesh;
    private int xValue;
    private CanRecieveXPick receiver;

    private int minValue;
    private int maxValue;

    private void Awake()
    {
        xValue = 0;
        minValue = 0;
        maxValue = 9999;
    }

    public void addX(int value)
    {
        xValue += value;
        if (xValue < minValue)
            xValue = minValue;
        else if (xValue > maxValue)
            xValue = maxValue;
        xValueText.text = xValue + "";
    }

    public void submit()
    {
        Destroy(gameObject);
        receiver.receiveXPick(xValue);
        EffectsManager.Get().signalEffectFinished();
    }

    public void setUp(CanRecieveXPick receiver, int minValue, int maxValue, string headerText)
    {
        this.receiver = receiver;
        this.minValue = minValue;
        this.maxValue = maxValue;

        if (headerText == null || headerText.Equals(""))
            headerText = "Select a value";
        headerTextMesh.text = headerText;
    }
}

public interface CanRecieveXPick
{
    void receiveXPick(int value);
}
