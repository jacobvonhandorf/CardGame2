using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWorm : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (spell.owner == controller && sourceCard.isCreature)
        {
            addAttack(1);
        }
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    public override int getCardId()
    {
        return 62;
    }
}
