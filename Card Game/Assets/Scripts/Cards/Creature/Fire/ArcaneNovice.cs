﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneNovice : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (hasCounter(Counters.arcane) > 0 && sourceCard.isCreature && spell.owner == controller)
        {
            controller.drawCard();
            removeCounters(Counters.arcane, 1);
        }
    }

    public override void onCreation()
    {
        addCounters(Counters.arcane, 1);
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
