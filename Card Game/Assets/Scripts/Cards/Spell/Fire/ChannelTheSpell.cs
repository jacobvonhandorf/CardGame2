﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelTheSpell : SpellCard
{
    public override int cardId => 13;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

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

    protected override List<Tag> getInitialTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    protected override void doEffect(Tile t)
    {
        int numCardsToReduce = 3;
        foreach (Card c in owner.deck.getCardList())
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
        owner.drawCard();
    }
}
