using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleTileTargetEffect : Effect
{
    public abstract void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature);

    public abstract List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile);

    public virtual bool canBeCancelled() { return true; }
}
