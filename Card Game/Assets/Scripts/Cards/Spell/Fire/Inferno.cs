using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inferno : SpellCard, Effect
{
    private const int damage = 4;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        // have to use two seperate lists because this is going to be killing creatures
        foreach (Creature c in GameManager.Get().getAllCreaturesControlledBy(sourcePlayer))
        {
            if (!c.hasTag(Tag.Arcane))
                c.takeDamage(4);
        }
        foreach (Creature c in GameManager.Get().getAllCreaturesControlledBy(targetPlayer))
        {
            if (!c.hasTag(Tag.Arcane))
                c.takeDamage(4);
        }
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
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

    public override int getCardId()
    {
        return 14;
    }
}
