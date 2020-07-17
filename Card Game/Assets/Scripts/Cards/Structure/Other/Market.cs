using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : Structure
{
    public const int CARD_ID = 29;
    public override int cardId => CARD_ID;
    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Income };

    public override void onPlaced()
    {
        controller.increaseGoldPerTurn(1);
    }

    public override void onRemoved()
    {
        controller.increaseGoldPerTurn(-1);
    }
}
