using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterMoveBox : MonoBehaviour
{
    public ActionBoxAttack attackText;
    public ActionBoxEffect effectText;
    public ActionBoxCancel cancelText;
    // private Creature creature;

    public void show(float x, float y, Player activePlayer, Creature creature)
    {
        attackText.setUp(activePlayer, creature);
        effectText.setUp(activePlayer, creature);

        Vector3 transform = gameObject.transform.position;
        transform.x = x;
        transform.y = y;
        gameObject.transform.position = transform;
        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
}
