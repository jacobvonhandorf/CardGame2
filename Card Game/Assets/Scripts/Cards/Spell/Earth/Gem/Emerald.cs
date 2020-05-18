using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emerald : SpellCard
{
    public override bool canBePlayed()
    {
        return false;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return new List<Tile>();
    }

    protected override Effect getEffect()
    {
        return null;
    }

    public override void onCardDrawn()
    {
        if (owner.graveyard.getAllCardsWithTag(Tag.Gem).Count > 0)
            owner.drawCard();
    }

    public override void onCardAddedByEffect()
    {
        if (owner.graveyard.getAllCardsWithTag(Tag.Gem).Count > 0)
            owner.drawCard();
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Gem);
        return tags;
    }

    public override int getCardId()
    {
        return 22;
    }
}
