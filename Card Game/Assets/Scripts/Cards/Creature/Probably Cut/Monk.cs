using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : Creature
{
    public override int cardId => 58;

    public override Effect getEffect()
    {
        return new MonkEffect();
    }

    // give creature +1/+1 at the cost of an action
    private class MonkEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            sourceCreature.Health += 1;
            sourceCreature.AttackStat += 1;
            sourcePlayer.addActions(-1);
        }
    }
}
