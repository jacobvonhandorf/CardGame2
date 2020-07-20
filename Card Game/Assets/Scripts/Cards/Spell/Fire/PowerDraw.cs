using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDraw : SpellCard
{
    public const int CARD_ID = 78;
    public override int cardId => CARD_ID;
    private const int NUM_DRAWN_CARDS = 3;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    protected override void doEffect(Tile t)
    {
        owner.drawCards(NUM_DRAWN_CARDS);
    }
}
