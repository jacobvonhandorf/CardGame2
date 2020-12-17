using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBox : MonoBehaviour
{
    public static ActionBox instance;
    private Creature creature;

    public void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void show(float x, float y, Creature creature)
    {
        this.creature = creature;
        Vector3 position = gameObject.transform.position;
        position.x = x;
        position.y = y;
        gameObject.transform.position = position;
        gameObject.SetActive(true);
    }
    public void show(Creature c)
    {
        show(c.Tile.X - 2.4f, c.Tile.Y - 2, c);
    }

    public void Attack()
    {
        if (!creature.ActionAvailable)
        {
            Toaster.Instance.DoToast("You have already acted with this creature");
            return;
        }
        GameManager.Instance.setUpCreatureAttack(creature);
        gameObject.SetActive(false);
    }
    public void Effect()
    {
        if (creature.ActionAvailable)
            GameManager.Instance.setUpCreatureEffect(creature);
        else
            Toaster.Instance.DoToast("This creature's action is unavailable");
        creature.Controller.heldCreature = null;
        gameObject.SetActive(false);
    }
    public void Cancel()
    {
        GameManager.Instance.ActivePlayer.heldCreature = null;
        Board.Instance.SetAllTilesToDefault();
        gameObject.SetActive(false);
    }
}
