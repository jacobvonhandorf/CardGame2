using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellCard : Card
{
    public override CardType getCardType() => CardType.Spell;

    public override void play(Tile t)
    {
        doEffect(t);
        moveToCardPile(owner.graveyard, null);
        owner.hand.resetCardPositions();
        GameManager.Get().onSpellCastEffects(this);
    }

    protected abstract void doEffect(Tile t);

    public override bool canBePlayed()
    {
        if (!ownerCanPayCosts())
            return false;
        if (!additionalCanBePlayedChecks())
            return false;
        return true;
    }

    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this spell then do them in this method. Return true if can be played
}
