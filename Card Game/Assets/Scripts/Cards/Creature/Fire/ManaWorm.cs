using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWorm : Creature
{
    public override int cardId => 62;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Arcane };

    private void OnEnable()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    }
    private void OnDisable()
    {
        GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
    }

    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (e.spell.owner == controller)
        {
            addAttack(1);
        }
    }
}
