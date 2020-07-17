
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelTheSpell : SpellCard, Effect
{
    public override int cardId => 13;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        int numCardsToReduce = 3;
        foreach (Card c in sourcePlayer.deck.getCardList())
        {
            if (c.isType(CardType.Spell))
            {
                c.setManaCost(c.getManaCost() - 1);
                if (c.getManaCost() < 0)
                    c.setManaCost(0);
                numCardsToReduce--;
                if (numCardsToReduce == 0)
                    break;
            }
        }
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
        foreach (Creature c in owner.getAllControlledCreatures())
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
