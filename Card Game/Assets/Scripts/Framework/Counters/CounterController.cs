using System;
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
        Debug.Log("Attached object " + attachedObject);
    }

    public void add(CounterType counterType, int amount)
    {
        counterList.addCounters(counterType, amount);
        attachedObject.OnCountersAdded(counterType, amount);
        updateDisplay(counterType);
    }
    public void remove(CounterType counterType, int amount)
    {
        counterList.removeCounters(counterType, amount);
        attachedObject.OnCountersAdded(counterType, amount);
        updateDisplay(counterType);
    }
    public int amountOf(CounterType counterType)
    {
        return counterList.hasCounter(counterType);
    }
    public void clear()
    {
        counterList.clear();
        updateDisplays();
    }

    private void updateDisplays()
    {
        foreach (CounterType type in counterList.CounterMap.Keys)
            updateDisplay(type);
    }

    private void updateDisplay(CounterType counterType)
    {
        // get the display if it exists
        if (displays.TryGetValue(counterType, out CounterDisplay display))
        {
            if (counterList.CounterMap.TryGetValue(counterType, out int amount))
            {
                display.gameObject.SetActive(true);
                display.setText(amount); // if a display exists and there is an amount for it then set the display to the amount
            }
            else
            {
                // A display exists but no amount so set the display to inactive
                display.setText(0);
                display.gameObject.SetActive(false);
                updateDisplayLocations();
            }
        }
        else
        {
            // no display exists so create a new one
            CounterDisplay newDisplay = Instantiate(counterDisplayPrefab);
            newDisplay.gameObject.transform.SetParent(transform);
            displays.Add(counterType, newDisplay);
            newDisplay.setBackgroundColor(Counters.GetData(counterType).FillColor);
            newDisplay.setBorderColor(Counters.GetData(counterType).BorderColor);
            newDisplay.setTextColor(Counters.GetData(counterType).BorderColor);
            newDisplay.setText(counterList.hasCounter(counterType));
            newDisplay.transform.localScale = new Vector3(1, 1, 1);
            updateDisplayLocations();
        }
    }

    private void updateDisplayLocations()
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
