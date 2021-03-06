﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRecycling : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        CardPicker.CreateAndQueue(Owner.Graveyard.GetAllCardsWithType(CardType.Spell).FindAll(c => c.CardId != (int) CardIds.SpellRecycling), 1, 2, "Select cards to add to your hand", Owner, delegate (List<Card> cards)
        {
            foreach (Card c in cards)
                c.MoveToCardPile(Owner.Hand, card);
        });
    }

    public override bool CanBePlayed => Owner.Graveyard.GetAllCardsWithType(CardType.Spell).Count > 0;
}
