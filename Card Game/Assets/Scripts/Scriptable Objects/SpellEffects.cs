using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellEffects : MonoBehaviour
{
    [HideInInspector] public Card card;

    public abstract void DoEffect(Tile t);
    public abstract List<Tile> ValidTiles { get; }
    public virtual bool CanBePlayed { get; } = true;

    public virtual EmptyHandler OnInitilization { get; }
    public virtual EventHandler<Card.AddedToCardPileArgs> OnMoveToCardPile { get; }

    public Player Owner { get { return card.Owner; } }
}
