using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWell : Structure, Effect
{
    public const int CARD_ID = 73;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    /*
    public override void onAnySpellCast(SpellCard spell)
    {
        if (spell.owner == controller && isActiveAndEnabled)
            addCounters(Counters.well, 1);
    }

    public override void onTurnStart()
    {
        if (GameManager.Get().activePlayer == controller)
            addCounters(Counters.well, 1);
    }*/

    private void OnEnable()
    {
        GameEvents.E_SpellCast += GameEvents_SpellCastEvent;
        GameEvents.E_TurnStart += GameEvents_E_TurnStart;
    }

    private void OnDisable()
    {
        GameEvents.E_SpellCast -= GameEvents_SpellCastEvent;
        GameEvents.E_TurnStart -= GameEvents_E_TurnStart;
    }

    private void GameEvents_E_TurnStart(object sender, System.EventArgs e)
    {
        if (GameManager.Get().activePlayer == controller)
        {
            addCounters(Counters.well, 1);
            sourceCard.showInEffectsQueue();
        }
    }


    private void GameEvents_SpellCastEvent(object sender, GameEvents.SpellCastEventArgs e)
    {
        if (e.spell.owner == controller)
        {
            addCounters(Counters.well, 1);
            sourceCard.showInEffectsQueue();
        }
    }

    public override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (hasCounter(Counters.well) < 3)
        {
            GameManager.Get().showToast("You need 3 well counters to activate this effect");
            return;
        }
        sourcePlayer.addMana(1);
        removeCounters(Counters.well, 3);
    }
}
