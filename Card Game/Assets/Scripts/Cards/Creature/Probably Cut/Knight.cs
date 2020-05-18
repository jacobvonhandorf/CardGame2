using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Creature
{
    public override int getCardId()
    {
        return 57;
    }

    public override int getStartingRange()
    {
        return 1;
    }
}
