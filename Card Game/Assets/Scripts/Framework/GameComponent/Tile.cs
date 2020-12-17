using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IScriptTile
{
    public bool IsPowerTile { get; private set; } = false;
    public List<Tile> AdjacentTiles => Board.Instance.GetAllTilesWithinExactRangeOfTile(this, 1);
    public Permanent Permanent
    {
        get
        {
            if (Creature != null)
                return Creature;
            else
                return Structure;
        }
        set
        {
            if (Creature != null || Structure != null)
            {
                Debug.LogError("Trying to set permanent on a tile that already has a permanent");
                return;
            }

            if (value is Creature c)
                Creature = c;
            else if (value is Structure s)
                Structure = s;
            else if (value == null)
            {
                Creature = null;
                Structure = null;
            }
        }
    }
    public Creature Creature { get; set; }
    public Structure Structure { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    IScriptPermanent IScriptTile.Permanent => Permanent;
    IScriptCreature IScriptTile.Creature => Creature;
    IScriptStructure IScriptTile.Structure => Structure;

    public GameObject effectFilterLocal; // keeps track of filter that highlights effectable tiles.

    [SerializeField] private GameObject attackableFilter; // prefab to instantiate
    [SerializeField] private GameObject effectableFilter; // prefab to instantiate
    [SerializeField] private Color attackColor;
    [SerializeField] private Color effectColor;
    [SerializeField] private Color moveColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color powerTileColor;
    private bool active = false;
    private bool attackable = false;
    private GameObject attackFilterLocal; // keeps track of filter that highlights attackable tiles.
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        GetComponent<OnMouseClickEvents>().OnMouseClick.AddListener(OnClicked);
    }

    public void SetAsPowerTile()
    {
        IsPowerTile = true;
        defaultColor = powerTileColor;
        image.color = powerTileColor;
    }

    #region Utility
    public bool CanDeployFrom => Permanent.CanDeployFrom;
    public int GetDistanceTo(Tile otherTile)
    {
        int distX = Math.Abs(otherTile.X - X);
        int distY = Math.Abs(otherTile.Y - Y);

        return distX + distY;
    }
    public int GetDistanceTo(IScriptTile t) => GetDistanceTo((Tile)t);
    #endregion

    // This should be reworked and moved to a different class
    #region Effects and Attacks Control Flow
    public void SetAttackable(bool attackable)
    {
        this.attackable = attackable;
        if (attackable)
        {
            Debug.Log("Creating attackable filter");
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = attackColor;
            attackFilterLocal = Instantiate(attackableFilter, new Vector3(X-4, Y-3, -1), Quaternion.identity);
            attackFilterLocal.GetComponent<AttackableFilter>().tile = this;
            // attackFilterLocal.GetComponent<AttackableFilter>().gameManager = gameManager;
        }
        else if (attackFilterLocal != null)
        {
            Destroy(attackFilterLocal);
        }
    }
    public void SetEffectable(SingleTileTargetEffect singleTileTargetEffect)
    {
        effectFilterLocal = Instantiate(effectableFilter, new Vector3(X - 4, Y - 3, -1), Quaternion.identity);
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
            image.color = moveColor;
        }
        else
        {
            image.color = defaultColor;
        }
    }
    private void nonActiveClick()
    {
        Player activePlayer = GameManager.Instance.ActivePlayer;
        // figure out what the player was doing before they clicked a non active tile
        if (activePlayer.heldCreature != null) // player was attacking or moving
        {
            activePlayer.heldCreature = null;
            Board.Instance.SetAllTilesToDefault();
        }
    }

    private void doCreatureMove()
    {
        Player playerWithCreature;
        if (GameManager.gameMode == GameManager.GameMode.online)
            playerWithCreature = NetInterface.Get().localPlayer;
        else
            playerWithCreature = GameManager.Instance.ActivePlayer;
        Creature creatureToMove = playerWithCreature.heldCreature;
        creatureToMove.Move(this);
        Board.Instance.SetAllTilesToDefault();
        ActionBox.instance.show(Creature);
        playerWithCreature.heldCreature = null;
    }
    #endregion

    private void OnClicked()
    {
        if (active)
        {
            doCreatureMove();
        }
        else if (attackable)
        {
            GameManager.Instance.doAttackOn(Creature);
            Board.Instance.SetAllTilesToDefault();
        }
        else
        {
            nonActiveClick();
        }
    }
}
