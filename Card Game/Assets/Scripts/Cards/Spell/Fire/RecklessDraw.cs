using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDraw : SpellCard
{
    public override int cardId => 12;
    public override List<Tile> legalTargetTiles => GameManager.Get().allTiles();

    protected override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(owner.hand.getCardList(), 1, 1, "Select a card to discard", owner, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(owner.graveyard, this);
        });
    }
}
