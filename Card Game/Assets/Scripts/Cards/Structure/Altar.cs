using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : Structure
{
    public const int CARD_ID = 5;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override void onPlaced()
    {
        controller.increaseManaPerTurn(1);
    }

    public override void onRemoved()
    {
        controller.increaseManaPerTurn(-1);
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Income);
        return tags;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }
}
