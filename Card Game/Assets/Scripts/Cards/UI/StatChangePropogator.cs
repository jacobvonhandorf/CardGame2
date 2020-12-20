using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StatChangePropogator : MonoBehaviour
{
    public IHaveReadableStats Source
    {
        private get
        {
            return source;
        }
        set
        {
            source?.E_OnStatChange?.RemoveListener(OnStatsChanged);
            source = value;
            source.E_OnStatChange?.AddListener(OnStatsChanged);
            OnStatsChanged();
        }
    }
    [SerializeField] private List<StatChangeListener> subscribedTexts;
    private IHaveReadableStats source;

    private void StatsContainer_E_OnStatsChanged(object sender, System.EventArgs e)
    {
        foreach (StatChangeListener statText in subscribedTexts)
            statText.UpdateValue(source);
    }

    private void OnStatsChanged()
    {
        foreach (StatChangeListener statText in subscribedTexts)
            statText.UpdateValue(source);
    }
}
