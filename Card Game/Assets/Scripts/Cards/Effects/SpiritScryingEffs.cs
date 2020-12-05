using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritScryingEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(owner.deck.getAllCardsWithTag(Card.Tag.Fairy), 1, 1, "Select a card to add to your hand", owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(owner.hand, card);
        });
    }
}
