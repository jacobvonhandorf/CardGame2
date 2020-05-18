using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : Structure
{
    public const int CARD_ID = 29;

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
        controller.increaseGoldPerTurn(1);
    }

    public override void onRemoved()
    {
        controller.increaseGoldPerTurn(-1);
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
