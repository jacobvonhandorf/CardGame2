using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Effect
{
    void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature);

}
