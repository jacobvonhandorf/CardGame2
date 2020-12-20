using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public interface IHaveReadableStats
{
    bool TryGetValue(StatType statType, out object value);
    UnityEvent E_OnStatChange { get; }
}
