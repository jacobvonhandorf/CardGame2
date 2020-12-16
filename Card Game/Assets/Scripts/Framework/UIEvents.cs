using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public static class UIEvents
{
    public class CardArgs : EventArgs { public Card card { get; set; } }

    public static event EventHandler<CardArgs> OnAttemptPlayCard;
    public static void TriggerPlayCard(Card card) { OnAttemptPlayCard?.Invoke(null, new CardArgs() { card = card}); }

    public static event EventHandler<CardArgs> OnCardBeginDrag;
    public static void BeginCardDrag(Card card) { OnCardBeginDrag?.Invoke(null, new CardArgs() { card = card}); }

    public static UnityEvent EnableUIBlocker = new UnityEvent();
    public static UnityEvent DisableUIBlocker = new UnityEvent();
    public static UnityEvent<Card> PermanentHovered = new CardEvent();
}
