using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PermanentCardVisual : CardVisual
{
    [SerializeField] private List<Transform> permanentOnlyIcons;
    [SerializeField] private TextMeshPro hasActedTextIndicator; // move to creature only?
    [SerializeField] private Image friendOrFoeBorder;

    private CardToPermanentConverter cardToPermanentConverter;

    private void Awake()
    {
        cardToPermanentConverter = GetComponent<CardToPermanentConverter>();
    }

    public void ResizeToPermanent(Vector3 newPosition) => cardToPermanentConverter.DoConversion(newPosition);

    // called when a creature leaves the field and needs to act like a card again
    // if the topology of a card is changed this method may need to be changed
    public void ResizeToCard()
    {
        friendOrFoeBorder.gameObject.SetActive(false);

        foreach (Transform icon in permanentOnlyIcons)
        {
            icon.localScale = Vector3.one;
        }

        Vector3 newRootScale = new Vector3(.5f, .5f, 1);
        transform.localScale = newRootScale;
    }

    public void MoveToTile(Tile t, bool inAnimationQueue)
    {
        throw new Exception("Not implemented");
    }

    public void SetIsAlly(bool isAlly)
    {
        friendOrFoeBorder.gameObject.SetActive(true);
        friendOrFoeBorder.color = isAlly ? Color.blue : Color.red;
    }
}
