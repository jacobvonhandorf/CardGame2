using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyGeneral : Creature
{
    public override int cardId => 49;
    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public override void onInitialization()
    {
        //GameEvents.E_
    }
    public override void onAnyCreaturePlayed(Creature c)
    {
        if (sourceCard.isCreature && c.controller == controller && c != this)
        {
            addAttack(1);
            addHealth(1);
        }
    }

}
