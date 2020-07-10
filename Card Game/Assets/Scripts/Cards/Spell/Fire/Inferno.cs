using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inferno : SpellCard, Effect
{
    private const int DAMAGE_AMOUNT = 4;
    public const int CARD_ID = 14;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        List<Creature> tempList = new List<Creature>();
        tempList.AddRange(GameManager.Get().allCreatures);
        foreach (Creature c in tempList)
        {
            if (!c.hasTag(Tag.Arcane))
                c.takeDamage(DAMAGE_AMOUNT);
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
        return CARD_ID;
    }
}
