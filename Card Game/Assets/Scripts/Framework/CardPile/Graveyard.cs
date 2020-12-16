using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Graveyard : ViewableCardPile
{
    // private Card topCard; // most recent card added to grave. Will be shown on top.
    protected override string PileName => "Graveyard";

    protected override void OnCardAdded(Card c)
    {
        if (c is CreatureCard cAsCreatureCard)
            cAsCreatureCard.Creature.RemoveFromCurrentTile();
        else if (c is StructureCard cAsStructureCard)
            cAsStructureCard.structure.RemoveFromCurrentTile();

        // remove the card from the scene
        c.removeGraphicsAndCollidersFromScene();

        // when cards are sent to grave they forget stat madifications
        c.resetToBaseStatsWithoutSyncing();
        // if network game mode need to sync creature stats here because the creature is set to not active so it won't sync itself
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            NetInterface.Get().SyncCardStats(c);
            if (c is CreatureCard)
                NetInterface.Get().SyncCreatureStats((c as CreatureCard).Creature);
            else if (c is StructureCard)
                NetInterface.Get().SyncStructureStats((c as StructureCard).structure);
        }
    }

    protected override void OnCardRemoved(Card c)
    {
        NumCardsChanged.Invoke();
    }
}
