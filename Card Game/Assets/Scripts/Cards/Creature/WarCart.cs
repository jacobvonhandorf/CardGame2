using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCart : Creature
{
    public override int cardId => 52;

    protected override bool getCanDeployFrom()
    {
        return true;
    }
}
