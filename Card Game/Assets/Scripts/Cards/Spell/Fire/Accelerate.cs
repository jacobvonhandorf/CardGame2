using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerate : SpellCard, Effect
{
    public override int cardId => 18;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        sourcePlayer.drawCard();
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }

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

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }
}
