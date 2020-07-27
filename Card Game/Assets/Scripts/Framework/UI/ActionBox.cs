﻿using System.Collections;
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
        show(c.currentTile.x - 2.4f, c.currentTile.y - 2, c);
    }

    public void Attack()
    {
        if (creature.hasDoneActionThisTurn)
        {
            GameManager.Get().showToast("You have already acted with this creature");
            return;
        }
        GameManager.Get().setUpCreatureAttack(creature);
        gameObject.SetActive(false);
    }
    public void Effect()
    {
        if (!creature.hasDoneActionThisTurn)
            GameManager.Get().setUpCreatureEffect(creature);
        else
            GameManager.Get().showToast("This creature's action is unavailable");
        creature.controller.heldCreature = null;
        gameObject.SetActive(false);
    }
    public void Cancel()
    {
        GameManager.Get().activePlayer.heldCreature = null;
        Board.instance.setAllTilesToDefault();
        gameObject.SetActive(false);
    }
}