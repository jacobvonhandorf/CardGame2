using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatChangePropogator : MonoBehaviour
{
    [SerializeField] private List<StatChangeListener> subscribedTexts;
    public StatsContainer StatsContainer {
        private get
        {
            return statsContainer;
        }
        set
        {
            // THIS CAN CAUSE PERFORMANCE PROBLEMS BUT I COULDN'T GET IT TO NOT DO A NULL POINTER
            //if (statsContainer != null)
                //statsContainer.E_OnStatsChanged -= StatsContainer_E_OnStatsChanged;
            statsContainer = value;
            statsContainer.E_OnStatsChanged += StatsContainer_E_OnStatsChanged;
        }
    }
    private StatsContainer statsContainer;

    private void StatsContainer_E_OnStatsChanged(object sender, System.EventArgs e)
    {
        Debug.Log("Propogating stat change");
        foreach (StatChangeListener statText in subscribedTexts)
        {
            statText.updateValue(statsContainer);
        }
    }
}
