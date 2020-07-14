using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemExchange : SpellCard, Effect
{
    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public override bool canBePlayed()
    {
        return owner.hand.getAllCardsWithTag(Tag.Gem).Count > 0 && base.canBePlayed();
    }

    public override int getCardId()
    {
        return 27;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        int maxCardsToPick = Mathf.Min(sourcePlayer.hand.getAllCardsWithTag(Tag.Gem).Count, 2);
        CardPicker.CreateAndQueue(sourcePlayer.hand.getAllCardsWithTag(Tag.Gem), 1, maxCardsToPick, "", sourcePlayer, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
                c.moveToCardPile(sourcePlayer.graveyard, this);
            sourcePlayer.drawCards(cardList.Count);
        });
    }
}
