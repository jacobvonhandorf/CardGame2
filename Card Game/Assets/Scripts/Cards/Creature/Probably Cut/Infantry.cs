using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infantry : Creature
{
    public override int getCardId()
    {
        return 59;
    }

    public override int getStartingRange()
    {
        return 1;
    }
}
