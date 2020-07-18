using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBlast : SpellCard
{
    public const int CARD_ID = 77;
    public override int cardId => CARD_ID;
    private const int DAMAGE_AMOUNT = 4;
    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(owner));

    protected override void doEffect(Tile t)
    {
        t.creature.takeDamage(DAMAGE_AMOUNT, this);
    }
}
