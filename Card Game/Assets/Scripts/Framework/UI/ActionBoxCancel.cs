using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBoxCancel : MonoBehaviour
{
    public AfterMoveBox afterMoveBox;
    public GameManager gameManager;

    private void OnMouseDown()
    {
        afterMoveBox.hide();
        Player activePlayer = gameManager.activePlayer;
        activePlayer.heldCreature = null;
        gameManager.setAllTilesToDefault();
    }
}
