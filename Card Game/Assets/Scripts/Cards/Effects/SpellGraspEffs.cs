using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGraspEffs : SpellEffects
{
    public override List<Tile> validTiles => Board.instance.allTiles;

    public override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(card.owner.deck.getAllCardsWithTag(Card.Tag.Arcane), 1, 1, "Select a card to add to your hand", card.owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(card.owner.hand, card);
            if (card.owner.getAllControlledCreatures().Find(c => c.HasTag(Card.Tag.Arcane)))
                card.owner.addMana(1);
        });
    }
}
