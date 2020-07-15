﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Creature creature;
    public Effect effect;
    public Structure structure;
    public int x;
    public int y;
    private bool active = false;
    private bool attackable = false;
    private bool effectable = false;

    [SerializeField] private GameObject attackableFilter; // prefab to instantiate
    [SerializeField] private GameObject effectableFilter; // prefab to instantiate
    [SerializeField] private Color attackColor;
    [SerializeField] private Color effectColor;
    [SerializeField] private Color moveColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color powerTileColor;
    private GameObject attackFilterLocal; // keeps track of filter that highlights attackable tiles.
    public GameObject effectFilterLocal; // keeps track of filter that highlights effectable tiles.
    private bool powerTile = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void addCreature(Creature creature)
    {
        this.creature = creature;
        onCreatureAdded();
    }

    private void onCreatureAdded()
    {
        if (structure != null)
            structure.onCreatureAdded(creature);
    }

    private void onCreatureRemoved()
    {
        if (structure != null)
            structure.onCreatureRemoved(creature);
    }

    internal void setAsPowerTile()
    {
        powerTile = true;
        defaultColor = powerTileColor;
        GetComponent<SpriteRenderer>().color = powerTileColor;
    }

    public bool isPowerTile()
    {
        return powerTile;
    }

    public int getDistanceTo(Tile otherTile)
    {
        int distX = Math.Abs(otherTile.x - x);
        int distY = Math.Abs(otherTile.y - y);

        return distX + distY;
    }

    public List<Tile> getAdjacentTiles()
    {
        return GameManager.Get().board.getAllTilesWithinExactRangeOfTile(this, 1);
    }

    public void removeCreature()
    {
        onCreatureRemoved();
        creature = null;
    }

    public void setAttackable(bool attackable)
    {
        this.attackable = attackable;
        if (attackable)
        {
            Debug.Log("Creating attackable filter");
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = attackColor;
            attackFilterLocal = Instantiate(attackableFilter, new Vector3(x-4, y-3, -1), Quaternion.identity);
            attackFilterLocal.GetComponent<AttackableFilter>().tile = this;
            // attackFilterLocal.GetComponent<AttackableFilter>().gameManager = gameManager;
        }
        else if (attackFilterLocal != null)
        {
            Destroy(attackFilterLocal);
        }
    }

    public void setEffectable(SingleTileTargetEffect singleTileTargetEffect)
    {
        effectFilterLocal = Instantiate(effectableFilter, new Vector3(x - 4, y - 3, -1), Quaternion.identity);
        EffectableFilter filter = effectFilterLocal.GetComponent<EffectableFilter>();
        filter.tile = this;
        filter.effect = singleTileTargetEffect;
    }

    public void setEffectableFalse()
    {
        if (effectFilterLocal != null)
        {
            Destroy(effectFilterLocal);
        }
    }

    public void setActive(bool active)
    {
        this.active = active;
        if (active)
        {
            sr.color = moveColor;
        }
        else
        {
            sr.color = defaultColor;
        }
    }

    void OnMouseUpAsButton()
    {
        if (active)
        {
            doCreatureMove();
        }
        else if (attackable)
        {
            GameManager.Get().doAttackOn(creature);
            GameManager.Get().setAllTilesToNotAttackable();
        }
        else
        {
            nonActiveClick();
        }
    }

    private void nonActiveClick()
    {
        Player activePlayer = GameManager.Get().activePlayer;
        // figure out what the player was doing before they clicked a non active tile
        if (activePlayer.heldCreature != null) // player was attacking or moving
        {
            activePlayer.heldCreature = null;
            GameManager.Get().setAllTilesToDefault();
        }
    }

    private void doCreatureMove()
    {
        Player playerWithCreature;
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            playerWithCreature = NetInterface.Get().getLocalPlayer();
        }
        else
        {
            playerWithCreature = GameManager.Get().activePlayer;
        }
        Creature creatureToMove = playerWithCreature.heldCreature;
        GameManager.Get().moveCreatureToTile(creatureToMove, this);
        GameManager.Get().setAllTilesToNotActive();
        playerWithCreature.heldCreature = null;
    }

    private SpriteRenderer sr;
    public void setColor(Color color)
    {
        sr.color = color;
    }
}
