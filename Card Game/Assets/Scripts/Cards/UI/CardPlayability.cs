using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayability : MonoBehaviour
{
    private Card card;
    private void Awake()
    {
        card = GetComponent<Card>();
        if (card == null)
            throw new System.Exception("Card is null");
    }
    public void attemptToPlayCard()
    {
        UIEvents.TriggerPlayCard(card);
    }
}
