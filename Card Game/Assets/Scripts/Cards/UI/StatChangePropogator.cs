using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            // THIS CAN CAUSE PERFORMANCE PROBLEMS BUT I COULDN'T GET IT TO NOT DO A NULL POINTER
            //if (statsContainer != null)
            //statsContainer.E_OnStatsChanged -= StatsContainer_E_OnStatsChanged;
            source?.E_OnStatChange?.RemoveListener(OnStatsChanged);
            source = value;
            source.E_OnStatChange?.AddListener(OnStatsChanged);
            OnStatsChanged();
            //statsContainer.E_OnStatsChanged += StatsContainer_E_OnStatsChanged;
        }
    }
    [SerializeField] private List<StatChangeListener> subscribedTexts;
    private IHaveReadableStats source;

    private void StatsContainer_E_OnStatsChanged(object sender, System.EventArgs e)
    {
        //Debug.Log("Propogating stat change");
        foreach (StatChangeListener statText in subscribedTexts)
        {
            statText.UpdateValue(source);
        }
    }

    private void OnStatsChanged()
    {
        foreach (StatChangeListener statText in subscribedTexts)
            statText.UpdateValue(source);
    }
}
