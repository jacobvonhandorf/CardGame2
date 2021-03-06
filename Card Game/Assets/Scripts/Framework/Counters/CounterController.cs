﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterController : MonoBehaviour
{
    private const float DISPLAY_Y_OFFSET = -1.82f;

    private CounterList counterList = new CounterList();
    [SerializeField] private CounterDisplay counterDisplayPrefab;
    private Dictionary<CounterType, CounterDisplay> displays = new Dictionary<CounterType, CounterDisplay>();
    private ICanReceiveCounters attachedObject;

    private void Awake()
    {
        attachedObject = GetComponentInParent<ICanReceiveCounters>();
    }

    public void Add(CounterType counterType, int amount)
    {
        counterList.AddCounters(counterType, amount);
        attachedObject.OnCountersAdded(counterType, amount);
        UpdateDisplay(counterType);
    }
    public void Remove(CounterType counterType, int amount)
    {
        counterList.RemoveCounters(counterType, amount);
        attachedObject.OnCountersAdded(counterType, amount);
        UpdateDisplay(counterType);
    }
    public int AmountOf(CounterType counterType)
    {
        return counterList.AmountOf(counterType);
    }
    public void Clear()
    {
        counterList.Clear();
        UpdateDisplays();
    }

    private void UpdateDisplays()
    {
        foreach (CounterType type in counterList.CounterMap.Keys)
            UpdateDisplay(type);
    }

    private void UpdateDisplay(CounterType counterType)
    {
        // get the display if it exists
        if (displays.TryGetValue(counterType, out CounterDisplay display))
        {
            if (counterList.CounterMap.TryGetValue(counterType, out int amount))
            {
                display.gameObject.SetActive(true);
                display.SetText(amount); // if a display exists and there is an amount for it then set the display to the amount
            }
            else
            {
                // A display exists but no amount so set the display to inactive
                display.SetText(0);
                display.gameObject.SetActive(false);
                UpdateDisplayLocations();
            }
        }
        else
        {
            // no display exists so create a new one
            CounterDisplay newDisplay = Instantiate(counterDisplayPrefab, transform);
            displays.Add(counterType, newDisplay);
            newDisplay.SetBackgroundColor(Counters.GetData(counterType).FillColor);
            newDisplay.SetBorderColor(Counters.GetData(counterType).BorderColor);
            newDisplay.SetTextColor(Counters.GetData(counterType).BorderColor);
            newDisplay.SetText(counterList.AmountOf(counterType));
            UpdateDisplayLocations();
        }
    }

    private void UpdateDisplayLocations()
    {
        int index = 0;
        foreach (CounterDisplay display in displays.Values)
        {
            if (!display.gameObject.activeInHierarchy)
                continue;
            Vector3 position = new Vector3(0, index * DISPLAY_Y_OFFSET, 0);
            display.transform.localPosition = position;
            index++;
        }
    }
}
