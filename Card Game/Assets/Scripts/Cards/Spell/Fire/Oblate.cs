﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oblate : SpellCard, Effect
{
    public static bool alreadyActivatedThisTurn = false;

    public override int cardId => 4;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        Card sourceCard;
        if (targetTile.creature != null)
        {
            sourceCard = targetTile.creature.sourceCard;
            GameManager.Get().destroyCreature(targetTile.creature);
        }
        else
        {
            sourceCard = targetTile.structure.sourceCard;
            GameManager.Get().destroyStructure(targetTile.structure, this);
        }
        int amount = sourceCard.getTotalCost();
        sourcePlayer.addMana(amount);
        sourcePlayer.drawCards(amount);

        alreadyActivatedThisTurn = true;
    }

    public override void onInitialization()
    {
        GameEvents.E_TurnStart += GameEvents_E_TurnStart;
        toolTipInfos.Add(ToolTipInfo.arcaneSpell);
    }
    private void OnDestroy()
    {
        GameEvents.E_TurnStart -= GameEvents_E_TurnStart;
    }

    private void GameEvents_E_TurnStart(object sender, System.EventArgs e)
    {
        alreadyActivatedThisTurn = false;
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        List<Tile> returnList = GameManager.Get().getAllTilesWithStructures(owner);
        returnList.AddRange(GameManager.Get().getAllTilesWithCreatures(owner, true));
        return returnList;
    }

    protected override Effect getEffect()
    {
        return this;
    }

    // must control arcane creature to play
    public override bool additionalCanBePlayedChecks()
    {
        if (alreadyActivatedThisTurn)
            return false;
        foreach (Creature creature in GameManager.Get().getAllCreaturesControlledBy(owner))
        {
            if (creature.hasTag(Tag.Arcane))
            {
                return true;
            }
        }
        return false;
    }
}
