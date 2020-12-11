using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGraspEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        CardPicker.CreateAndQueue(card.owner.Deck.GetAllCardsWithTag(Card.Tag.Arcane), 1, 1, "Select a card to add to your hand", card.owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(card.owner.Hand, card);
            if (card.owner.ControlledCreatures.Find(c => c.HasTag(Card.Tag.Arcane)))
                card.owner.Mana += 1;
        });
    }
}
