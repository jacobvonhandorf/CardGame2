using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Creature
{
    public override int getCardId()
    {
        return 56;
    }

    public override int getStartingRange()
    {
        return 1;
    }
}
