using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDraw : SpellCard, Effect
{
    public const int CARD_ID = 78;
    private const int NUM_DRAWN_CARDS = 4;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        sourcePlayer.drawCards(NUM_DRAWN_CARDS);
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }
    
}
