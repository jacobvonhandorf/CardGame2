using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWell : Structure, Effect
{
    public const int CARD_ID = 73;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (spell.owner == controller && isActiveAndEnabled)
            addCounters(Counters.well, 1);
    }

    public override void onTurnStart()
    {
        addCounters(Counters.well, 1);
    }

    public override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        sourcePlayer.addMana(1);
        removeCounters(Counters.well, 3);
    }
}
