using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used when player is activating a single tile target effect
// When it is clicked it pays costs and activates the effect on the target tile
public class EffectableFilter : MonoBehaviour
{
    public Tile tile;
    public Effect effect;
    public Creature sourceCreature;
    public Structure sourceStructure;
    public Player sourcePlayer;

    private void OnMouseUpAsButton()
    {
        if (effect is SingleTileTargetEffect)
        {
            if (sourceStructure != null) // effect is coming from a structure
            {
                sourcePlayer = sourceStructure.controller;
                Player targetPlayer = GameManager.Get().getOppositePlayer(sourcePlayer);
                sourcePlayer.subtractActions(sourceStructure.effectActionsCost);
                sourcePlayer.addGold(-sourceStructure.effectGoldCost);
                sourcePlayer.addMana(-sourceStructure.effectActionsCost);
                effect.activate(sourcePlayer, targetPlayer, sourceStructure.tile, tile, null, null);
            }
            else if (sourceCreature != null) // effect is coming from a creature
            {
                Debug.Log("Effect with a source creature");
                sourcePlayer = sourceCreature.controller;
                //int actionsToSubtract = sourceCreature.effectActionCost;
                //if (sourceCreature.hasMovedThisTurn) // if the player has already spent an action on this creature then don't subtract one more
                //    actionsToSubtract--;
                //sourcePlayer.subtractActions(actionsToSubtract);
                Debug.Log("Effect action cost " + sourceCreature.effectActionCost);
                //if (sourceCreature.effectActionCost > 0)
                //    sourceCreature.hasDoneActionThisTurn = true;
                sourceCreature.updateHasActedIndicators();
                Player targetPlayer = GameManager.Get().getOppositePlayer(sourcePlayer);
                effect.activate(sourcePlayer, targetPlayer, sourceCreature.currentTile, tile, sourceCreature, tile.creature);
            }
            else if (sourcePlayer != null) // effect is not tied to a creature or a structure
            {
                effect.activate(sourcePlayer, GameManager.Get().getOppositePlayer(sourcePlayer), null, tile, sourceCreature, tile.creature);
            }
            else
            {
                throw new Exception("Effects need at least a source player for online play later");
            }
        }
        else
        {
            throw new NotImplementedException();
        }
        Destroy(gameObject);
        GameManager.Get().setAllTilesToDefault();
        EffectsManager.Get().signalEffectFinished();
    }
}
