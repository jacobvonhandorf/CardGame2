using UnityEngine;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;

public abstract class StatChangeListener : MonoBehaviour
{
    //private TextMeshProUGUI textMesh;
    [SerializeField] private StatType syncWith;
    protected Type statType { get; }

    public void updateValue(StatsContainer container)
    {
        if (container.Stats.TryGetValue(syncWith, out object value))
        {
            if (value != null)
                onValueUpdated(container.Stats[syncWith]);
        }
    }

    protected abstract void onValueUpdated(object value);
}
