using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Holds an effect and the information needed to activate it for use later
 * ex: activating an effect at end of turn
 */ 
public class EffectActuator
{
    public Effect effect;
    public Player sourcePlayer;
    public Player targetPlayer;
    public Tile sourceTile;
    public Tile targetTile;
    public Creature sourceCreature;
    public Creature targetCreature;
    public string informationText; // text to display while activating this effect

    public void activate()
    {
        effect.activate(sourcePlayer, targetPlayer, sourceTile, targetTile, sourceCreature, targetCreature);
    }


}
