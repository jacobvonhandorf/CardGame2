using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AboveTileFilter : MonoBehaviour, IPointerDownHandler
{
    public TileHandler action;
    public Image image;
    [HideInInspector] public Tile tile;

    public void OnPointerDown(PointerEventData eventData)
    {
        action.Invoke(tile);
    }
}