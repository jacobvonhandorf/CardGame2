using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCart : Creature
{
    public override int getCardId()
    {
        return 52;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    protected override bool getCanDeployFrom()
    {
        return true;
    }
}
