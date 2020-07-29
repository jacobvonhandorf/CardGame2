using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ArcaneNovice : Creature
{
    public override int cardId => 64;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Arcane };

    public override void onInitialization()
    {
        E_OnDeployed += ArcaneNovice_E_OnDeployed;
    }
    private void OnDestroy()
    {
        E_OnDeployed -= ArcaneNovice_E_OnDeployed;
    }
    private void OnEnable()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    }
    private void OnDisable()
    {
        GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
    }

    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (enabled && Counters.hasCounter(Counters.arcane) > 0 && e.spell.owner == controller)
        {
            controller.drawCard();
            removeCounters(Counters.arcane, 1);
        }
    }
    private void ArcaneNovice_E_OnDeployed(object sender, System.EventArgs e)
    {
        addCounters(Counters.arcane, 1);
    }

}
*/