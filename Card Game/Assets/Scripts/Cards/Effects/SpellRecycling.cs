using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRecycling : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(owner.graveyard.getAllCardsWithType(Card.CardType.Spell).FindAll(c => c.cardId != (int) CardIds.SpellRecycling), 1, 2, "Select cards to add to your hand", owner, delegate (List<Card> cards)
        {
            foreach (Card c in cards)
                c.MoveToCardPile(owner.hand, card);
        });
    }

    public override bool canBePlayed => owner.graveyard.getAllCardsWithType(Card.CardType.Spell).Count > 0;
}
