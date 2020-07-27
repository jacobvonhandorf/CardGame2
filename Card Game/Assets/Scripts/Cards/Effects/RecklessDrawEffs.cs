using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDrawEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(card.owner.hand.getCardList(), 1, 1, "Select a card to discard", card.owner, delegate (List<Card> cards)
        {
            cards[0].moveToCardPile(card.owner.graveyard, card);
            card.owner.drawCards(2);
        });
    }
}
