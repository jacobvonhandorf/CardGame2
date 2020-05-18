using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : SpellCard
{
    public override List<Tile> getLegalTargetTiles()
    {
        return new List<Tile>();
    }

    protected override Effect getEffect()
    {
        return null;
    }

    public override bool canBePlayed()
    {
        return false;
    }

    public override void onCardAddedByEffect()
    {
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
        return 25;
    }
}
