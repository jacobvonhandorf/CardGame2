using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterController : MonoBehaviour
{
    private const float DISPLAY_Y_OFFSET = -1.82f;

    private CardCounterList counterList;
    [SerializeField] private CounterDisplay counterDisplayPrefab;

    private void Awake()
    {
        counterList = new CardCounterList(counterDisplayPrefab, transform);
    }

    public void addCounters(CounterClass counterType, int amount)
    {
        counterList.addCounters(counterType, amount);
    }
    public void removeCounters(CounterClass counterType, int amount)
    {
        counterList.removeCounters(counterType, amount);
    }
    public int hasCounter(CounterClass counterType)
    {
        return counterList.hasCounter(counterType);
    }
    public void clearAll()
    {
        counterList.clearAll();
    }

    private class CardCounterList : CounterList
    {
        private CounterDisplay counterDisplayPrefab;
        private Dictionary<CounterClass, CounterDisplay> counterDisplayMap = new Dictionary<CounterClass, CounterDisplay>();
        private Transform parentTransform;

        public CardCounterList(CounterDisplay counterDisplayPrefab, Transform parentTransform)
        {
            this.counterDisplayPrefab = counterDisplayPrefab;
            this.parentTransform = parentTransform;
        }

        public override void addCounters(CounterClass counterType, int amount)
        {
            base.addCounters(counterType, amount);
            updateDisplay(counterType);
        }

        public override void removeCounters(CounterClass counterType, int amount)
        {
            base.removeCounters(counterType, amount);
            updateDisplay(counterType);
        }

        public void clearAll()
        {
            List<CounterClass> tempList = new List<CounterClass>();
            foreach (CounterClass cc in counterAmounts.Keys) tempList.Add(cc);
            foreach (CounterClass cc in tempList)
            {
                counterAmounts.Remove(cc);
                updateDisplay(cc);
            }
        }

        private void updateDisplay(CounterClass counterType)
        {
            // get the display if it exists
            if (counterDisplayMap.TryGetValue(counterType, out CounterDisplay display))
            {
                if (counterAmounts.TryGetValue(counterType, out int amount))
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
                newDisplay.gameObject.transform.SetParent(parentTransform);
                counterDisplayMap.Add(counterType, newDisplay);
                newDisplay.setBackgroundColor(counterType.fillColor());
                newDisplay.setBorderColor(counterType.borderColor());
                newDisplay.setText(counterAmounts[counterType]);
                newDisplay.setTextColor(counterType.borderColor());
                newDisplay.transform.localScale = new Vector3(1, 1, 1);
                updateDisplayLocations();
            }
        }

        private void updateDisplayLocations()
        {
            int index = 0;
            foreach (CounterDisplay display in counterDisplayMap.Values)
            {
                if (!display.gameObject.activeInHierarchy)
                    continue;
                Vector3 position = new Vector3(0, index * DISPLAY_Y_OFFSET, 0);
                display.transform.localPosition = position;
                index++;
            }
        }
    }

}
