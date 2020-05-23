using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SingleTileTargetEffect : Effect
{
    List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile);
    bool canBeCancelled();
}
