using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : Structure
{
    public const int CARD_ID = 5;

    public override int cardId => CARD_ID;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override void onPlaced()
    {
        controller.increaseManaPerTurn(1);
    }

    public override void onRemoved()
    {
        controller.increaseManaPerTurn(-1);
    }

    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Income };
}
