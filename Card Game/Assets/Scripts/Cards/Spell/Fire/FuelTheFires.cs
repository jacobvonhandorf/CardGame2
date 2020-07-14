using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTheFires : SpellCard, Effect
{
    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        CardPicker.CreateAndQueue(sourcePlayer.hand.getCardList(), 0, sourcePlayer.hand.getCardList().Count, "Select cards to disard", sourcePlayer, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
                c.moveToCardPile(sourcePlayer.graveyard, this);
        });
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    public override bool additionalCanBePlayedChecks()
    {
        foreach (Creature c in GameManager.Get().getAllCreaturesControlledBy(owner))
        {
            if (c.hasTag(Tag.Arcane))
                return true;
        }
        return false;
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public override int getCardId()
    {
        return 6;
    }
}
