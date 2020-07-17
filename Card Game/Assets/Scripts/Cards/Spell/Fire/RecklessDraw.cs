using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessDraw : SpellCard, Effect
{
    public override int cardId => 12;

    public override List<Tile> getLegalTargetTiles() => GameManager.Get().allTiles();

    protected override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        CardPicker.CreateAndQueue(sourcePlayer.hand.getCardList(), 1, 1, "Select a card to discard", sourcePlayer, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(sourcePlayer.graveyard, this);
        });
    }
}
