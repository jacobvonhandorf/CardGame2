using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemExchange : SpellCard
{
    public override int cardId => 27;
    public override List<Tile> legalTargetTiles => GameManager.Get().allTiles();
    public override bool additionalCanBePlayedChecks() => owner.hand.getAllCardsWithTag(Tag.Gem).Count > 0;

    protected override void doEffect(Tile t)
    {
        int maxCardsToPick = Mathf.Min(owner.hand.getAllCardsWithTag(Tag.Gem).Count, 2);
        CardPicker.CreateAndQueue(owner.hand.getAllCardsWithTag(Tag.Gem), 1, maxCardsToPick, "Select cards to shuffle back", owner, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
                c.moveToCardPile(owner.graveyard, this);
            owner.drawCards(cardList.Count);
        });
    }
}
