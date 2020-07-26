using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlankSpell : SpellCard
{
    public override int cardId => spellId;
    public int spellId;
    public SpellEffects effects;

    public override List<Tile> legalTargetTiles => effects.validTiles;

    protected override void doEffect(Tile t)
    {
        effects.doEffect(t);
    }
}
