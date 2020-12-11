using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDrawEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        CardPicker.CreateAndQueue(card.owner.Hand.CardList, 1, 1, "Select a card to discard", card.owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(card.owner.Graveyard, card);
            card.owner.DrawCards(2);
        });
    }
}
