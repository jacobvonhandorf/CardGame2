using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CreatureStatsGetter : CardStatsGetter
{
    [SerializeField] private TextMeshPro hasActedTextIndicator;
   
    public void updateHasActedIndicator(bool hasDoneActionThisTurn, bool hasMovedThisTurn)
    {
        if (!hasDoneActionThisTurn && hasMovedThisTurn)
            hasActedTextIndicator.text = "A";
        else
            hasActedTextIndicator.text = "";
    }
}
