using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBoxAttack : MonoBehaviour
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
        if (creature.hasDoneActionThisTurn)
        {
            GameManager.Get().showToast("You have already acted with this creature");
            return;
        }
        GameManager.Get().setUpCreatureAttack(creature);
        afterMoveBox.hide();
    }
}
