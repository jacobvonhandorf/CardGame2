using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGrasp : SpellCard, Effect
{
    public const int CARD_ID = 3;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        CardPicker.CreateAndQueue(owner.deck.getAllCardsWithTag(Tag.Arcane), 1, 1, "Select a card to add to your hand", owner, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(owner.hand, this);
            foreach (Creature creature in GameManager.Get().getAllCreaturesControlledBy(owner))
            {
                if (creature.hasTag(Tag.Arcane))
                {
                    owner.addMana(1);
                    break;
                }
            }
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

    public override int getCardId()
    {
        return CARD_ID;
    }
}
