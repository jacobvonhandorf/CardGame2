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

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color powerTileColor;
    private bool active = false;
    private bool attackable = false;
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

    public void OnClicked()
    {
        CreatureMoveControl.Cancel();
        AttackControl.Cancel();
        // need to add single tile target effect control
    }
}
