using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public override CardType getCardType() => CardType.Spell;
    [HideInInspector] public SpellEffects effects;
    public override List<Tile> LegalTargetTiles => GetComponent<SpellEffects>().ValidTiles;

    public override void Play(Tile t)
    {
        DoEffect(t);
        MoveToCardPile(owner.Graveyard, null);
        GameManager.Get().onSpellCastEffects(this);
    }

    private void DoEffect(Tile t)
    {
        GetComponent<SpellEffects>().DoEffect(t);
    }

    public override bool CanBePlayed()
    {
        if (!OwnerCanPayCosts())
            return false;
        if (!additionalCanBePlayedChecks())
            return false;
        return true;
    }

    public override void Initialize()
    {
        onInitilization?.Invoke();
        onInitilization = null;
    }

    public virtual bool additionalCanBePlayedChecks() => GetComponent<SpellEffects>().CanBePlayed; // if some conditions need to be met before playing this spell then do them in this method. Return true if can be played
}
