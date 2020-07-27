using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlankSpell : SpellCard
{
    public override int cardId => spellId;
    public int spellId;
    public SpellEffects effects;

    public override List<Tile> legalTargetTiles => GetComponent<SpellEffects>().validTiles; //effects.validTiles;

    protected override void doEffect(Tile t)
    {
        GetComponent<SpellEffects>().doEffect(t);
    }

    public override void initialize()
    {
        onInitilization?.Invoke();
        onInitilization = null;
    }
}
