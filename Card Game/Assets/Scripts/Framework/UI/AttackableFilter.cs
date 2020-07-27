using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableFilter : MonoBehaviour
{
    public Tile tile; // the tile the filter is over

    private void OnMouseDown()
    {
        Debug.Log("attack filter clicked");
        Creature targetCreature = tile.creature;
        Structure targetStructure = tile.structure;
        Board.instance.setAllTilesToDefault();
        if (targetCreature == null) // attack structure if not creature is on tile
        {
            GameManager.Get().doAttackOn(targetStructure);
        } else
        {
            GameManager.Get().doAttackOn(targetCreature);
        }
        GameManager.Get().activePlayer.heldCreature = null;
        GameManager.Get().nonActivePlayer.heldCreature = null;
    }
}
