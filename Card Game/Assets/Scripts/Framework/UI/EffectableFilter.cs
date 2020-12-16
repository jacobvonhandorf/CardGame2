using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used when player is activating a single tile target effect
// When it is clicked it pays costs and activates the effect on the target tile
public class EffectableFilter : MonoBehaviour
{
    public Tile tile;
    public SingleTileTargetEffect effect;

    private void OnMouseUpAsButton()
    {
        effect.handler.Invoke(tile);
        Board.Instance.SetAllTilesToDefault();
        effect.finished = true;
        Destroy(gameObject);
    }
}
