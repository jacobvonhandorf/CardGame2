using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDrawEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        CardPicker.CreateAndQueue(card.Owner.Hand.CardList, 1, 1, "Select a card to discard", card.Owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(card.Owner.Graveyard, card);
            card.Owner.DrawCards(2);
        });
    }
}
