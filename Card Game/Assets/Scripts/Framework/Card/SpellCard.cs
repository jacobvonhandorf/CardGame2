using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellCard : Card
{
    public override CardType getCardType()
    {
        return CardType.Spell;
    }

    public override void play(Tile t)
    {
        GameManager.Get().onSpellCastEffects(this);
        getEffect().activate(owner, GameManager.Get().getOppositePlayer(owner), t, t, t.creature, t.creature);
        moveToCardPile(owner.graveyard, null);
        owner.hand.resetCardPositions();
        GameManager.Get().afterSpellCastEffects();

    }

    protected abstract Effect getEffect();

    public override bool canBePlayed()
    {
        if (!ownerCanPayCosts())
            return false;
        if (!additionalCanBePlayedChecks())
            return false;
        return true;
    }

    public override List<Keyword> getInitialKeywords()
    {
        return getSpellInitialKeywords();
    }

    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this spell then do them in this method. Return true if can be played
    public virtual List<Keyword> getSpellInitialKeywords() { return new List<Keyword>(); }
}
