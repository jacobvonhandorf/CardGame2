using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obsidian : SpellCard, SingleTileTargetEffect
{
    public const int CARD_ID = 81;
    private const int DAMAGE_AMOUNT = 1;

    public override int getCardId()
    {
        return CARD_ID;
    }

    private List<Tile> emptyList = new List<Tile>();
    public override List<Tile> getLegalTargetTiles()
    {
        return emptyList;
    }

    protected override Effect getEffect()
    {
        return null;
    }

    public override void onCardAddedByEffect()
    {
        doEffect();
    }

    public override void onCardDrawn()
    {
        doEffect();
    }

    private void doEffect()
    {
        owner.drawCard();
        GameManager.Get().setUpSingleTileTargetEffect(this, owner, null, null, null, "Deal 1 damage", false);
    }

    // Effect
    public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
    {
        return GameManager.Get().getAllTilesWithCreatures(oppositePlayer);
    }

    public bool canBeCancelled()
    {
        return true;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        targetCreature.takeDamage(DAMAGE_AMOUNT);
    }

    protected override List<Tag> getTags()
    {
        return new List<Tag>() { Tag.Gem };
    }
}
