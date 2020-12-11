using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritScryingEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        CardPicker.CreateAndQueue(Owner.Deck.GetAllCardsWithTag(Card.Tag.Fairy), 1, 1, "Select a card to add to your hand", Owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(Owner.Hand, card);
        });
    }
}
