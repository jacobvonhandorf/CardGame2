using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBlast : SpellCard, Effect
{
    public const int CARD_ID = 77;
    private const int DAMAGE_AMOUNT = 4;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        targetCreature.takeDamage(DAMAGE_AMOUNT);
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(owner));
    }

    protected override Effect getEffect()
    {
        return this;
    }
}
