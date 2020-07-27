using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTower : Structure
{
    public const int CARD_ID = 43;
    public override int cardId => CARD_ID;
    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Income };

    public override void onPlaced()
    {
        controller.increaseActionsPerTurn(1);
    }
    public override void onRemoved()
    {
        controller.increaseActionsPerTurn(-1);
    }

}
