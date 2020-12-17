using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableFilter : MonoBehaviour
{
    public Tile tile; // the tile the filter is over

    private void OnMouseDown()
    {
        Debug.Log("attack filter clicked");
        Creature targetCreature = tile.Creature;
        Structure targetStructure = tile.Structure;
        Board.Instance.SetAllTilesToDefault();
        if (targetCreature == null) // attack structure if not creature is on tile
        {
            GameManager.Instance.doAttackOn(targetStructure);
        } else
        {
            GameManager.Instance.doAttackOn(targetCreature);
        }
        GameManager.Instance.ActivePlayer.heldCreature = null;
        GameManager.Instance.NonActivePlayer.heldCreature = null;
    }
}
