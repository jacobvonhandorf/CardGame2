using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWorm : Creature
{
    public override int cardId => 62;
    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Arcane };

    public override void onAnySpellCast(SpellCard spell)
    {
        if (spell.owner == controller && sourceCard.isCreature)
        {
            addAttack(1);
        }
    }
}
