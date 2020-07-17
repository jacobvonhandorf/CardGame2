using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBoxEffect : MonoBehaviour
{
    public Player activePlayer;
    public Creature creature;
    public AfterMoveBox afterMoveBox;

    public void setUp(Player activePlayer, Creature creature)
    {
        this.activePlayer = activePlayer;
        this.creature = creature;
    }

    void OnMouseDown()
    {
        if (!creature.hasDoneActionThisTurn)
            GameManager.Get().setUpCreatureEffect(creature);
        else
            GameManager.Get().showToast("This creature's action is unavailable");
        afterMoveBox.hide();
        creature.controller.heldCreature = null;
    }
}
