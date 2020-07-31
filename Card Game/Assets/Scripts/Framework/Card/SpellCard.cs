using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public override CardType getCardType() => CardType.Spell;
    public int spellId;
    public SpellEffects effects;
    public override List<Tile> legalTargetTiles => GetComponent<SpellEffects>().validTiles;

    public override void play(Tile t)
    {
        doEffect(t);
        moveToCardPile(owner.graveyard, null);
        owner.hand.resetCardPositions();
        GameManager.Get().onSpellCastEffects(this);
    }

    private void doEffect(Tile t)
    {
        GetComponent<SpellEffects>().doEffect(t);
    }

    public override bool canBePlayed()
    {
        if (!ownerCanPayCosts())
            return false;
        if (!additionalCanBePlayedChecks())
            return false;
        return true;
    }

    public override void initialize()
    {
        onInitilization?.Invoke();
        onInitilization = null;
    }

    public virtual bool additionalCanBePlayedChecks() => GetComponent<SpellEffects>().canBePlayed; // if some conditions need to be met before playing this spell then do them in this method. Return true if can be played
}
