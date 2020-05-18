using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercenary : Creature
{
    public const int CARD_ID = 69;

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override int getStartingRange()
    {
        return 1;
    }
}
