using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerate : SpellCard
{
    public override int cardId => 18;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    public override void onInitialization()
    {
        toolTipInfos.Add(ToolTipInfo.arcaneSpell);
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

    protected override List<Tag> getInitialTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    protected override void doEffect(Tile t)
    {
        owner.drawCard();
    }
}
