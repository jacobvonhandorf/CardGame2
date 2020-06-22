using System.Collections;
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
        if (sourceCard.isCreature && hasCounter(Counters.arcane) > 0 && spell.owner == controller)
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

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
