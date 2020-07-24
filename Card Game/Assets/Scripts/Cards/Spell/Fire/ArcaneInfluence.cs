using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneInfluence : SpellCard
{
    public const int NUM_COUNTERS = 2;
    public const int CARD_ID = 75;
    public override int cardId => CARD_ID;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(owner);

    public override void onInitialization()
    {
        toolTipInfos.Add(ToolTipInfo.arcaneSpell);
    }

    protected override List<Tag> getInitialTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    protected override void doEffect(Tile t)
    {
        t.creature.addCounters(Counters.arcane, NUM_COUNTERS);
        owner.drawCard();
    }
}
