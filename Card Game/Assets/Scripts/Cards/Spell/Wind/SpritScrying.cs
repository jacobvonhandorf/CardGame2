using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritScrying : SpellCard, Effect
{
    public override int cardId => 2;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        CardPicker.CreateAndQueue(owner.deck.getAllCardsWithTag(Tag.Fairy), 1, 1, "Select a card to add to your hand", owner, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(owner.hand, this);
        });
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }
}
