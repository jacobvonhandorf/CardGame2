using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneInfluence : SpellCard, Effect
{
    public const int CARD_ID = 75;
    public const int NUM_COUNTERS = 2;

    public override int cardId => CARD_ID;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        targetCreature.addCounters(Counters.arcane, NUM_COUNTERS);
        sourcePlayer.drawCard();
    }

    public override void onInitialization()
    {
        toolTipInfos.Add(ToolTipInfo.arcaneSpell);
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(owner);
    }

    protected override Effect getEffect()
    {
        return this;
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }
}
