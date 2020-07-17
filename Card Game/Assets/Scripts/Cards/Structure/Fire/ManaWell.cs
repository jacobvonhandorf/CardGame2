using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaWell : Structure, Effect
{
    public const int CARD_ID = 73;
    public override int cardId => CARD_ID;

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
            sourceCard.showInEffectsView();
        }
    }

    private void GameEvents_SpellCastEvent(object sender, GameEvents.SpellCastEventArgs e)
    {
        if (e.spell.owner == controller)
        {
            addCounters(Counters.well, 1);
            sourceCard.showInEffectsView();
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
