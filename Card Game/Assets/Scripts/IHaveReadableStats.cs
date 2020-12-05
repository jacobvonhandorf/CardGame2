using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public interface IHaveReadableStats
{
    //object GetValue(StatType statType)
    bool TryGetValue(StatType statType, out object value);
    //Dictionary<StatType, object> StatList { get; }
    UnityEvent E_OnStatChange { get; }
}
