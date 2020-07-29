using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellEffects : MonoBehaviour
{
    [HideInInspector] public Card card;

    public abstract void doEffect(Tile t);
    public abstract List<Tile> validTiles { get; }
    public virtual bool canBePlayed { get; } = true;

    public virtual EmptyHandler onInitilization { get; }
    public virtual EventHandler<Card.AddedToCardPileArgs> onMoveToCardPile { get; }

    public Player owner { get { return card.owner; } }
}
