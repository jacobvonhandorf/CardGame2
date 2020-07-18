using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inferno : SpellCard
{
    public const int CARD_ID = 14;
    public override int cardId => CARD_ID;
    private const int DAMAGE_AMOUNT = 4;
    public override List<Tile> legalTargetTiles => GameManager.Get().allTiles();

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

    protected override void doEffect(Tile t)
    {
        List<Creature> tempList = new List<Creature>();
        tempList.AddRange(GameManager.Get().allCreatures);
        foreach (Creature c in tempList)
        {
            if (!c.hasTag(Tag.Arcane))
                c.takeDamage(DAMAGE_AMOUNT, this);
        }
    }
}
