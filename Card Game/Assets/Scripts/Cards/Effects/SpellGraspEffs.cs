using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGraspEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        CardPicker.CreateAndQueue(card.Owner.Deck.GetAllCardsWithTag(Tag.Arcane), 1, 1, "Select a card to add to your hand", card.Owner, delegate (List<Card> cards)
        {
            cards[0].MoveToCardPile(card.Owner.Hand, card);
            if (card.Owner.ControlledCreatures.Find(c => c.HasTag(Tag.Arcane)))
                card.Owner.Mana += 1;
        });
    }
}
