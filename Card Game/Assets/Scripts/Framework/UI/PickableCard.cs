using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableCard : MonoBehaviour
{
    public Card sourceCard;
    private CanReceiveCardPick cardPickReceiver;
    private GameObject selectedBackground;
    private bool selected = false;
    [SerializeField] private GameObject highlightBackgroundPrefab;
    private GameObject highlightBackgroundLocal;


    public void setUp(CanReceiveCardPick receiver, Card sourceCard)
    {
        cardPickReceiver = receiver;
        this.sourceCard = sourceCard;
    }

    // when clicked selected or deselect the card
    private void OnMouseUpAsButton()
    {
        if (!selected)
        {
            cardPickReceiver.receiveCardPick(sourceCard);
            selected = true;
            highlightBackgroundLocal = Instantiate(highlightBackgroundPrefab, transform);
        }
        else
        {
            cardPickReceiver.removeCardPick(sourceCard);
            selected = false;
            Destroy(highlightBackgroundLocal);
        }

    }
}
