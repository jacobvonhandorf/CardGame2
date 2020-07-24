using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestActivatedEffect : CreatureActivatedEffect
{
    public override void activate(Creature creature)
    {
        Debug.Log("Activating effect");
    }
}
