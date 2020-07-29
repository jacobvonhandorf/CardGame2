using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class StatText : MonoBehaviour
{
    private StatsContainer objectToListenTo;
    private TextMeshPro textMesh;
    [SerializeField] private StatType syncWith;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        objectToListenTo = GetComponentInParent<StatsContainer>();
        objectToListenTo.E_OnStatsChanged += onStatChange;
    }

    private void onStatChange(object s, EventArgs e)
    {
        textMesh.text = objectToListenTo.Stats[syncWith] + "";
    }
}
