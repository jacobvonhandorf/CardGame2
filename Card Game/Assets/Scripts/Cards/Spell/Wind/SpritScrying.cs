using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritScrying : SpellCard
{
    public override int cardId => 2;
    public override List<Tile> legalTargetTiles => GameManager.Get().allTiles();

    protected override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(owner.deck.getAllCardsWithTag(Tag.Fairy), 1, 1, "Select a card to add to your hand", owner, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(owner.hand, this);
        });
    }
}
