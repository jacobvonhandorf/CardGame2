using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanReceiveCounters
{
    // called whenever counters are added
    void OnCountersAdded(CounterType counterType, int amount);
}
