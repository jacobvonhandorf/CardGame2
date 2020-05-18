using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneNovice : Creature
{
    private bool drawAvailable = true;

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (drawAvailable && sourceCard.isCreature && spell.owner == controller)
        {
            controller.drawCard();
            drawAvailable = false;
        }
    }

    public override void onCreation()
    {
        drawAvailable = true;
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    public override int getCardId()
    {
        return 64;
    }
}
