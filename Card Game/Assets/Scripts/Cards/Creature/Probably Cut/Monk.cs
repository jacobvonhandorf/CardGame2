using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override Effect getEffect()
    {
        return new MonkEffect();
    }

    public override int getCardId()
    {
        return 58;
    }

    // give creature +1/+1 at the cost of an action
    private class MonkEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            sourceCreature.setHealth(sourceCreature.getHealth() + 1);
            sourceCreature.setAttack(sourceCreature.getAttack() + 1);
            sourcePlayer.addActions(-1);
        }
    }
}
