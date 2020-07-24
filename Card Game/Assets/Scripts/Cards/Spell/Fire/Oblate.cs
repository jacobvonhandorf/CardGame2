using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oblate : SpellCard
{
    public static bool alreadyActivatedThisTurn = false;
    public override int cardId => 4;
    public override List<Tile> legalTargetTiles => getLegalTargetTiles();

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

    protected override List<Tag> getInitialTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    public List<Tile> getLegalTargetTiles()
    {
        List<Tile> returnList = GameManager.Get().getAllTilesWithStructures(owner);
        returnList.AddRange(GameManager.Get().getAllTilesWithCreatures(owner, true));
        return returnList;
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

    protected override void doEffect(Tile t)
    {
        Card sourceCard;
        if (t.creature != null)
        {
            sourceCard = t.creature.sourceCard;
            GameManager.Get().destroyCreature(t.creature);
        }
        else
        {
            sourceCard = t.structure.sourceCard;
            GameManager.Get().destroyStructure(t.structure, this);
        }
        int amount = sourceCard.getTotalCost();
        owner.addMana(amount);
        owner.drawCards(amount);

        alreadyActivatedThisTurn = true;
    }
}
