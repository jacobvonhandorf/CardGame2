using UnityEngine;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;

public abstract class StatChangeListener : MonoBehaviour
{
    [SerializeField] private StatType syncWith;

    public void UpdateValue(IHaveReadableStats source)
    {
        if (source.TryGetValue(syncWith, out object value))
        {
            if (value != null)
                OnValueUpdated(value);
        }
    }

    protected abstract void OnValueUpdated(object value);
}
