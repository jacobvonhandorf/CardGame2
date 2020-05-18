using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyGeneral : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAnyCreaturePlayed(Creature c)
    {
        if (sourceCard.isCreature && c.controller == controller && c != this)
        {
            addAttack(1);
            addHealth(1);
        }
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Fairy);
        return tags;
    }

    public override int getCardId()
    {
        return 49;
    }
}
