﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XPickerBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro xValueText;
    [SerializeField] private TextMeshPro headerTextMesh;
    private int xValue = 0;
    private int minValue = 0;
    private int maxValue = 9999;
    private XValueHandler handler;

    private bool finished = false;

    #region Command
    public static IQueueableCommand CreateAsCommand(int minValue, int maxValue, string headerText, Player owner, XValueHandler handler)
    {
        return new XPickerCmd(minValue, maxValue, headerText, owner, handler);
    }
    public static void CreateAndQueue(int minValue, int maxValue, string headerText, Player owner, XValueHandler handler)
    {
        InformativeAnimationsQueue.Instance.AddAnimation(CreateAsCommand(minValue, maxValue, headerText, owner, handler));
    }
    private class XPickerCmd : IQueueableCommand
    {
        XPickerBox xPicker;
        int minValue;
        int maxValue;
        string headerText;
        Player owner;
        XValueHandler handler;

        bool forceFinished = false;

        public XPickerCmd(int minValue, int maxValue, string headerText, Player owner, XValueHandler handler)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.headerText = headerText;
            this.handler = handler;
        }

        public bool IsFinished => forceFinished || xPicker.finished;

        public void Execute()
        {
            if (owner != null && NetInterface.Get().localPlayer != owner)
            {
                forceFinished = true;
                return;
            }
            xPicker = Instantiate(PrefabHolder.Instance.xPickerPrefab);
            xPicker.setUp(minValue, maxValue, headerText, handler);
        }
    }
    #endregion

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
        handler.Invoke(xValue);
        finished = true;
    }

    public void setUp(int minValue, int maxValue, string headerText, XValueHandler handler)
    {
        this.handler = handler;
        this.minValue = minValue;
        this.maxValue = maxValue;

        if (headerText == null || headerText.Equals(""))
            headerText = "Select a value";
        headerTextMesh.text = headerText;

        if (minValue > 0)
        {
            xValue = minValue;
            xValueText.text = "" + xValue;
        }
    }
}
